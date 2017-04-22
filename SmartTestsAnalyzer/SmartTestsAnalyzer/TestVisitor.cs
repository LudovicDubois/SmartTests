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
            _Compilation = context.SemanticModel.Compilation;
            _TestingFrameworks = new TestingFrameworks( _Compilation );
            if( !IsTestProject )
                return;

            var smartTest = _Compilation.GetTypeByMetadataName( _SmartTestClassName );
            Debug.Assert( smartTest != null );
            _RunTestMethods = smartTest.GetMethods( "RunTest" );
            _CaseMethods = smartTest.GetMethods( "Case" );
        }


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
            SemanticModel model = null;
            var runTests = method.GetDescendantNodes<InvocationExpressionSyntax>().Where( m => _Compilation.HasMethod( m, _RunTestMethods ) );
            foreach( var runTest in runTests )
            {
                // Get Tested Member
                var argument1Syntax = runTest.GetArgument( 1 );
                if( argument1Syntax == null )
                    // ?!?!?
                    continue;

                if( model == null )
                {
                    model = _Compilation.GetSemanticModel( runTest.SyntaxTree );
                    Debug.Assert( model != null );
                }
                var testedMember = AnalyzeMember( model, argument1Syntax.Expression );
                if( testedMember == null )
                    // ?!?!?
                    continue;


                // Collect criterias
                var argument0Syntax = runTest.GetArgument( 0 );
                if( argument0Syntax == null )
                    // ?!?!?
                    continue;

                var memberTestCases = MembersTestCases.GetOrCreate( testedMember );
                AddCases( model, argument0Syntax, memberTestCases );
            }
        }


        private ISymbol AnalyzeMember( SemanticModel model, ExpressionSyntax expression )
        {
            var lambda = expression as ParenthesizedLambdaExpressionSyntax;
            return lambda != null
                       ? model.GetSymbol( lambda.Body )
                       : null;
        }


        private void AddCases( SemanticModel model, ArgumentSyntax argument0Syntax, MemberTestCases memberTestCases )
        {
            var arg0InvocationSyntax = argument0Syntax.Expression as InvocationExpressionSyntax;
            if( arg0InvocationSyntax == null )
                // ?!?!?
                return;
            var caseMethod = model.FindMethodSymbol( arg0InvocationSyntax, _CaseMethods );
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
                parameterName = model.GetConstantValue( arg0InvocationSyntax.GetArgument( 0 )?.Expression ).Value as string;
                criterias = arg0InvocationSyntax.GetArgument( 1 )?.Expression;
            }
            if( criterias == null )
                // ?!?!?
                return;

            var criteriasCollection = criterias.Accept( new CriteriaVisitor( model, criterias ) );
            memberTestCases.Add( parameterName ?? MemberTestCases.NoParameter, criteriasCollection );
        }
    }
}