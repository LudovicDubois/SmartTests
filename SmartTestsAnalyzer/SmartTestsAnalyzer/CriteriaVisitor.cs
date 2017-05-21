using System.Diagnostics;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using SmartTestsAnalyzer.Helpers;



namespace SmartTestsAnalyzer
{
    class CriteriaVisitor: CSharpSyntaxVisitor<CriteriasAndOr>
    {
        public CriteriaVisitor( SemanticModel model, ExpressionSyntax casesExpression, ExpressionSyntax parameterNameExpression )
        {
            _Model = model;
            _CasesExpression = casesExpression;
            _ParameterNameExpression = parameterNameExpression;
            _ErrorAttribute = _Model.Compilation.GetTypeByMetadataName( "SmartTests.ErrorAttribute" );
            Debug.Assert( _ErrorAttribute != null );
        }


        private readonly SemanticModel _Model;
        private readonly ExpressionSyntax _CasesExpression;
        private readonly ExpressionSyntax _ParameterNameExpression;
        private readonly INamedTypeSymbol _ErrorAttribute;


        public override CriteriasAndOr VisitMemberAccessExpression( MemberAccessExpressionSyntax node )
        {
            var criteria = _Model.GetSymbolInfo( node ).Symbol as IFieldSymbol;
            return criteria != null
                       ? new CriteriasAndOr( _CasesExpression, _ParameterNameExpression, criteria, criteria.HasAttribute( _ErrorAttribute ) )
                       : null;
        }


        public override CriteriasAndOr VisitParenthesizedExpression( ParenthesizedExpressionSyntax node )
        {
            return node.Expression.Accept( this );
        }


        public override CriteriasAndOr VisitBinaryExpression( BinaryExpressionSyntax node )
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
    }
}