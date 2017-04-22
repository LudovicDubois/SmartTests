using System.Diagnostics;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

using SmartTestsAnalyzer.Helpers;



namespace SmartTestsAnalyzer
{
    class TestVisitor: FullSymbolVisitor
    {
        private const string _SmartTestClassName = "SmartTests.SmartTest";


        public TestVisitor( SemanticModelAnalysisContext context )
        {
            _Context = context;
            _Compilation = context.SemanticModel.Compilation;
            _TestingFrameworks = new TestingFrameworks( _Compilation );
            if( !IsTestProject )
                return;

            var smartTest = _Compilation.GetTypeByMetadataName( _SmartTestClassName );
            Debug.Assert( smartTest != null );
            _RunTestMethods = smartTest.GetMethods( "RunTest" );
            _CaseMethods = smartTest.GetMethods( "Case" );
        }


        private readonly SemanticModelAnalysisContext _Context;
        private readonly Compilation _Compilation;
        private readonly TestingFrameworks _TestingFrameworks;
        private readonly IMethodSymbol[] _RunTestMethods;
        private readonly IMethodSymbol[] _CaseMethods;


        public bool IsTestProject => _TestingFrameworks.IsTestProject;

        public MembersTestCases MembersTestCases { get; } = new MembersTestCases();


        public override void VisitNamedType( INamedTypeSymbol symbol )
        {
            if( _TestingFrameworks.IsTestClass( symbol ) )
                base.VisitNamedType( symbol );
        }


        public override void VisitMethod( IMethodSymbol method )
        {
            // Is it a test?
            if( method.MethodKind == MethodKind.Ordinary &&
                _TestingFrameworks.IsTestMethod( method ) )
                AnalyzeRunTest( method );
        }


        private void AnalyzeRunTest( IMethodSymbol method )
        {
            var methodDecl = method.DeclaringSyntaxReferences[ 0 ].GetSyntax( _Context.CancellationToken );
            var runTests = methodDecl.DescendantNodes().OfType<InvocationExpressionSyntax>().Where( m => _Compilation.HasMethod( m, _RunTestMethods ) );
            foreach( var runTest in runTests )
            {
                // Get Tested Member
                var argument1Syntax = runTest.GetArgument( 1 );
                if( argument1Syntax == null )
                    // ?!?!?
                    continue;
                var testedMember = AnalyzeMember( argument1Syntax.Expression );
                if( testedMember == null )
                    // ?!?!?
                    continue;


                // Collect criterias
                var argument0Syntax = runTest.GetArgument( 0 );
                if( argument0Syntax == null )
                    // ?!?!?
                    continue;

                var memberTestCases = MembersTestCases.GetOrCreate( testedMember );
                AddCases( argument0Syntax, memberTestCases );
            }
        }


        private ISymbol AnalyzeMember( ExpressionSyntax expression )
        {
            var lambda = expression as ParenthesizedLambdaExpressionSyntax;
            return lambda != null
                       ? _Compilation.GetSymbol( lambda.Body )
                       : null;
        }


        private void AddCases( ArgumentSyntax argument0Syntax, MemberTestCases memberTestCases )
        {
            var semanticModel = _Compilation.GetSemanticModel( argument0Syntax.SyntaxTree );
            var arg0InvocationSyntax = argument0Syntax.Expression as InvocationExpressionSyntax;
            if( arg0InvocationSyntax == null )
                // ?!?!?
                return;
            var caseMethod = semanticModel.FindMethodSymbol( arg0InvocationSyntax, _CaseMethods );
            if( caseMethod == null )
                // ?!?!?
                return;

            string parameterName;
            ExpressionSyntax criterias;
            if( caseMethod.Parameters.Length == 1 )
            {
                parameterName = null;
                criterias = arg0InvocationSyntax.GetArgument( 0 )?.Expression;
            }
            else
            {
                parameterName = semanticModel.GetConstantValue( arg0InvocationSyntax.GetArgument( 0 )?.Expression ).Value as string;
                criterias = arg0InvocationSyntax.GetArgument( 1 )?.Expression;
            }
            if( criterias == null )
                // ?!?!?
                return;

            var criteriasCollection = criterias.Accept( new CriteriaVisitor( semanticModel, criterias ) );
            memberTestCases.Add( parameterName ?? MemberTestCases.NoParameter, criteriasCollection );
        }
    }
}