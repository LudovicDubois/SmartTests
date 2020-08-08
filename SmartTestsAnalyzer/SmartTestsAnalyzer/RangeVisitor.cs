using System;
using System.Collections.Generic;
using System.Diagnostics;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using SmartTestsAnalyzer.Helpers;
using SmartTestsAnalyzer.Ranges;



namespace SmartTestsAnalyzer
{
    internal class RangeVisitor: CSharpSyntaxVisitor<IRangeVisitor>
    {
        public RangeVisitor( SemanticModel model, Action<Diagnostic> reportDiagnostic )
        {
            _Model = model;

            var smartTestType = model.Compilation.GetTypeByMetadataName( "SmartTests.SmartTest" );
#pragma warning disable 618
            AddType( smartTestType, "Byte", () => new RangeVisitor<byte>( model, new SymbolicByteType(), null, reportDiagnostic ) );
            AddType( smartTestType, "ByteRange", () => new RangeVisitor<byte>( model, new SymbolicByteType(), null, reportDiagnostic ) );
            AddType( smartTestType, "SByte", () => new RangeVisitor<sbyte>( model, new SymbolicSByteType(), null, reportDiagnostic ) );
            AddType( smartTestType, "SByteRange", () => new RangeVisitor<sbyte>( model, new SymbolicSByteType(), null, reportDiagnostic ) );
            AddType( smartTestType, "Short", () => new RangeVisitor<short>( model, new SymbolicInt16Type(), null, reportDiagnostic ) );
            AddType( smartTestType, "Int16Range", () => new RangeVisitor<short>( model, new SymbolicInt16Type(), null, reportDiagnostic ) );
            AddType( smartTestType, "UShort", () => new RangeVisitor<ushort>( model, new SymbolicUInt16Type(), null, reportDiagnostic ) );
            AddType( smartTestType, "UInt16Range", () => new RangeVisitor<ushort>( model, new SymbolicUInt16Type(), null, reportDiagnostic ) );
            AddType( smartTestType, "Int", () => new RangeVisitor<int>( model, new SymbolicInt32Type(), null, reportDiagnostic ) );
            AddType( smartTestType, "Int32Range", () => new RangeVisitor<int>( model, new SymbolicInt32Type(), null, reportDiagnostic ) );
            AddType( smartTestType, "UInt", () => new RangeVisitor<uint>( model, new SymbolicUInt32Type(), null, reportDiagnostic ) );
            AddType( smartTestType, "UInt32Range", () => new RangeVisitor<uint>( model, new SymbolicUInt32Type(), null, reportDiagnostic ) );
            AddType( smartTestType, "Long", () => new RangeVisitor<long>( model, new SymbolicInt64Type(), null, reportDiagnostic ) );
            AddType( smartTestType, "Int64Range", () => new RangeVisitor<long>( model, new SymbolicInt64Type(), null, reportDiagnostic ) );
            AddType( smartTestType, "ULong", () => new RangeVisitor<ulong>( model, new SymbolicUInt64Type(), null, reportDiagnostic ) );
            AddType( smartTestType, "UInt64Range", () => new RangeVisitor<ulong>( model, new SymbolicUInt64Type(), null, reportDiagnostic ) );
            AddType( smartTestType, "Float", () => new RangeVisitor<float>( model, new SymbolicSingleType(), null, reportDiagnostic ) );
            AddType( smartTestType, "SingleRange", () => new RangeVisitor<float>( model, new SymbolicSingleType(), null, reportDiagnostic ) );
            AddType( smartTestType, "Double", () => new RangeVisitor<double>( model, new SymbolicDoubleType(), null, reportDiagnostic ) );
            AddType( smartTestType, "DoubleRange", () => new RangeVisitor<double>( model, new SymbolicDoubleType(), null, reportDiagnostic ) );
            AddType( smartTestType, "DecimalRange", () => new RangeVisitor<decimal>( model, new SymbolicDecimalType(), null, reportDiagnostic ) );
            AddType( smartTestType, "DateTimeRange", () => new RangeVisitor<DateTime>( model, new SymbolicDateTimeType(), null, reportDiagnostic ) );
            AddType( smartTestType, "Enum", () => new EnumVisitor( model, reportDiagnostic ) );
            AddType( smartTestType, "EnumRange", () => new EnumVisitor( model, reportDiagnostic ) );
#pragma warning restore 618
        }


        private readonly SemanticModel _Model;


        public override IRangeVisitor VisitParenthesizedExpression( ParenthesizedExpressionSyntax node ) => node.Expression.Accept( this );


        private readonly Dictionary<IPropertySymbol, Func<IRangeVisitor>> _TypeProperties =
            new Dictionary<IPropertySymbol, Func<IRangeVisitor>>();


        private void AddType( ITypeSymbol smartTestType, string propertyName, Func<IRangeVisitor> iTypeCreator )
        {
            var rootProperty = (IPropertySymbol)smartTestType.GetMembers( propertyName )[ 0 ];
            Debug.Assert( rootProperty != null, $"Problem with SmartTest.{propertyName} property" );
            _TypeProperties[ rootProperty ] = iTypeCreator;
        }


        private IRangeVisitor GetRoot( SyntaxNode node )
        {
            var member = _Model.GetSymbol( node );
            if( member is IPropertySymbol type )
                if( _TypeProperties.TryGetValue( type, out var func ) )
                    return func();
            return null;
        }


        public override IRangeVisitor VisitIdentifierName( IdentifierNameSyntax node ) => GetRoot( node );


        public override IRangeVisitor VisitMemberAccessExpression( MemberAccessExpressionSyntax node ) => GetRoot( node ) ?? node.Expression.Accept( this );


        public override IRangeVisitor VisitInvocationExpression( InvocationExpressionSyntax node )
        {
            var result = node.Expression.Accept( this );
            result?.VisitInvocationExpression( node );
            return result;
        }
    }
}