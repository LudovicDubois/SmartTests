using System.Diagnostics;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using SmartTestsAnalyzer.Helpers;



namespace SmartTestsAnalyzer
{
    class CriteriaVisitor: CSharpSyntaxVisitor<CombinedCriteriasCollection>
    {
        public CriteriaVisitor( SemanticModel model, ExpressionSyntax criterias )
        {
            _Model = model;
            _Criterias = criterias;
            _ErrorAttribute = _Model.Compilation.GetTypeByMetadataName( "SmartTests.ErrorAttribute" );
            Debug.Assert( _ErrorAttribute != null );
        }


        private readonly SemanticModel _Model;
        private readonly ExpressionSyntax _Criterias;
        private readonly INamedTypeSymbol _ErrorAttribute;


        public override CombinedCriteriasCollection VisitMemberAccessExpression( MemberAccessExpressionSyntax node )
        {
            var criteria = _Model.GetSymbolInfo( node ).Symbol as IFieldSymbol;
            return criteria != null
                       ? new CombinedCriteriasCollection( _Criterias, criteria, criteria.HasAttribute( _ErrorAttribute ) )
                       : null;
        }


        public override CombinedCriteriasCollection VisitBinaryExpression( BinaryExpressionSyntax node )
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

                default:
                    return null;
            }
        }
    }
}