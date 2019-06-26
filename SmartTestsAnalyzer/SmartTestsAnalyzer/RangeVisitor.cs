﻿using System;
using System.Collections.Generic;
using System.Diagnostics;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using SmartTests;

using SmartTestsAnalyzer.Helpers;



namespace SmartTestsAnalyzer
{
    class RangeVisitor: CSharpSyntaxVisitor<IRangeVisitor>
    {
        public RangeVisitor( SemanticModel model, Action<Diagnostic> reportDiagnostic )
        {
            _Model = model;

            var smartTestType = model.Compilation.GetTypeByMetadataName( "SmartTests.SmartTest" );
            AddType( smartTestType, "Byte", () => new RangeVisitor<byte>( model, SmartTest.Byte, null, reportDiagnostic ) );
            AddType( smartTestType, "ByteRange", () => new RangeVisitor<byte>( model, SmartTest.ByteRange, null, reportDiagnostic ) );
            AddType( smartTestType, "SByte", () => new RangeVisitor<sbyte>( model, SmartTest.SByte, null, reportDiagnostic ) );
            AddType( smartTestType, "SByteRange", () => new RangeVisitor<sbyte>( model, SmartTest.SByteRange, null, reportDiagnostic ) );
            AddType( smartTestType, "Short", () => new RangeVisitor<short>( model, SmartTest.Short, null, reportDiagnostic ) );
            AddType( smartTestType, "Int16Range", () => new RangeVisitor<short>( model, SmartTest.Int16Range, null, reportDiagnostic ) );
            AddType( smartTestType, "UShort", () => new RangeVisitor<ushort>( model, SmartTest.UShort, null, reportDiagnostic ) );
            AddType( smartTestType, "UInt16Range", () => new RangeVisitor<ushort>( model, SmartTest.UInt16Range, null, reportDiagnostic ) );
            AddType( smartTestType, "Int", () => new RangeVisitor<int>( model, SmartTest.Int, null, reportDiagnostic ) );
            AddType( smartTestType, "Int32Range", () => new RangeVisitor<int>( model, SmartTest.Int32Range, null, reportDiagnostic ) );
            AddType( smartTestType, "UInt", () => new RangeVisitor<uint>( model, SmartTest.UInt, null, reportDiagnostic ) );
            AddType( smartTestType, "UInt32Range", () => new RangeVisitor<uint>( model, SmartTest.UInt32Range, null, reportDiagnostic ) );
            AddType( smartTestType, "Long", () => new RangeVisitor<long>( model, SmartTest.Long, null, reportDiagnostic ) );
            AddType( smartTestType, "Int64Range", () => new RangeVisitor<long>( model, SmartTest.Int64Range, null, reportDiagnostic ) );
            AddType( smartTestType, "ULong", () => new RangeVisitor<ulong>( model, SmartTest.ULong, null, reportDiagnostic ) );
            AddType( smartTestType, "UInt64Range", () => new RangeVisitor<ulong>( model, SmartTest.UInt64Range, null, reportDiagnostic ) );
            AddType( smartTestType, "Float", () => new RangeVisitor<float>( model, SmartTest.Float, null, reportDiagnostic ) );
            AddType( smartTestType, "SingleRange", () => new RangeVisitor<float>( model, SmartTest.SingleRange, null, reportDiagnostic ) );
            AddType( smartTestType, "Double", () => new RangeVisitor<double>( model, SmartTest.Double, null, reportDiagnostic ) );
            AddType( smartTestType, "DoubleRange", () => new RangeVisitor<double>( model, SmartTest.DoubleRange, null, reportDiagnostic ) );
            AddType( smartTestType, "DecimalRange", () => new RangeVisitor<decimal>( model, SmartTest.DecimalRange, null, reportDiagnostic ) );
            AddType( smartTestType, "DateTimeRange", () => new RangeVisitor<DateTime>( model, SmartTest.DateTimeRange, null, reportDiagnostic ) );
            AddType( smartTestType, "Enum", () => new EnumVisitor( model, reportDiagnostic ) );
            AddType( smartTestType, "EnumRange", () => new EnumVisitor( model, reportDiagnostic ) );
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