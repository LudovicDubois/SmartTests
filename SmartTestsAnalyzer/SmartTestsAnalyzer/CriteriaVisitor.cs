using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using SmartTestsAnalyzer.Criterias;



namespace SmartTestsAnalyzer
{
    class CriteriaVisitor: CSharpSyntaxVisitor<CriteriaSymbolExpression>
    {
        public CriteriaVisitor( SemanticModel model )
        {
            _Model = model;
        }


        private readonly SemanticModel _Model;


        public override CriteriaSymbolExpression VisitMemberAccessExpression( MemberAccessExpressionSyntax node )
        {
            var criteria = _Model.GetSymbolInfo( node ).Symbol;
            return criteria != null
                       ? new AtomicCriteria( criteria )
                       : null;
        }


        public override CriteriaSymbolExpression VisitBinaryExpression( BinaryExpressionSyntax node )
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
                    return new AndCriteria( leftCriteria, rightCriteria );

                default:
                    return null;
            }
        }
    }
}