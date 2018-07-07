using Microsoft.CodeAnalysis;

// ReSharper disable UnusedMember.Global


namespace SmartTestsAnalyzer.Helpers
{
    static class CompilationHelper
    {
        public static IMethodSymbol FindMethodSymbol( this Compilation @this, SyntaxNode node, params IMethodSymbol[] methods ) => @this.GetSemanticModel( node.SyntaxTree ).FindMethodSymbol( node, methods );


        public static bool HasMethod( this Compilation @this, SyntaxNode node, params IMethodSymbol[] methods ) => FindMethodSymbol( @this, node, methods ) != null;

        public static ISymbol GetSymbol( this Compilation @this, SyntaxNode node ) => @this.GetSemanticModel( node.SyntaxTree ).GetSymbolInfo( node ).Symbol;
    }
}