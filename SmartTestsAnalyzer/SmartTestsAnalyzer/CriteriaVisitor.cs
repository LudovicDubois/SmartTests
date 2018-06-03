using System;
using System.Diagnostics;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using SmartTests;

using SmartTestsAnalyzer.Criterias;
using SmartTestsAnalyzer.Helpers;



namespace SmartTestsAnalyzer
{
    class CriteriaVisitor: CSharpSyntaxVisitor<CasesAndOr>
    {
        public CriteriaVisitor( SemanticModel model, ExpressionSyntax casesExpression, ExpressionSyntax parameterNameExpression )
        {
            _Model = model;
            _CasesExpression = casesExpression;
            _ParameterNameExpression = parameterNameExpression;
            _ErrorAttribute = _Model.Compilation.GetTypeByMetadataName( "SmartTests.ErrorAttribute" );
            Debug.Assert( _ErrorAttribute != null );
            var smartTestType = _Model.Compilation.GetTypeByMetadataName( "SmartTests.SmartTest" );
            var rangeMethods = smartTestType.GetMethods( "Range" );
            Debug.Assert( rangeMethods.Length == 2, "Problem with SmartTest.Range methods" );
            if( rangeMethods[ 0 ].Parameters.Length == 2 )
            {
                _RangeMethod = rangeMethods[ 0 ];
                _RangeValueMethod = rangeMethods[ 1 ];
            }
            else
            {
                _RangeMethod = rangeMethods[ 1 ];
                _RangeValueMethod = rangeMethods[ 0 ];
            }
            Debug.Assert( _RangeMethod.Parameters.Length == 2, "Problem with SmartTest.Range(int, int) methods" );
            Debug.Assert( _RangeValueMethod.Parameters.Length == 3, "Problem with SmartTest.Range(int, int, out int) methods" );
            Debug.Assert( _RangeValueMethod.Parameters[2].RefKind == RefKind.Out, "Problem with SmartTest.Range(int, int, out int) methods" );
        }


        private readonly SemanticModel _Model;
        private readonly ExpressionSyntax _CasesExpression;
        private readonly ExpressionSyntax _ParameterNameExpression;
        private readonly INamedTypeSymbol _ErrorAttribute;
        private readonly IMethodSymbol _RangeMethod;
        private readonly IMethodSymbol _RangeValueMethod;


        public override CasesAndOr VisitMemberAccessExpression( MemberAccessExpressionSyntax node )
        {
            var criteria = _Model.GetSymbolInfo( node ).Symbol as IFieldSymbol;
            if( criteria == null )
                return null;

            var parameterName = _ParameterNameExpression != null
                                    ? _Model.GetConstantValue( _ParameterNameExpression ).Value as string
                                    : null;
            return new CasesAndOr( _CasesExpression, _ParameterNameExpression, parameterName ?? Case.NoParameter, new FieldAnalysis( criteria ), criteria.HasAttribute( _ErrorAttribute ) );
        }


        public override CasesAndOr VisitParenthesizedExpression( ParenthesizedExpressionSyntax node )
        {
            return node.Expression.Accept( this );
        }


        public override CasesAndOr VisitBinaryExpression( BinaryExpressionSyntax node )
        {
            var leftCriteria = node.Left.Accept( this );
            if( leftCriteria == null )
                return null;
            var rightCriteria = node.Right.Accept( this );
            if( rightCriteria == null )
                return null;

            switch( node.OperatorToken.Kind() )
            {
                case SyntaxKind.AmpersandToken:
                    leftCriteria.CombineAnd( rightCriteria );
                    return leftCriteria;

                case SyntaxKind.BarToken:
                    leftCriteria.CombineOr( rightCriteria );
                    return leftCriteria;

                default:
                    return null;
            }
        }


        public override CasesAndOr VisitInvocationExpression( InvocationExpressionSyntax node )
        {
            var criteria = _Model.GetSymbolInfo( node ).Symbol as IMethodSymbol;
            if( criteria != _RangeValueMethod &&
                criteria != _RangeMethod )
                return base.VisitInvocationExpression( node );

            // Search for arguments
            var min = _Model.GetConstantValue( node.GetArgument( 0 ).Expression );
            //TODO: If no constant? Not an int?
            var max = _Model.GetConstantValue( node.GetArgument( 1 ).Expression );
            //TODO: If no constant? Not an int?

            var range = SmartTest.Range( (int)min.Value, (int)max.Value );

            return new CasesAndOr( _CasesExpression, _ParameterNameExpression, Case.NoParameter, new RangeAnalysis( range ), false );
        }
    }
}