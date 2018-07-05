using System;
using System.Collections.Generic;
using System.Diagnostics;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using SmartTests.Ranges;

using SmartTestsAnalyzer.Helpers;



namespace SmartTestsAnalyzer
{
    class RangeVisitor<T>: CSharpSyntaxVisitor<INumericType<T>>
        where T: struct, IComparable<T>
    {
        public RangeVisitor( SemanticModel model, INumericType<T> root, Action<Diagnostic> reportDiagnostic )
        {
            _Model = model;
            _Type = root;
            _ReportDiagnostic = reportDiagnostic;

            // INumericType<T> methods
            AddITypeTMethods();
        }


        private readonly SemanticModel _Model;
        private readonly INumericType<T> _Type;
        private readonly Action<Diagnostic> _ReportDiagnostic;

        private readonly Dictionary<IMethodSymbol, Func<InvocationExpressionSyntax, INumericType<T>>> _RangeMethods =
            new Dictionary<IMethodSymbol, Func<InvocationExpressionSyntax, INumericType<T>>>();


        private void AddRangeExtension( ITypeSymbol smartTestType, string methodName,
                                        Func<InvocationExpressionSyntax, INumericType<T>> func )
        {
            var rangeMethods = smartTestType.GetMethods( methodName );
            foreach( var rangeMethod in rangeMethods )
            {
                Debug.Assert( rangeMethod != null, $"Problem with SmartTest.{methodName}<T> method" );
                _RangeMethods[ rangeMethod ] = func;
            }
        }


        private void AddITypeTMethods()
        {
            var typeName = typeof(INumericType<>).FullName;
            var iTypeType = _Model.Compilation.GetTypeByMetadataName( typeName );

            // SmartTest type extension methods
            AddRangeExtension( iTypeType, "Range",
                               node => Range( node, ( iType, min, max ) => iType.Range( min, max ) ) );
            AddRangeExtension( iTypeType, "AboveOrEqual",
                               node => Range( node, ( iType, min ) => iType.AboveOrEqual( min ) ) );
            AddRangeExtension( iTypeType, "Above",
                               node => Range( node, ( iType, min ) => iType.Above( min ) ) );
            AddRangeExtension( iTypeType, "BelowOrEqual",
                               node => Range( node, ( iType, max ) => iType.BelowOrEqual( max ) ) );
            AddRangeExtension( iTypeType, "Below",
                               node => Range( node, ( iType, max ) => iType.Below( max ) ) );

            // GetValue
            var getValueMethod = iTypeType.GetMethods( "GetValue" )[ 0 ];
            Debug.Assert( getValueMethod.Parameters.Length == 1, $"Problem with {typeName}.GetValue(out int) method" );
            Debug.Assert( getValueMethod.Parameters[ 0 ].RefKind == RefKind.Out,
                          $"Problem with {typeName}.GetValue(out int) method" );
            _RangeMethods[ getValueMethod ] = GetValue;
        }


        // Visit Methods
        private INumericType<T> GetRoot( SyntaxNode node ) => _Model.GetSymbol( node ) is IPropertySymbol ? _Type : null;


        public override INumericType<T> VisitIdentifierName( IdentifierNameSyntax node ) => GetRoot( node ) ?? base.VisitIdentifierName( node );


        public override INumericType<T> VisitMemberAccessExpression( MemberAccessExpressionSyntax node ) => GetRoot( node ) ?? node.Expression.Accept( this );


        public override INumericType<T> VisitParenthesizedExpression( ParenthesizedExpressionSyntax node ) => node.Expression.Accept( this );


        // Analysis Methods


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


        private INumericType<T> Range( InvocationExpressionSyntax node, Action<INumericType<T>, T, T> addRange )
        {
            var result = node.Expression.Accept( this );

            if( !( TryGetConstant( node.GetArgument( 0 ).Expression, out T min ) &
                   TryGetConstant( node.GetArgument( 1 ).Expression, out T max ) ) ||
                result == null )
                return null;

            addRange( result, min, max );
            return result;
        }


        private INumericType<T> Range( InvocationExpressionSyntax node, Action<INumericType<T>, T> addRange )
        {
            var result = node.Expression.Accept( this );

            if( !TryGetConstant( node.GetArgument( 0 ).Expression, out T value ) ||
                result == null )
                return null;

            addRange( result, value );
            return result;
        }


        private INumericType<T> Range( InvocationExpressionSyntax node )
        {
            var result = node.Expression.Accept( this );

            if( !( TryGetConstant( node.GetArgument( 0 ).Expression, out T min ) &
                   TryGetConstant( node.GetArgument( 1 ).Expression, out T max ) ) ||
                result == null )
                return null;

            result.Range( min, max );
            return result;
        }


        private INumericType<T> GetValue( InvocationExpressionSyntax node ) => node.Expression.Accept( this );


        public override INumericType<T> VisitInvocationExpression( InvocationExpressionSyntax node )
        {
            var criteria = _Model.GetSymbol( node ) as IMethodSymbol;
            if( criteria == null )
                return base.VisitInvocationExpression( node );


            if( _RangeMethods.TryGetValue( criteria, out var func ) )
                return func( node );

            if( criteria.ReducedFrom != null &&
                _RangeMethods.TryGetValue( criteria.ReducedFrom, out func ) )
                return func( node );

            if( criteria.OriginalDefinition != null &&
                _RangeMethods.TryGetValue( criteria.OriginalDefinition, out func ) )
                return func( node );

            return base.VisitInvocationExpression( node );
        }
    }
}