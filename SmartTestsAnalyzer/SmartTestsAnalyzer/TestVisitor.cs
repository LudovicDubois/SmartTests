using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

using SmartTestsAnalyzer.Criterias;
using SmartTestsAnalyzer.Helpers;



namespace SmartTestsAnalyzer
{
    partial class TestVisitor: FullSymbolVisitor
    {
        public TestVisitor( SemanticModelAnalysisContext context )
        {
            _Context = context;
            _Model = context.SemanticModel;
        }


        private readonly SemanticModelAnalysisContext _Context;
        private readonly SemanticModel _Model;


        public MembersTestCases MembersTestCases { get; } = new MembersTestCases();

        private const string _SmartTestClassName = "SmartTests.SmartTest";


        public override void VisitNamedType( INamedTypeSymbol symbol )
        {
            if( symbol.IsTestClass( _Model ) )
                base.VisitNamedType( symbol );
        }


        public override void VisitMethod( IMethodSymbol method )
        {
            // Is it a test?
            if( method.MethodKind == MethodKind.Ordinary &&
                method.IsTestMethod( _Model ) )
                AnalyzeRunTest( method );
        }


        private void AnalyzeRunTest( IMethodSymbol method )
        {
            var methodDecl = method.DeclaringSyntaxReferences[ 0 ].GetSyntax( _Context.CancellationToken );
            var invocations = methodDecl.DescendantNodes().OfType<InvocationExpressionSyntax>();
            var smartTest = _Model.Compilation.GetTypeByMetadataName( _SmartTestClassName );
            var runTestMethods = smartTest.GetMethods( "RunTest" );
            var caseMethods = smartTest.GetMethods( "Case" );
            foreach( var runTest in invocations )
            {
                if( !_Model.HasMethod( runTest, runTestMethods ) )
                    continue;

                // We have a call to SmartTest.RunTests method
                // => Collect Test Member & criterias
                var argument0Syntax = runTest.GetArgument( 0 );
                if( argument0Syntax == null )
                    continue;

                var memberTestCase = GetCases( argument0Syntax, caseMethods, runTest );
                if( memberTestCase == null )
                    continue;

                MembersTestCases.Add( memberTestCase );
            }
        }


        private MemberTestCase GetCases( ArgumentSyntax argument0Syntax, IMethodSymbol[] caseMethods, InvocationExpressionSyntax runTest )
        {
            var arg0InvocationSyntax = argument0Syntax.Expression as InvocationExpressionSyntax;
            if( !_Model.HasMethod( arg0InvocationSyntax, caseMethods ) )
                return null;

            var criteria = AnalyzeCriteria( arg0InvocationSyntax.GetArgument( 0 )?.Expression );
            if( criteria == null )
                return null;
            var member = AnalyzeMember( runTest.GetArgument( 1 ).Expression );
            if( member == null )
                return null;

            return new MemberTestCase( member, criteria );
        }


        private CriteriaSymbolExpression AnalyzeCriteria( ExpressionSyntax criterias ) => criterias?.Accept( new CriteriaVisitor( _Model ) );


        private ISymbol AnalyzeMember( ExpressionSyntax expression )
        {
            var lambda = expression as ParenthesizedLambdaExpressionSyntax;
            return lambda != null
                       ? _Model.GetSymbolInfo( lambda.Body ).Symbol
                       : null;
        }
    }
}