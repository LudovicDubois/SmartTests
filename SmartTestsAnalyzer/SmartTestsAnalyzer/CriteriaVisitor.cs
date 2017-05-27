using System.Diagnostics;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using SmartTestsAnalyzer.Helpers;



namespace SmartTestsAnalyzer
{
    class CriteriaVisitor: CSharpSyntaxVisitor<CasesAndOr>
    {
        public CriteriaVisitor( SemanticModel model, ExpressionSyntax casesExpression, ExpressionSyntax parameterNameExpression, string parameterName )
        {
            _Model = model;
            _CasesExpression = casesExpression;
            _ParameterNameExpression = parameterNameExpression;
            _ParameterName = parameterName;
            _ErrorAttribute = _Model.Compilation.GetTypeByMetadataName( "SmartTests.ErrorAttribute" );
            Debug.Assert( _ErrorAttribute != null );
        }


        private readonly SemanticModel _Model;
        private readonly ExpressionSyntax _CasesExpression;
        private readonly ExpressionSyntax _ParameterNameExpression;
        private readonly string _ParameterName;
        private readonly INamedTypeSymbol _ErrorAttribute;


        public override CasesAndOr VisitMemberAccessExpression( MemberAccessExpressionSyntax node )
        {
            var criteria = _Model.GetSymbolInfo( node ).Symbol as IFieldSymbol;
            return criteria != null
                       ? new CasesAndOr( _CasesExpression, _ParameterNameExpression, _ParameterName, criteria, criteria.HasAttribute( _ErrorAttribute ) )
                       : null;
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
    }
}