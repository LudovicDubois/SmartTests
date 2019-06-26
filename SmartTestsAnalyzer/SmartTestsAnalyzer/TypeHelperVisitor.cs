using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using SmartTests;
using SmartTests.Ranges;

using SmartTestsAnalyzer.Criterias;
using SmartTestsAnalyzer.Helpers;



namespace SmartTestsAnalyzer
{
    class TypeHelperVisitor: CSharpSyntaxVisitor
    {
        public TypeHelperVisitor( SemanticModel model, bool isError, Action<Diagnostic> reportDiagnostic )
        {
            _Model = model;
            _IsError = isError;
            _Roots.Add( _Model.Compilation.GetTypeByMetadataName( "SmartTests.Ranges.ByteTypeHelper" ), () => new RangeVisitor<byte>( model, SmartTest.ByteRange, typeof(ByteTypeHelper), reportDiagnostic ) );
            _Roots.Add( _Model.Compilation.GetTypeByMetadataName( "SmartTests.Ranges.DecimalTypeHelper" ), () => new RangeVisitor<decimal>( model, SmartTest.DecimalRange, typeof(DecimalTypeHelper), reportDiagnostic ) );
            _Roots.Add( _Model.Compilation.GetTypeByMetadataName( "SmartTests.Ranges.DoubleTypeHelper" ), () => new RangeVisitor<double>( model, SmartTest.DoubleRange, typeof(DoubleTypeHelper), reportDiagnostic ) );
            _Roots.Add( _Model.Compilation.GetTypeByMetadataName( "SmartTests.Ranges.SingleTypeHelper" ), () => new RangeVisitor<float>( model, SmartTest.SingleRange, typeof(SingleTypeHelper), reportDiagnostic ) );
            _Roots.Add( _Model.Compilation.GetTypeByMetadataName( "SmartTests.Ranges.Int32TypeHelper" ), () => new RangeVisitor<int>( model, SmartTest.Int32Range, typeof(Int32TypeHelper), reportDiagnostic ) );
            _Roots.Add( _Model.Compilation.GetTypeByMetadataName( "SmartTests.Ranges.Int64TypeHelper" ), () => new RangeVisitor<long>( model, SmartTest.Int64Range, typeof(Int64TypeHelper), reportDiagnostic ) );
            _Roots.Add( _Model.Compilation.GetTypeByMetadataName( "SmartTests.Ranges.SByteTypeHelper" ), () => new RangeVisitor<sbyte>( model, SmartTest.SByteRange, typeof(SByteTypeHelper), reportDiagnostic ) );
            _Roots.Add( _Model.Compilation.GetTypeByMetadataName( "SmartTests.Ranges.Int16TypeHelper" ), () => new RangeVisitor<short>( model, SmartTest.Int16Range, typeof(Int16TypeHelper), reportDiagnostic ) );
            _Roots.Add( _Model.Compilation.GetTypeByMetadataName( "SmartTests.Ranges.UInt32TypeHelper" ), () => new RangeVisitor<uint>( model, SmartTest.UInt32Range, typeof(UInt32TypeHelper), reportDiagnostic ) );
            _Roots.Add( _Model.Compilation.GetTypeByMetadataName( "SmartTests.Ranges.UInt64TypeHelper" ), () => new RangeVisitor<ulong>( model, SmartTest.UInt64Range, typeof(UInt64TypeHelper), reportDiagnostic ) );
            _Roots.Add( _Model.Compilation.GetTypeByMetadataName( "SmartTests.Ranges.UInt16TypeHelper" ), () => new RangeVisitor<ushort>( model, SmartTest.UInt16Range, typeof(UInt16TypeHelper), reportDiagnostic ) );
            _Roots.Add( _Model.Compilation.GetTypeByMetadataName( "SmartTests.Ranges.DateTimeTypeHelper" ), () => new RangeVisitor<DateTime>( model, SmartTest.DateTimeRange, typeof(DateTimeTypeHelper), reportDiagnostic ) );
            _Roots.Add( _Model.Compilation.GetTypeByMetadataName( "SmartTests.Ranges.EnumTypeHelper" ), () => new EnumVisitor( model, reportDiagnostic ) );
        }


        private readonly SemanticModel _Model;
        private readonly bool _IsError;
        private readonly Dictionary<ITypeSymbol, Func<IRangeVisitor>> _Roots = new Dictionary<ITypeSymbol, Func<IRangeVisitor>>();
        private ExpressionSyntax _Expression;
        private string _ParameterName;
        private string _ParameterPath;
        private CasesAndOr _Case;


        public CasesAndOr GetCase( ExpressionSyntax parameterNameExpression )
        {
            _Expression = parameterNameExpression;
            _Expression.Accept( this );
            return _Case;
        }


        public override void VisitParenthesizedExpression( ParenthesizedExpressionSyntax node ) => node.Expression.Accept( this );


        public override void VisitParenthesizedLambdaExpression( ParenthesizedLambdaExpressionSyntax node )
        {
            // Start point
            // => must separate the path from the range
            ( node.Body as ExpressionSyntax )?.Accept( this );
        }


        private IRangeVisitor _RangeVisitor;


        public override void VisitInvocationExpression( InvocationExpressionSyntax node )
        {
            node.Expression.Accept( this );

            // Should be a method from XTypeHelper
            var methodSymbol = _Model.GetSymbol( node ) as IMethodSymbol;
            if( methodSymbol == null )
                return;

            if( _RangeVisitor != null )
            {
                _RangeVisitor.VisitInvocationExpression( node );
                if( _RangeVisitor.Root == null )
                    _Case = null;
                return;
            }

            // Create Root
            if( !_Roots.TryGetValue( methodSymbol.ContainingType, out var rangeVisitorCreator ) )
                return;

            _RangeVisitor = rangeVisitorCreator();
            _RangeVisitor.VisitInvocationExpression( node );
            if( _RangeVisitor.Root != null )
                _Case = new CasesAndOr( _Expression,
                                        new TestedParameter( _ParameterName, _ParameterPath ),
                                        new RangeAnalysis( _RangeVisitor.Root, _IsError || _RangeVisitor.IsError ),
                                        _IsError || _RangeVisitor.IsError );
        }


        public override void VisitMemberAccessExpression( MemberAccessExpressionSyntax node )
        {
            node.Expression.Accept( this );
            if( node.Parent.Kind() == SyntaxKind.InvocationExpression )
                return;

            _ParameterPath = _ParameterPath + "." + node.Name.Identifier.Text;
        }


        public override void VisitIdentifierName( IdentifierNameSyntax node )
        {
            _ParameterName = node.Identifier.Text;
            _ParameterPath = _ParameterName;
        }
    }
}