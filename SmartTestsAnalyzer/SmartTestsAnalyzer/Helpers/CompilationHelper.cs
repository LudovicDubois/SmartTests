using JetBrains.Annotations;

using Microsoft.CodeAnalysis;



namespace SmartTestsAnalyzer.Helpers
{
    static class CompilationHelper
    {
        public static IMethodSymbol FindMethodSymbol( [NotNull] this Compilation @this, [NotNull] SyntaxNode node, [NotNull] params IMethodSymbol[] methods ) => @this.GetSemanticModel( node.SyntaxTree ).FindMethodSymbol( node, methods );


        public static bool HasMethod( [NotNull] this Compilation @this, [NotNull] SyntaxNode node, [NotNull] params IMethodSymbol[] methods ) => FindMethodSymbol( @this, node, methods ) != null;

        public static ISymbol GetSymbol( [NotNull] this Compilation @this, SyntaxNode node ) => @this.GetSemanticModel( node.SyntaxTree ).GetSymbolInfo( node ).Symbol;
    }
}