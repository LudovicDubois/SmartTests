using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using SmartTests.Ranges;

using SmartTestsAnalyzer.Helpers;

// ReSharper disable MemberCanBePrivate.Global


namespace SmartTestsAnalyzer
{
    class EnumTypeAnalyzer: IType
    {
        public EnumTypeAnalyzer( INamedTypeSymbol type, params object[] values )
        {
            Type = type;

            if( Type != null )
            {
                Fields.AddRange( Type.GetMembers().Where( m => m.Kind == SymbolKind.Field ).Cast<IFieldSymbol>() );
                _Symbols = Fields.ToDictionary( field => field.ConstantValue );
                Values.AddRange( values.Select( val => _Symbols[ val ] ) );
            }
            else
            {
                Values.AddRange( values.Cast<IFieldSymbol>() );
            }
        }


        public INamedTypeSymbol Type { get; }
        public List<IFieldSymbol> Fields { get; } = new List<IFieldSymbol>();
        public List<IFieldSymbol> Values { get; } = new List<IFieldSymbol>();


        private readonly Dictionary<object, IFieldSymbol> _Symbols;


        public void AddValue( object value ) => Values.Add( _Symbols[ value ] );


        public override string ToString()
        {
            var result = new StringBuilder();
            if( Values.Count == 1 )
            {
                // Enum.Value
                result.Append( "EnumRange.Value(" );
                result.Append( Values[ 0 ].ToDisplayString( SymbolDisplayFormat.CSharpShortErrorMessageFormat ) );
                result.Append( ')' );
                return result.ToString();
            }

            // Enum.Values
            result.Append( "EnumRange.Values(" );
            if( Values.Count > 0 )
            {
                foreach( var value in Values )
                {
                    result.Append( value.ToDisplayString( SymbolDisplayFormat.CSharpShortErrorMessageFormat ) );
                    result.Append( ", " );
                }

                result.Length -= 2;
            }

            result.Append( ')' );

            return result.ToString();
        }
    }


    class EnumVisitor: BaseVisitor, IRangeVisitor
    {
        public EnumVisitor( SemanticModel model, Action<Diagnostic> reportDiagnostic )
            : base( model, reportDiagnostic )
        {
            // EnumType methods
            var enumType = Model.Compilation.GetTypeByMetadataName( typeof(EnumType).FullName );
            _ValueMethod = enumType.GetMethod( nameof(EnumType.Value) );
            _ErrorValueMethod = enumType.GetMethod( nameof(EnumType.ErrorValue) );
            _ValuesMethod = enumType.GetMethod( nameof(EnumType.Values) );
            _ErrorValuesMethod = enumType.GetMethod( nameof(EnumType.ErrorValues) );

            var enumHelperType = Model.Compilation.GetTypeByMetadataName( typeof(EnumTypeHelper).FullName );
            _HelperValuesMethod = enumHelperType.GetMethod( nameof(EnumTypeHelper.Values) );
        }


        private readonly IMethodSymbol _ValueMethod;
        private readonly IMethodSymbol _ErrorValueMethod;
        private readonly IMethodSymbol _ValuesMethod;
        private readonly IMethodSymbol _HelperValuesMethod;
        private readonly IMethodSymbol _ErrorValuesMethod;

        public IType Root { get; private set; }
        public bool IsError { get; set; }


        private void Value( InvocationExpressionSyntax node )
        {
            var expression = node.GetArgument( 0 ).Expression;
            if( TryGetConstant( expression, out object value ) )
                Root = new EnumTypeAnalyzer( Model.GetSymbol( expression ).ContainingType, value );
        }


        private void Values( InvocationExpressionSyntax node, bool fromStart )
        {
            // We do not care about argument 0, as it is the out parameter
            var start = fromStart ? 0 : 1;
            var expression = node.GetArgument( start ).Expression;
            var enumType = new EnumTypeAnalyzer( Model.GetSymbol( expression ).ContainingType );
            foreach( var argument in node.ArgumentList.Arguments.Skip( start ) )
            {
                if( !TryGetConstant( argument.Expression, out int value ) )
                    return;

                enumType.AddValue( value );
            }

            Root = enumType;
        }


        void IRangeVisitor.VisitInvocationExpression( InvocationExpressionSyntax node )
        {
            if( !( Model.GetSymbol( node ) is IMethodSymbol criteria ) )
                return;

            if( criteria.OriginalDefinition == null )
                //?!?
                return;


            if( criteria.OriginalDefinition.ReducedFrom?.Equals( _HelperValuesMethod ) == true )
            {
                Values( node, true );
                return;
            }

            if( criteria.OriginalDefinition.Equals( _ValueMethod ) )
            {
                Value( node );
                return;
            }

            if( criteria.OriginalDefinition.Equals( _ErrorValueMethod ) )
            {
                Value( node );
                IsError = true;
                return;
            }

            if( criteria.OriginalDefinition.Equals( _ValuesMethod ) )
            {
                Values( node, false );
                return;
            }

            if( criteria.OriginalDefinition.Equals( _ErrorValuesMethod ) )
            {
                Values( node, false );
                IsError = true;
            }
        }
    }
}