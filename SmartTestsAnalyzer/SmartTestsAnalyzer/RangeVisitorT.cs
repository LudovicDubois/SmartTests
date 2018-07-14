using System;
using System.Collections.Generic;
using System.Diagnostics;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using SmartTests.Ranges;

using SmartTestsAnalyzer.Helpers;



namespace SmartTestsAnalyzer
{
    interface IRangeVisitor
    {
        IType Root { get; }
        bool IsError { get; }

        void VisitInvocationExpression( InvocationExpressionSyntax node );
    }


    class RangeVisitor<T>: IRangeVisitor
        where T: struct, IComparable<T>
    {
        public RangeVisitor( SemanticModel model, INumericType<T> root, Action<Diagnostic> reportDiagnostic )
        {
            _Model = model;
            _Root = root;
            _ReportDiagnostic = reportDiagnostic;

            // INumericType<T> methods
            AddITypeTMethods();
        }


        private readonly SemanticModel _Model;
        private readonly Action<Diagnostic> _ReportDiagnostic;

        private readonly Dictionary<IMethodSymbol, Action<InvocationExpressionSyntax>> _RangeMethods =
            new Dictionary<IMethodSymbol, Action<InvocationExpressionSyntax>>();


        private INumericType<T> _Root;
        public IType Root => _Root;
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


        private void AddITypeTMethods()
        {
            var typeName = typeof(INumericType<>).FullName;
            var iTypeType = _Model.Compilation.GetTypeByMetadataName( typeName );

            // SmartTest type extension methods
            AddRangeExtension( iTypeType, "Range",
                               node => Range( node, ( min, max ) => _Root.Range( min, max ) ) );
            AddRangeExtension( iTypeType, "AboveOrEqual",
                               node => Range( node, min => _Root.AboveOrEqual( min ) ) );
            AddRangeExtension( iTypeType, "Above",
                               node => Range( node, min => _Root.Above( min ) ) );
            AddRangeExtension( iTypeType, "BelowOrEqual",
                               node => Range( node, max => _Root.BelowOrEqual( max ) ) );
            AddRangeExtension( iTypeType, "Below",
                               node => Range( node, max => _Root.Below( max ) ) );
            AddRangeExtension( iTypeType, "GetErrorValue",
                              node => IsError = true );
        }


        private bool TryGetConstant( ExpressionSyntax expression, out T value )
        {
            var constant = _Model.GetConstantValue( expression );
            if( !constant.HasValue )
            {
                _ReportDiagnostic( SmartTestsDiagnostics.CreateNotAConstant( expression ) );
                value = default(T);
                return false;
            }

            value = (T)Convert.ChangeType( constant.Value, typeof(T) );
            return true;
        }


        private void Range( InvocationExpressionSyntax node, Action<T, T> addRange )
        {
            if( TryGetConstant( node.GetArgument( 0 ).Expression, out T min ) &
                TryGetConstant( node.GetArgument( 1 ).Expression, out T max ) )
            {
                if( _Root != null )
                    addRange( min, max );
            }
            else
                _Root = null;
        }


        private void Range( InvocationExpressionSyntax node, Action<T> addRange )
        {
            if( TryGetConstant( node.GetArgument( 0 ).Expression, out T value ) )
            {
                if( _Root != null )
                    addRange( value );
            }
            else
                _Root = null;
        }


        void IRangeVisitor.VisitInvocationExpression( InvocationExpressionSyntax node )
        {
            if( !( _Model.GetSymbol( node ) is IMethodSymbol criteria ) )
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