using System.Linq;

using Microsoft.CodeAnalysis;



namespace SmartTestsAnalyzer.Helpers
{
    public static class SymbolHelper
    {
        public static bool HasAttribute( this ISymbol symbol, INamedTypeSymbol type ) => symbol.GetAttributes().Any( attribute => attribute.AttributeClass == type );
        public static bool HasAttribute( this ISymbol symbol, SemanticModel model, string typeName ) => symbol.HasAttribute( model.Compilation.GetTypeByMetadataName( typeName ) );
    }
}