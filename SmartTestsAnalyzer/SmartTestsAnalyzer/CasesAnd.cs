using System.Collections.Generic;
using System.Linq;
using System.Text;

using JetBrains.Annotations;


#if !EXTENSION

using System.Diagnostics;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

#endif



namespace SmartTestsAnalyzer
{
    [UsedImplicitly( ImplicitUseTargetFlags.WithMembers )]
    public class CasesAnd
    {
#if EXTENSION

        public Dictionary<string, Case> Cases { get; } = new Dictionary<string, Case>();
        public bool HasError { get; set; }
        public bool IsMissing { get; set; }

#else

        private CasesAnd()
        { }


        public CasesAnd( ExpressionSyntax parameterNameExpression, string parameterName, ExpressionSyntax caseExpression, IFieldSymbol criterion, bool hasError )
        {
            Debug.Assert( parameterName != null );

            Case currentCase;
            if( !Cases.TryGetValue( parameterName, out currentCase ) )
            {
                currentCase = new Case( parameterNameExpression, parameterName );
                Cases[ parameterName ] = currentCase;
            }
            currentCase.Add( caseExpression, criterion, hasError );
            if( currentCase.HasError )
                HasError = true;
        }


        public Dictionary<string, Case> Cases { get; } = new Dictionary<string, Case>();
        public bool HasError { get; private set; }
        public bool IsMissing { get; set; }


        public CasesAnd CombineAnd( CasesAnd otherCases )
        {
            var result = new CasesAnd();
            result.FillCasesWith( Cases );
            result.FillCasesWith( otherCases.Cases );
            result.HasError = HasError || otherCases.HasError;
            return result;
        }


        private void FillCasesWith( Dictionary<string, Case> other )
        {
            foreach( var otherCase in other.Values )
            {
                var parameterName = otherCase.ParameterName;
                Case parameterCase;
                if( !Cases.TryGetValue( parameterName, out parameterCase ) )
                {
                    parameterCase = new Case( otherCase.ParameterNameExpression, parameterName );
                    Cases[ parameterName ] = parameterCase;
                }
                parameterCase.FillWith( otherCase );
            }
        }


        public void FillCriteriaTypes( Dictionary<string, HashSet<ITypeSymbol>> criteriaTypes )
        {
            foreach( var pair in Cases )
            {
                HashSet<ITypeSymbol> types;
                if( !criteriaTypes.TryGetValue( pair.Key, out types ) )
                {
                    types = new HashSet<ITypeSymbol>();
                    criteriaTypes[ pair.Key ] = types;
                }

                pair.Value.FillCriteriaTypes( types );
            }
        }


        public void FillExpressionSyntax( List<ExpressionSyntax> result )
        {
            foreach( var aCase in Cases.Values )
                aCase.FillExpressionSyntax( result );
        }

#endif


        public override bool Equals( object other )
        {
            var otherCriterias = other as CasesAnd;
            if( otherCriterias?.Cases.Count != Cases.Count )
                return false;

            foreach( var otherCase in otherCriterias.Cases )
            {
                if (!Cases.TryGetValue(otherCase.Key, out Case parameterCase))
                    // Not same parameter name!
                    return false;

                if ( !Equals( otherCase.Value, parameterCase ) )
                    return false;
            }
            return true;
        }


        public override int GetHashCode() => Cases.Aggregate( 0,
                                                              ( current, pair ) => current ^
                                                                                   pair.Key.GetHashCode() ^
                                                                                   pair.Value.GetHashCode() );


        public void ToString( StringBuilder result )
        {
            foreach( var aCase in Cases.Values )
            {
                aCase.ToString( result );
                result.Append( " & " );
            }
            result.Length -= 3;
        }


        public override string ToString()
        {
            var result = new StringBuilder();
            ToString( result );
            return result.ToString();
        }
    }
}