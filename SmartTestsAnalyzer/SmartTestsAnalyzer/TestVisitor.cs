using System;
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
                // This is not a Testing Project
                return;

            var smartTest = _Compilation.GetTypeByMetadataName( _SmartTestClassName );
            if( smartTest == null )
                // This is not a SmartTest Project!
                return;

            IsSmartTestProject = IsTestProject;
            _CaseType = _Compilation.GetTypeByMetadataName( "SmartTests.Case" );
            Debug.Assert( _CaseType != null );
            _ErrorType = _Compilation.GetTypeByMetadataName( "SmartTests.ErrorAttribute" );
            Debug.Assert( _ErrorType != null );
            _RunTestMethods = smartTest.GetMethods( "RunTest" );
            _CaseMethods = smartTest.GetMethods( "Case" );
            _AssignMethods = smartTest.GetMethods( "Assign" );
        }


        private readonly Compilation _Compilation;
        private readonly TestingFrameworks _TestingFrameworks;
        private readonly INamedTypeSymbol _CaseType;
        private readonly INamedTypeSymbol _ErrorType;
        private readonly IMethodSymbol[] _RunTestMethods;
        private readonly IMethodSymbol[] _CaseMethods;
        private readonly IMethodSymbol[] _AssignMethods;


        public bool IsTestProject => _TestingFrameworks.IsTestProject;
        public bool IsSmartTestProject { get; private set; }

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
                var member = AnalyzeMember( model, argument1Syntax.Expression );
                if( member == null )
                    // ?!?!?
                    continue;


                // Collect criterias
                var argument0Syntax = runTest.GetArgument( 0 );
                if( argument0Syntax == null )
                    // ?!?!?
                    continue;

                var runTestSymbol = model.GetSymbol( runTest ) as IMethodSymbol;
                if( runTestSymbol == null )
                    // ?!?!?
                    return;

                var aCase = runTestSymbol.Parameters[ 0 ].Type == _CaseType
                                ? GetCases( model, argument0Syntax.Expression, argument0Syntax.Expression )
                                : argument0Syntax.Expression.Accept( new CriteriaVisitor( model, argument0Syntax.Expression, null ) );
                if( aCase == null )
                    // ?!?
                    continue;

                var testedMember = CreateTestedMember( model, member, runTest.ArgumentList.Arguments[ 1 ].Expression );
                var memberTestCases = MembersTestCases.GetOrCreate( testedMember );
                memberTestCases.CombineOr( aCase );
            }
        }


        private TestedMember CreateTestedMember( SemanticModel model, ISymbol testedMember, ExpressionSyntax actExpression )
        {
            switch( testedMember.Kind )
            {
                case SymbolKind.Field:
                    return new TestedMember( testedMember, TestedMemberKind.Field );

                case SymbolKind.Method:
                    return new TestedMember( testedMember, TestedMemberKind.Method );

                case SymbolKind.Property:
                    var isIndexer = ( (IPropertySymbol)testedMember ).IsIndexer;
                    var isAssignment = model.FindMethodSymbol( actExpression, _AssignMethods ) != null;
                    return new TestedMember( testedMember,
                                             isIndexer
                                                 ? isAssignment
                                                       ? TestedMemberKind.IndexerSet
                                                       : TestedMemberKind.IndexerGet
                                                 : isAssignment
                                                     ? TestedMemberKind.PropertySet
                                                     : TestedMemberKind.PropertyGet );
                default:
                    throw new NotImplementedException();
            }
        }


        private ISymbol AnalyzeMember( SemanticModel model, ExpressionSyntax expression )
        {
            var lambda = expression as ParenthesizedLambdaExpressionSyntax;
            if( lambda != null )
                return model.GetSymbol( lambda.Body );

            var ctxLambda = expression as SimpleLambdaExpressionSyntax;
            if( ctxLambda != null )
                return model.GetSymbol( ctxLambda.Body );

            var invocation = expression as InvocationExpressionSyntax;
            if( invocation != null )
            {
                // Can be Assign method
                var invoked = model.FindMethodSymbol( invocation, _AssignMethods );
                if( invoked != null )
                    return AnalyzeMember( model, invocation.ArgumentList.Arguments[ 0 ].Expression );
            }

            return null;
        }


        private CasesAndOr GetCases( SemanticModel model, ExpressionSyntax casesExpression, ExpressionSyntax caseExpression )
        {
            var binaryExpression = caseExpression as BinaryExpressionSyntax;
            if( binaryExpression != null )
            {
                var left = GetCases( model, casesExpression, binaryExpression.Left );
                if( left == null )
                    return null;
                var right = GetCases( model, casesExpression, binaryExpression.Right );
                if( right == null )
                    return null;
                left.CombineAnd( right );
                return left;
            }

            var argumentInvocation = (InvocationExpressionSyntax)caseExpression;
            var caseMethod = model.FindMethodSymbol( argumentInvocation, _CaseMethods );
            if( caseMethod == null )
                return null;

            ExpressionSyntax criterias;
            ExpressionSyntax parameterNameExpression;
            if( caseMethod.Parameters.Length == 1 )
            {
                parameterNameExpression = null;
                criterias = argumentInvocation.GetArgument( 0 )?.Expression;
            }
            else
            {
                parameterNameExpression = argumentInvocation.GetArgument( 0 )?.Expression;
                criterias = argumentInvocation.GetArgument( 1 )?.Expression;
            }

            return criterias?.Accept( new CriteriaVisitor( model, casesExpression, parameterNameExpression ) );
        }


        public void Validate( Action<Diagnostic> reportDiagnostic ) => MembersTestCases.Validate( _ErrorType, reportDiagnostic );
    }
}