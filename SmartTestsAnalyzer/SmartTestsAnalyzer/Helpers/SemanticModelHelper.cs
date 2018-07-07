using System;

using Microsoft.CodeAnalysis;



namespace SmartTestsAnalyzer.Helpers
{
    public static class SemanticModelHelper
    {
        public static IMethodSymbol FindMethodSymbol( this SemanticModel @this, SyntaxNode node, params IMethodSymbol[] methods )
        {
            if( @this == null )
                throw new ArgumentNullException( nameof(@this) );
            if( node == null )
                throw new ArgumentNullException( nameof(node) );
            if( methods == null )
                throw new ArgumentNullException( nameof(methods) );

            var symbol = @this.GetSymbolInfo( node ).Symbol as IMethodSymbol;
            if( symbol == null )
                return null;

            foreach( var method in methods )
                if( method.IsGenericMethod )
                {
                    if( method == symbol.ConstructedFrom )
                        return method;
                }
                else if( method == symbol )
                    return method;

            // Not found!
            return null;
        }


        public static bool HasMethod( this SemanticModel @this, SyntaxNode node, params IMethodSymbol[] methods ) => @this.FindMethodSymbol( node, methods ) != null;

        public static ISymbol GetSymbol( this SemanticModel @this, SyntaxNode node ) => @this.GetSymbolInfo( node ).Symbol;
    }
}