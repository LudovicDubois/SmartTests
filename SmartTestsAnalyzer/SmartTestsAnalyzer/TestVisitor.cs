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
            _ReportDiagnostic = context.ReportDiagnostic;
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
            _ErrorCaseMethods = smartTest.GetMethods( "ErrorCase" );
            _AssignMethods = smartTest.GetMethods( "Assign" );
        }


        private readonly Action<Diagnostic> _ReportDiagnostic;
        private readonly Compilation _Compilation;
        private readonly TestingFrameworks _TestingFrameworks;
        private readonly INamedTypeSymbol _CaseType;
        private readonly INamedTypeSymbol _ErrorType;
        private readonly IMethodSymbol[] _RunTestMethods;
        private readonly IMethodSymbol[] _CaseMethods;
        private readonly IMethodSymbol[] _ErrorCaseMethods;
        private readonly IMethodSymbol[] _AssignMethods;


        // ReSharper disable once InconsistentNaming
        // ReSharper disable once MemberCanBePrivate.Global
        public bool IsTestProject => _TestingFrameworks.IsTestProject;
        // ReSharper disable once InconsistentNaming
        public bool IsSmartTestProject { get; }

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

                var aCase = runTestSymbol.Parameters[ 0 ].Type.Equals( _CaseType )
                                ? GetCases( model, argument0Syntax.Expression, argument0Syntax.Expression )
                                : argument0Syntax.Expression.Accept( new CriteriaVisitor( model, argument0Syntax.Expression, null, _ReportDiagnostic ) );
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
            switch( expression )
            {
                case ParenthesizedLambdaExpressionSyntax lambda:
                    return model.GetSymbol( lambda.Body );

                case SimpleLambdaExpressionSyntax ctxLambda:
                    return model.GetSymbol( ctxLambda.Body );

                case InvocationExpressionSyntax invocation:
                {
                    // Can be Assign method
                    var invoked = model.FindMethodSymbol( invocation, _AssignMethods );
                    if( invoked != null )
                        return AnalyzeMember( model, invocation.ArgumentList.Arguments[ 0 ].Expression );
                    break;
                }
            }

            return null;
        }


        private CasesAndOr GetCases( SemanticModel model, ExpressionSyntax casesExpression, ExpressionSyntax caseExpression )
        {
            if( caseExpression is BinaryExpressionSyntax binaryExpression )
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
            var isError = false;
            if( caseMethod == null )
            {
                caseMethod = model.FindMethodSymbol( argumentInvocation, _ErrorCaseMethods );
                if( caseMethod != null ) // Logical, even if it does not do anything in case caseMethod == null
                    isError = true;
            }

            if( caseMethod == null )
                return null;

            ExpressionSyntax criteria;
            ExpressionSyntax parameterNameExpression;
            if( caseMethod.Parameters.Length == 1 )
            {
                parameterNameExpression = null;
                criteria = argumentInvocation.GetArgument( 0 )?.Expression;
            }
            else
            {
                parameterNameExpression = argumentInvocation.GetArgument( 0 )?.Expression;
                if( parameterNameExpression != null &&
                    caseMethod.Parameters[ 1 ].RefKind == RefKind.Out )
                {
                    var visitor = new TypeHelperVisitor( model, isError, _ReportDiagnostic );
                    return visitor.GetCase( parameterNameExpression );
                }

                criteria = argumentInvocation.GetArgument( 1 )?.Expression;
            }

            return criteria?.Accept( new CriteriaVisitor( model, casesExpression, parameterNameExpression, _ReportDiagnostic ) );
        }


        public void Validate( Action<Diagnostic> reportDiagnostic ) => MembersTestCases.Validate( _ErrorType, reportDiagnostic );
    }
}