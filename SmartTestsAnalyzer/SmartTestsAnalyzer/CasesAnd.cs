using System.Collections.Generic;
using System.Linq;
using System.Text;

using JetBrains.Annotations;

#if !EXTENSION
using System.Diagnostics;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using SmartTestsAnalyzer.Helpers;
using SmartTestsAnalyzer.Criterias;

#endif



namespace SmartTestsAnalyzer
{
    [UsedImplicitly( ImplicitUseTargetFlags.WithMembers )]
    public class CasesAnd
    {
#if EXTENSION

        public string TestNamespaceName { get; set; }
        public string TestClassName { get; set; }
        public string TestName { get; set; }
        public string TestFileName { get; set; }
        public int TestLine { get; set; }
        public Dictionary<string, Case> Cases { get; } = new Dictionary<string, Case>();
        public bool HasError { get; set; }
        public bool IsMissing { get; set; }

#else


        private CasesAnd()
        { }


        public CasesAnd( ExpressionSyntax parameterNameExpression, string parameterName, ExpressionSyntax caseExpression, CriteriaAnalysis criterion, bool hasError )
        {
            Debug.Assert( parameterName != null );

            if( !Cases.TryGetValue( parameterName, out Case currentCase ) )
            {
                currentCase = new Case( parameterNameExpression, parameterName );
                Cases[ parameterName ] = currentCase;
            }
            currentCase.Add( caseExpression, criterion, hasError );
            if( currentCase.HasError )
                HasError = true;

            if( caseExpression == null )
                // other cases
                return;

            var testMethod = caseExpression.FirstAncestorOrSelf<MethodDeclarationSyntax>();
            var testClass = testMethod.FirstAncestorOrSelf<ClassDeclarationSyntax>();
            var testNamespace = testClass.FirstAncestorOrSelf<NamespaceDeclarationSyntax>();
            TestNamespaceName = testNamespace.GetFullName();
            TestClassName = testClass.GetFullName( false );
            TestName = testMethod.Identifier.Text;
            var span = caseExpression.GetLocation().GetLineSpan();
            TestFileName = span.Path;
            TestLine = span.StartLinePosition.Line + 1;
        }


        public string TestNamespaceName { get; private set; }
        public string TestClassName { get; private set; }
        public string TestName { get; private set; }
        public string TestFileName { get; private set; }
        public int TestLine { get; private set; }
        public Dictionary<string, Case> Cases { get; } = new Dictionary<string, Case>();
        public bool HasError { get; private set; }
        public bool IsMissing { get; set; }


        public CasesAnd CombineAnd( CasesAnd otherCases )
        {
            var result = new CasesAnd();
            result.TestNamespaceName = otherCases.TestNamespaceName;
            result.TestClassName = otherCases.TestClassName;
            result.TestName = otherCases.TestName;
            result.TestFileName = otherCases.TestFileName;
            result.TestLine = otherCases.TestLine;
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


        public void FillCriteriaValues( Dictionary<string, Dictionary<string,CriteriaValues>> criteriaTypes, INamedTypeSymbol errorType )
        {
            foreach( var pair in Cases )
            {
                if( !criteriaTypes.TryGetValue( pair.Key, out var values ) )
                {
                    values = new Dictionary<string, CriteriaValues>();
                    criteriaTypes[ pair.Key ] = values;
                }

                pair.Value.FillCriteriaValues( values, errorType );
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
                if( !Cases.TryGetValue( otherCase.Key, out Case parameterCase ) )
                    // Not same parameter name!
                    return false;

                if( !Equals( otherCase.Value, parameterCase ) )
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