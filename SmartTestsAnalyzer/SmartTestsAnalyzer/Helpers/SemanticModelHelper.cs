using System;

using JetBrains.Annotations;

using Microsoft.CodeAnalysis;



namespace SmartTestsAnalyzer.Helpers
{
    public static class SemanticModelHelper
    {
        public static IMethodSymbol FindMethodSymbol( [NotNull] this SemanticModel @this, SyntaxNode node, [NotNull] params IMethodSymbol[] methods )
        {
            if( @this == null )
                throw new ArgumentNullException( nameof( @this ) );
            if( node == null )
                return null;
            if( methods == null )
                throw new ArgumentNullException( nameof( methods ) );

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


        public static bool HasMethod( [NotNull] this SemanticModel @this, SyntaxNode node, [NotNull] params IMethodSymbol[] methods ) => @this.FindMethodSymbol( node, methods ) != null;
    }
}