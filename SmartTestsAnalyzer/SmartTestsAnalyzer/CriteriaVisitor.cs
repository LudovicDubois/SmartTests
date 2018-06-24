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
            // SmartTests.Range
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
            Debug.Assert( _RangeValueMethod.Parameters[ 2 ].RefKind == RefKind.Out, "Problem with SmartTest.Range(int, int, out int) methods" );

            // IntRange.Add
            var intRangeType = _RangeMethod.ReturnType;
            var addMethods = intRangeType.GetMethods( "Add" );
            Debug.Assert( addMethods.Length == 2, "Problem with IntRange.Add methods" );
            if( addMethods[ 0 ].Parameters.Length == 2 )
            {
                _AddMethod = addMethods[ 0 ];
                _AddValueMethod = addMethods[ 1 ];
            }
            else
            {
                _AddMethod = rangeMethods[ 1 ];
                _AddValueMethod = rangeMethods[ 0 ];
            }
            Debug.Assert( _AddMethod.Parameters.Length == 2, "Problem with IntRange.Add(int, int) methods" );
            Debug.Assert( _AddValueMethod.Parameters.Length == 3, "Problem with IntRange.Add(int, int, out int) methods" );
            Debug.Assert( _AddValueMethod.Parameters[ 2 ].RefKind == RefKind.Out, "Problem with IntRange.Add(int, int, out int) methods" );

            // GetValue
            _GetValueMethod = intRangeType.GetMethods( "GetValue" )[ 0 ];
            Debug.Assert( _GetValueMethod.Parameters.Length == 1, "Problem with IntRange.GetValue(out int) method" );
            Debug.Assert( _GetValueMethod.Parameters[ 0 ].RefKind == RefKind.Out, "Problem with IntRange.GetValue(out int) method" );

            _SpecialMethods = new HashSet<IMethodSymbol>
                              {
                                  _RangeMethod,
                                  _RangeValueMethod,
                                  _AddMethod,
                                  _AddValueMethod,
                                  _GetValueMethod
                              };
        }


        private readonly SemanticModel _Model;
        private readonly ExpressionSyntax _CasesExpression;
        private readonly ExpressionSyntax _ParameterNameExpression;
        private readonly Action<Diagnostic> _ReportDiagnostic;
        private readonly INamedTypeSymbol _ErrorAttribute;
        private readonly HashSet<IMethodSymbol> _SpecialMethods;
        private readonly IMethodSymbol _RangeMethod;
        private readonly IMethodSymbol _RangeValueMethod;
        private readonly IMethodSymbol _AddMethod;
        private readonly IMethodSymbol _AddValueMethod;
        private readonly IMethodSymbol _GetValueMethod;


        public override CasesAndOr VisitMemberAccessExpression( MemberAccessExpressionSyntax node )
        {
            var criteria = _Model.GetSymbolInfo( node ).Symbol as IFieldSymbol;
            if( criteria == null )
                return node.Expression.Accept( this );

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


        private IntRange _CurrentRange;


        public override CasesAndOr VisitInvocationExpression( InvocationExpressionSyntax node )
        {
            var criteria = _Model.GetSymbolInfo( node ).Symbol as IMethodSymbol;
            if( criteria == null ||
                !_SpecialMethods.Contains( criteria ) )
                return base.VisitInvocationExpression( node );

            if( criteria == _RangeValueMethod ||
                criteria == _RangeMethod )
            {
                var min = _Model.GetConstantValue( node.GetArgument( 0 ).Expression );
                if( !min.HasValue )
                    _ReportDiagnostic( SmartTestsDiagnostics.CreateNotAConstant( node.GetArgument( 0 ) ) );
                var max = _Model.GetConstantValue( node.GetArgument( 1 ).Expression );
                if( !max.HasValue )
                    _ReportDiagnostic( SmartTestsDiagnostics.CreateNotAConstant( node.GetArgument( 1 ) ) );
                if( !min.HasValue ||
                    !max.HasValue )
                    return base.VisitInvocationExpression( node );

                _CurrentRange = new IntRange( (int)min.Value, (int)max.Value );

                return new CasesAndOr( _CasesExpression, _ParameterNameExpression, Case.NoParameter, new RangeAnalysis( _CurrentRange ), false );
            }

            if( criteria == _AddValueMethod ||
                criteria == _AddMethod )
            {
                var result = node.Expression.Accept( this );

                var min = _Model.GetConstantValue( node.GetArgument( 0 ).Expression );
                if( !min.HasValue )
                    _ReportDiagnostic( SmartTestsDiagnostics.CreateNotAConstant( node.GetArgument( 0 ) ) );
                var max = _Model.GetConstantValue( node.GetArgument( 1 ).Expression );
                if( !max.HasValue )
                    _ReportDiagnostic( SmartTestsDiagnostics.CreateNotAConstant( node.GetArgument( 1 ) ) );
                if( !min.HasValue ||
                    !max.HasValue )
                    return base.VisitInvocationExpression( node );

                _CurrentRange?.Add( (int)min.Value, (int)max.Value );

                if( criteria == _AddValueMethod )
                    // last one
                    _CurrentRange = null;
                return result;
            }

            if( criteria == _GetValueMethod )
            {
                var result = node.Expression.Accept( this );
                _CurrentRange = null;
                return result;
            }

            throw new NotImplementedException();
        }
    }
}