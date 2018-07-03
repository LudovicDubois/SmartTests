using System;
using System.Collections.Generic;
using System.Diagnostics;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using SmartTests.Ranges;

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

            var smartTestType = _Model.Compilation.GetTypeByMetadataName( "SmartTests.SmartTest" );

            // For IType<T> extension methods
            AddRangeExtension( smartTestType, "Range" );
            AddRangeExtension( smartTestType, "AboveOrEqual" );
            AddRangeExtension( smartTestType, "Above" );
            AddRangeExtension( smartTestType, "BelowOrEqual" );
            AddRangeExtension( smartTestType, "Below" );
            AddITypeTMethods();
        }


        private readonly SemanticModel _Model;
        private readonly ExpressionSyntax _CasesExpression;
        private readonly ExpressionSyntax _ParameterNameExpression;
        private readonly Action<Diagnostic> _ReportDiagnostic;
        private readonly INamedTypeSymbol _ErrorAttribute;
        private readonly HashSet<IMethodSymbol> _RangeMethods = new HashSet<IMethodSymbol>();


        private void AddRangeExtension( ITypeSymbol smartTestType, string methodName )
        {
            var rangeMethods = smartTestType.GetMethods( methodName );
            foreach( var rangeMethod in rangeMethods )
            {
                Debug.Assert( rangeMethod != null, $"Problem with SmartTest.{methodName}<T> method" );
                _RangeMethods.Add( rangeMethod );
            }
        }


        private void AddITypeTMethods()
        {
            var typeName = typeof(IType<>).FullName;
            var iTypeType = _Model.Compilation.GetTypeByMetadataName( typeName );
            var rangeMethod = iTypeType.GetMethods( "Range" )[ 0 ];
            Debug.Assert( rangeMethod.Parameters.Length == 2, $"Problem with {typeName}.Range(T, T) method" );
            _RangeMethods.Add( rangeMethod );

            // GetValue
            var getValueMethod = iTypeType.GetMethods( "GetValue" )[ 0 ];
            Debug.Assert( getValueMethod.Parameters.Length == 1, $"Problem with {typeName}.GetValue(out T) method" );
            Debug.Assert( getValueMethod.Parameters[ 0 ].RefKind == RefKind.Out, $"Problem with {typeName}.GetValue(out T) method" );
            _RangeMethods.Add( getValueMethod );
        }


        // Visit Methods


        public override CasesAndOr VisitMemberAccessExpression( MemberAccessExpressionSyntax node )
        {
            var member = _Model.GetSymbol( node );
            if( member is IFieldSymbol criteria )
            {
                var parameterName = _ParameterNameExpression != null
                                        ? _Model.GetConstantValue( _ParameterNameExpression ).Value as string
                                        : null;
                return new CasesAndOr( _CasesExpression, _ParameterNameExpression, parameterName ?? Case.NoParameter, new FieldAnalysis( criteria ), criteria.HasAttribute( _ErrorAttribute ) );
            }

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


        // Analysis Methods


        public override CasesAndOr VisitInvocationExpression( InvocationExpressionSyntax node )
        {
            var criteria = _Model.GetSymbol( node ) as IMethodSymbol;
            if( criteria == null )
                return base.VisitInvocationExpression( node );

            // For now, we do not have direct methods
            // if( _RangeMethods.Contains( criteria ) )
            //     return Analyze( node );

            if( criteria.ReducedFrom != null &&
                _RangeMethods.Contains( criteria.ReducedFrom ) )
                return Analyze( node );

            if( criteria.OriginalDefinition != null &&
                _RangeMethods.Contains( criteria.OriginalDefinition ) )
                return Analyze( node );

            return base.VisitInvocationExpression( node );
        }


        private CasesAndOr Analyze( InvocationExpressionSyntax node )
        {
            var typeSearchVisitor = new TypeSearchVisitor( _Model, _ReportDiagnostic, node );
            var iType = node.Accept( typeSearchVisitor );
            if( iType == null )
                // There was an error
                return null;
            return new CasesAndOr( _CasesExpression, _ParameterNameExpression, Case.NoParameter, new RangeAnalysis( iType ), false );
        }
    }
}