using System;
using System.Collections.Generic;
using System.Diagnostics;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using SmartTests.Ranges;

using SmartTestsAnalyzer.Helpers;
using SmartTestsAnalyzer.Ranges;



namespace SmartTestsAnalyzer
{
    internal class RangeVisitor<T>: BaseVisitor, IRangeVisitor
        where T: struct, IComparable<T>
    {
        public RangeVisitor( SemanticModel model, ISymbolicNumericType<T> root, Type helperType, Action<Diagnostic> reportDiagnostic )
            : base( model, reportDiagnostic )
        {
            _Root = root;

            // INumericType<T> methods
            AddMethods( typeof(INumericType<>), false );
            if( helperType != null )
                AddMethods( helperType, true );
        }


        private readonly Dictionary<IMethodSymbol, Action<InvocationExpressionSyntax>> _RangeMethods =
            new Dictionary<IMethodSymbol, Action<InvocationExpressionSyntax>>();


        private ISymbolicNumericType<T> _Root;
        public IType Root => _Root;
        // ReSharper disable once MemberCanBePrivate.Global
        public bool IsError { get; set; }


        private void AddRangeExtension( ITypeSymbol smartTestType, string methodName,
                                        Action<InvocationExpressionSyntax> action )
        {
            var rangeMethods = smartTestType.GetMethods( methodName );
            foreach( var rangeMethod in rangeMethods )
            {
                Debug.Assert( rangeMethod != null, $"Problem with SmartTest.{methodName}<T> method" );
                _RangeMethods[ rangeMethod ] = action;
            }
        }


        private void AddRangeExtension( ITypeSymbol smartTestType, string methodName, int parameterCount,
                                        Action<InvocationExpressionSyntax> action )
        {
            var rangeMethods = smartTestType.GetMethods( methodName );
            foreach( var rangeMethod in rangeMethods )
            {
                Debug.Assert( rangeMethod != null, $"Problem with SmartTest.{methodName}<T> method" );
                if( ( rangeMethod.Parameters.Length == parameterCount && rangeMethod.Parameters[ parameterCount - 2 ].RefKind != RefKind.Out ) ||
                    // (min, max, out, avoided) we do not car about out and avoided values
                    ( rangeMethod.Parameters.Length == parameterCount + 2 && rangeMethod.Parameters[ parameterCount ].RefKind == RefKind.Out ) )
                    _RangeMethods[ rangeMethod ] = action;
            }
        }


        private void AddMethods( Type type, bool hasThisParameter )
        {
            var roslynType = Model.Compilation.GetTypeByMetadataName( type.FullName );
            var thisCount = hasThisParameter ? 1 : 0;

            // SmartTest type extension methods
            AddRangeExtension( roslynType, "Range", thisCount + 2,
                               node => Range( node, ( min, max ) => _Root.Range( min, max ) ) );
            AddRangeExtension( roslynType, "Range", thisCount + 4,
                               node => Range( node, ( min, minIncluded, max, maxIncluded ) => _Root.Range( min, minIncluded, max, maxIncluded ) ) );
            AddRangeExtension( roslynType, "AboveOrEqual",
                               node => Range( node, min => _Root.AboveOrEqual( min ) ) );
            AddRangeExtension( roslynType, "Above",
                               node => Range( node, min => _Root.Above( min ) ) );
            AddRangeExtension( roslynType, "BelowOrEqual",
                               node => Range( node, max => _Root.BelowOrEqual( max ) ) );
            AddRangeExtension( roslynType, "Below",
                               node => Range( node, max => _Root.Below( max ) ) );
            AddRangeExtension( roslynType, "Value",
                               node => Range( node, value => _Root.Value( value ) ) );
            AddRangeExtension( roslynType, "GetErrorValue",
                               node => IsError = true );
        }


        private void Range( InvocationExpressionSyntax node, Action<SymbolicConstant<T>, SymbolicConstant<T>> addRange )
        {
            if( TryGetSymbolicConstant( node.GetArgument( 0 ).Expression, out SymbolicConstant<T> min ) &
                TryGetSymbolicConstant( node.GetArgument( 1 ).Expression, out SymbolicConstant<T> max ) )
            {
                if( min.ConstantGreaterThan( max ) )
                    ReportDiagnostic( SmartTestsDiagnostics.CreateMinShouldBeLessThanMax( node, min.ToString(), max.ToString() ) );
                else if( _Root != null )
                    addRange( min, max );
            }
            else
                _Root = null;
        }


        private void Range( InvocationExpressionSyntax node, Action<SymbolicConstant<T>, bool, SymbolicConstant<T>, bool> addRange )
        {
            if( TryGetSymbolicConstant( node.GetArgument( 0 ).Expression, out SymbolicConstant<T> min ) &
                TryGetConstant( node.GetArgument( 1 ).Expression, out bool minIncluded ) &
                TryGetSymbolicConstant( node.GetArgument( 2 ).Expression, out SymbolicConstant<T> max ) &
                TryGetConstant( node.GetArgument( 3 ).Expression, out bool maxIncluded ) )
            {
                if( min.ConstantGreaterThan( max ) )
                    ReportDiagnostic( SmartTestsDiagnostics.CreateMinShouldBeLessThanMax( node, min.ToString(), max.ToString() ) );
                else if( _Root != null )
                    addRange( min, minIncluded, max, maxIncluded );
            }
            else
                _Root = null;
        }


        private void Range( InvocationExpressionSyntax node, Action<SymbolicConstant<T>> addRange )
        {
            if( TryGetSymbolicConstant( node.GetArgument( 0 ).Expression, out SymbolicConstant<T> value ) )
            {
                if( _Root != null )
                    addRange( value );
            }
            else
                _Root = null;
        }


        void IRangeVisitor.VisitInvocationExpression( InvocationExpressionSyntax node )
        {
            if( !( Model.GetSymbol( node ) is IMethodSymbol criteria ) )
                return;

            if( _RangeMethods.TryGetValue( criteria, out var func ) )
            {
                func( node );
                return;
            }

            if( criteria.ReducedFrom != null &&
                _RangeMethods.TryGetValue( criteria.ReducedFrom, out func ) )
            {
                func( node );
                return;
            }

            if( criteria.OriginalDefinition != null &&
                _RangeMethods.TryGetValue( criteria.OriginalDefinition, out func ) )
                func( node );
        }
    }
}