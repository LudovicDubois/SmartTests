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

                var testedMember = CreateTestedMember( model, member, runTest.ArgumentList.Arguments[ 1 ].Expression );
                var memberTestCases = MembersTestCases.GetOrCreate( testedMember );
                if( runTestSymbol.Parameters[ 0 ].Type == _CaseType )
                    AddCases( model, argument0Syntax.Expression, memberTestCases );
                else
                    AddCase( model, argument0Syntax.Expression, null, null, argument0Syntax.Expression, memberTestCases );
            }
        }


        private TestedMember CreateTestedMember( SemanticModel model, ISymbol testedMember, ExpressionSyntax actExpression )
        {
            switch( testedMember.Kind )
            {
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


        private void AddCases( SemanticModel model, ExpressionSyntax expression, MemberTestCases memberTestCases )
        {
            var binaryExpression = expression as BinaryExpressionSyntax;
            if( binaryExpression != null )
            {
                AddCases( model, binaryExpression.Left, memberTestCases );
                AddCases( model, binaryExpression.Right, memberTestCases );
                return;
            }

            var argumentInvocation = (InvocationExpressionSyntax)expression;
            var caseMethod = model.FindMethodSymbol( argumentInvocation, _CaseMethods );
            if( caseMethod == null )
                // ?!?!?
                return;

            string parameterName;
            ExpressionSyntax criterias;
            ExpressionSyntax parameterNameExpression;
            if( caseMethod.Parameters.Length == 1 )
            {
                parameterNameExpression = null;
                parameterName = null;
                criterias = argumentInvocation.GetArgument( 0 )?.Expression;
            }
            else
            {
                parameterNameExpression = argumentInvocation.GetArgument( 0 )?.Expression;
                parameterName = model.GetConstantValue( parameterNameExpression ).Value as string;
                criterias = argumentInvocation.GetArgument( 1 )?.Expression;
            }
            if( criterias != null )
                AddCase( model, expression, parameterNameExpression, parameterName, criterias, memberTestCases );
        }


        private static void AddCase( SemanticModel model, ExpressionSyntax expression, ExpressionSyntax parameterNameExpression, string parameterName, ExpressionSyntax criterias, MemberTestCases memberTestCases )
        {
            var criteriasCollection = criterias.Accept( new CriteriaVisitor( model, expression, parameterNameExpression ) );
            memberTestCases.Add( parameterName ?? MemberTestCases.NoParameter, criteriasCollection );
        }


        public void Validate( Action<Diagnostic> reportDiagnostic ) => MembersTestCases.Validate( _ErrorType, reportDiagnostic );
    }
}