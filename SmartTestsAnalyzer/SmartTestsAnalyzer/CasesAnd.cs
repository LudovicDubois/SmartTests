using System.Collections.Generic;
using System.Linq;
using System.Text;

#if !EXTENSION
using System.Diagnostics;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using SmartTestsAnalyzer.Criterias;
using SmartTestsAnalyzer.Helpers;

#endif

// ReSharper disable UnusedAutoPropertyAccessor.Global


namespace SmartTestsAnalyzer
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class CasesAnd
    {
#if EXTENSION
        // ReSharper disable once UnusedMember.Global
        public string TestNamespaceName { get; set; }
        public string TestClassName { get; set; }
        public string TestName { get; set; }
        public string TestFileName { get; set; }
        public int TestLine { get; set; }
        // ReSharper disable once CollectionNeverUpdated.Global
        public Dictionary<string, Case> Cases { get; } = new Dictionary<string, Case>();
        public bool HasError { get; set; }
        // ReSharper disable once InconsistentNaming
        public bool IsMissing { get; set; }

#else
        private CasesAnd()
        { }


        public CasesAnd( TestedParameter testedParameter, ExpressionSyntax caseExpression, CriteriaAnalysis criterion, bool hasError )
        {
            Debug.Assert( testedParameter?.Name != null );

            if( !Cases.TryGetValue( testedParameter, out Case currentCase ) )
            {
                currentCase = new Case( testedParameter );
                Cases[ testedParameter ] = currentCase;
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


        // ReSharper disable MemberCanBePrivate.Global
        public string TestNamespaceName { get; private set; }
        public string TestClassName { get; private set; }
        public string TestName { get; private set; }
        public string TestFileName { get; private set; }
        public int TestLine { get; private set; }
        public Dictionary<TestedParameter, Case> Cases { get; } = new Dictionary<TestedParameter, Case>();
        public bool HasError { get; private set; }
        // ReSharper disable once InconsistentNaming
        public bool IsMissing { get; set; }
        // ReSharper restore MemberCanBePrivate.Global


        public CasesAnd CombineAnd( CasesAnd otherCases )
        {
            var result = new CasesAnd
                         {
                             TestNamespaceName = otherCases.TestNamespaceName,
                             TestClassName = otherCases.TestClassName,
                             TestName = otherCases.TestName,
                             TestFileName = otherCases.TestFileName,
                             TestLine = otherCases.TestLine
                         };
            result.FillCasesWith( Cases );
            result.FillCasesWith( otherCases.Cases );
            result.HasError = HasError || otherCases.HasError;
            return result;
        }


        private void FillCasesWith( Dictionary<TestedParameter, Case> other )
        {
            foreach( var otherCase in other )
            {
                if( !Cases.TryGetValue( otherCase.Key, out Case parameterCase ) )
                {
                    parameterCase = new Case( otherCase.Key );
                    Cases[ otherCase.Key ] = parameterCase;
                }

                parameterCase.FillWith( otherCase.Value );
            }
        }


        public void FillCriteriaValues( Dictionary<TestedParameter, Dictionary<TestedParameter, CriteriaValues>> criteriaTypes, INamedTypeSymbol errorType )
        {
            foreach( var pair in Cases )
            {
                if( !criteriaTypes.TryGetValue( pair.Key, out var values ) )
                {
                    values = new Dictionary<TestedParameter, CriteriaValues>();
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