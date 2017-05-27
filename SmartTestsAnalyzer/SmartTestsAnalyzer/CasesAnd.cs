using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;



namespace SmartTestsAnalyzer
{
    public class CasesAnd
    {
        private CasesAnd()
        { }


        public CasesAnd( ExpressionSyntax parameterNameExpression, string parameterName, ExpressionSyntax caseExpression, IFieldSymbol criterion, bool hasError )
        {
            if( parameterName == null )
                parameterName = MemberTestCases.NoParameter;

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
            foreach( var aCase in other.Values )
            {
                Case parameterCase;
                if( !Cases.TryGetValue( aCase.ParameterName, out parameterCase ) )
                {
                    parameterCase = new Case( aCase.ParameterNameExpression, aCase.ParameterName );
                    Cases[ aCase.ParameterName ] = parameterCase;
                }
                parameterCase.FillWith( aCase );
            }
        }


        public void FillWithCriteriaTypes( Dictionary<string, HashSet<ITypeSymbol>> criteriaTypes )
        {
            foreach( var pair in Cases )
            {
                HashSet<ITypeSymbol> types;
                if( !criteriaTypes.TryGetValue( pair.Key, out types ) )
                {
                    types = new HashSet<ITypeSymbol>();
                    criteriaTypes[ pair.Key ] = types;
                }

                pair.Value.FillWithCriteriaTypes( types );
            }
        }


        public void FillWithExpressionSyntaxes( List<ExpressionSyntax> result )
        {
            foreach( var aCase in Cases.Values )
                aCase.FillWithExpressionSyntaxes( result );
        }


        public override bool Equals( object other )
        {
            var otherCriterias = other as CasesAnd;
            if( otherCriterias?.Cases.Count != Cases.Count )
                return false;

            foreach( var pair in otherCriterias.Cases )
            {
                Case parameterCase;
                if( !Cases.TryGetValue( pair.Key, out parameterCase ) )
                    // Not samne parameter name
                    return false;

                if( !Equals( pair.Value, parameterCase ) )
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
            foreach( var caseValue in Cases.Values )
            {
                caseValue.ToString( result );
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