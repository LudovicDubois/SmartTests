using System;
using System.Diagnostics;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using SmartTestsAnalyzer.Criterias;
using SmartTestsAnalyzer.Helpers;



namespace SmartTestsAnalyzer
{
    class CriteriaVisitor: CSharpSyntaxVisitor<CasesAndOr>
    {
        public CriteriaVisitor( SemanticModel model, ExpressionSyntax casesExpression, ExpressionSyntax parameterNameExpression, Action<Diagnostic> reportDiagnostic )
        {
            _Model = model;
            _CasesExpression = casesExpression;
            _ParameterNameExpression = parameterNameExpression;
            _ReportDiagnostic = reportDiagnostic;
            _ErrorAttribute = _Model.Compilation.GetTypeByMetadataName( "SmartTests.ErrorAttribute" );
            Debug.Assert( _ErrorAttribute != null );

            _RangeVisitor = new RangeVisitor( model, reportDiagnostic );
        }


        private readonly SemanticModel _Model;
        private readonly ExpressionSyntax _CasesExpression;
        private readonly ExpressionSyntax _ParameterNameExpression;
        private readonly Action<Diagnostic> _ReportDiagnostic;
        private readonly INamedTypeSymbol _ErrorAttribute;
        private readonly RangeVisitor _RangeVisitor;


        // Visit Methods


        public override CasesAndOr VisitMemberAccessExpression( MemberAccessExpressionSyntax node )
        {
            var member = _Model.GetSymbol( node );
            if( member is IFieldSymbol criteria )
                return new CasesAndOr( _CasesExpression,
                                       _ParameterNameExpression, GetParameterNameAndType( out var type ), type,
                                       new FieldAnalysis( criteria ), criteria.HasAttribute( _ErrorAttribute ) );

            return node.Expression.Accept( this );
        }


        public override CasesAndOr VisitParenthesizedExpression( ParenthesizedExpressionSyntax node ) => node.Expression.Accept( this );


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
            var visitor = node.Accept( _RangeVisitor );
            if( visitor?.Root != null )
                return new CasesAndOr( _CasesExpression,
                                       _ParameterNameExpression, GetParameterNameAndType( out var type ), type,
                                       new RangeAnalysis( visitor.Root, visitor.IsError ), visitor.IsError );

            return base.VisitInvocationExpression( node );
        }


        private string GetParameterNameAndType( out ITypeSymbol type )
        {
            type = null;
            if( _ParameterNameExpression == null )
                return Case.NoParameter;

            if( _Model.GetConstantValue( _ParameterNameExpression ).Value is string result )
                return result;

            // Lambda?
            if( _ParameterNameExpression is ParenthesizedLambdaExpressionSyntax lambda )
            {
                var parameter = lambda.ParameterList.Parameters[ 0 ];
                type = (ITypeSymbol)_Model.GetSymbol( parameter.Type );
                return parameter.Identifier.Text;
            }

            return Case.NoParameter;
        }


        // Helper Methods
    }
}