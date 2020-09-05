using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;



namespace SmartTestsAnalyzer
{
    abstract class BaseVisitor
    {
        protected BaseVisitor( SemanticModel model, Action<Diagnostic> reportDiagnostic )
        {
            Model = model;
            ReportDiagnostic = reportDiagnostic;
            var iNumericType = Model.Compilation.GetTypeByMetadataName( "SmartTests.Ranges.INumericType`1" );
            _DateTimeConstants[ iNumericType.GetMembers( "MinValue" ).First() ] = DateTime.MinValue;
            _DateTimeConstants[ iNumericType.GetMembers( "MaxValue" ).First() ] = DateTime.MaxValue;
            _DateTimeType = Model.Compilation.GetTypeByMetadataName( "System.DateTime" );
            _DateTimeConstants[ _DateTimeType.GetMembers( "MinValue" ).First() ] = DateTime.MinValue;
            _DateTimeConstants[ _DateTimeType.GetMembers( "MaxValue" ).First() ] = DateTime.MaxValue;
            // ReSharper disable once PossibleInvalidCastExceptionInForeachLoop
            foreach( IMethodSymbol ctor in _DateTimeType.GetMembers( ".ctor" ) )
                _DateTimeConstructions.Add( ctor );
        }


        protected SemanticModel Model { get; }
        protected Action<Diagnostic> ReportDiagnostic { get; }
        private readonly INamedTypeSymbol _DateTimeType;
        private readonly Dictionary<ISymbol, DateTime> _DateTimeConstants = new Dictionary<ISymbol, DateTime>();
        private readonly HashSet<ISymbol> _DateTimeConstructions = new HashSet<ISymbol>();


        protected bool TryGetConstant<T>( ExpressionSyntax expression, out T value )
        {
            var constant = Model.GetConstantValue( expression );
            if( !constant.HasValue )
            {
                var symbol = Model.GetSymbolInfo( expression ).Symbol;
                if( _DateTimeConstants.TryGetValue( symbol, out var dt ) ||
                    _DateTimeConstants.TryGetValue( symbol.OriginalDefinition, out dt ) )
                {
                    value = (T)(object)dt;
                    return true;
                }

                if( _DateTimeConstructions.Contains( symbol ) )
                    return TryCreateDateTime( (ObjectCreationExpressionSyntax)expression, out value );

                if( Equals( ( symbol as ILocalSymbol )?.Type ??
                            ( symbol as IPropertySymbol )?.Type ??
                            ( symbol as IMethodSymbol )?.ReturnType,
                            _DateTimeType ) )
                    ReportDiagnostic( SmartTestsDiagnostics.CreateNotADateCreation( expression ) );
                else
                    ReportDiagnostic( SmartTestsDiagnostics.CreateNotAConstantPropertyField( expression ) );
                value = default;
                return false;
            }

            value = (T)Convert.ChangeType( constant.Value, typeof(T) );
            return true;
        }


        protected bool TryGetSymbolicConstant<T>( ExpressionSyntax expression, out SymbolicConstant<T> value, bool acceptSymbolic = true )
            where T: IComparable<T>
        {
            var constant = Model.GetConstantValue( expression );
            if( !constant.HasValue )
            {
                var symbol = Model.GetSymbolInfo( expression ).Symbol;
                if( _DateTimeConstants.TryGetValue( symbol, out var dt ) ||
                    _DateTimeConstants.TryGetValue( symbol.OriginalDefinition, out dt ) )
                {
                    value = new SymbolicConstant<T>( (T)(object)dt );
                    return true;
                }

                if( _DateTimeConstructions.Contains( symbol ) )
                    return TryCreateDateTime( (ObjectCreationExpressionSyntax)expression, out value );

                if( acceptSymbolic )
                {
                    if( symbol is IFieldSymbol field )
                    {
                        // Symbolic Range
                        value = new SymbolicConstant<T>( field );
                        return true;
                    }

                    if( symbol is IPropertySymbol property &&
                        !property.IsIndexer )
                    {
                        // Symbolic Range
                        value = new SymbolicConstant<T>( property );
                        return true;
                    }
                }

                if( Equals( ( symbol as ILocalSymbol )?.Type ??
                            ( symbol as IPropertySymbol )?.Type ??
                            ( symbol as IMethodSymbol )?.ReturnType,
                            _DateTimeType ) )
                    ReportDiagnostic( SmartTestsDiagnostics.CreateNotADateCreation( expression ) );
                else
                    ReportDiagnostic( SmartTestsDiagnostics.CreateNotAConstantPropertyField( expression ) );
                value = default;
                return false;
            }

            value = new SymbolicConstant<T>( (T)Convert.ChangeType( constant.Value, typeof(T) ) );
            return true;
        }


        private bool TryCreateDateTime<T>( ObjectCreationExpressionSyntax expression, out T result )
        {
            try
            {
                var args = new List<object>();
                foreach( var arg in expression.ArgumentList.Arguments )
                    args.Add( Model.GetConstantValue( arg.Expression ).Value );
                result = (T)Activator.CreateInstance( typeof(T), Activator.CreateInstance( typeof(DateTime), args.ToArray() ) );
                return true;
            }
            catch
            {
                ReportDiagnostic( SmartTestsDiagnostics.CreateNotADateCreation( expression ) );
                result = default(T);
                return false;
            }
        }


        protected bool TryGetConstant( ExpressionSyntax expression, out bool value )
        {
            var constant = Model.GetConstantValue( expression );
            if( !constant.HasValue )
            {
                ReportDiagnostic( SmartTestsDiagnostics.CreateNotAConstantPropertyField( expression ) );
                value = false;
                return false;
            }

            value = (bool)Convert.ChangeType( constant.Value, typeof(bool) );
            return true;
        }
    }
}