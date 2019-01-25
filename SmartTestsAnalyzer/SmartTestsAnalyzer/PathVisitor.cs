using System.Text;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;



namespace SmartTestsAnalyzer
{
    class PathVisitor: CSharpSyntaxVisitor
    {
        public PathVisitor( string parameterName )
        {
            _ParameterName = parameterName;
        }


        private readonly string _ParameterName;

        private readonly StringBuilder _PathStr = new StringBuilder();
        public string PathStr => _PathStr.ToString();

        public bool HasErrors { get; private set; }


        public override void DefaultVisit( SyntaxNode node ) => HasErrors = true;


        public override void VisitIdentifierName( IdentifierNameSyntax node )
        {
            if( node.Identifier.Text != _ParameterName )
                HasErrors = true;
            _PathStr.Append( node.Identifier.Text );
        }


        public override void VisitParenthesizedExpression( ParenthesizedExpressionSyntax node ) => node.Expression.Accept( this );


        public override void VisitCastExpression( CastExpressionSyntax node ) => node.Expression.Accept( this );


        public override void VisitMemberAccessExpression( MemberAccessExpressionSyntax node )
        {
            node.Expression.Accept( this );
            _PathStr.Append( '.' );
            _PathStr.Append( node.Name.Identifier.Text );
        }
    }
}