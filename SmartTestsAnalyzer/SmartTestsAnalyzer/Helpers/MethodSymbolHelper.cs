using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;



namespace SmartTestsAnalyzer.Helpers
{
    static class MethodSymbolHelper
    {
        public static IEnumerable<T> GetDescendantNodes<T>( this IMethodSymbol method )
        {
            return method.DeclaringSyntaxReferences
                         .Select( syntaxReference => syntaxReference.GetSyntax() )
                         .SelectMany( syntaxNode => syntaxNode.DescendantNodes().OfType<T>() );
        }
    }
}