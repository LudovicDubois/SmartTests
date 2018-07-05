using System;
using System.Linq;

using JetBrains.Annotations;

using Microsoft.CodeAnalysis;



namespace SmartTestsAnalyzer.Helpers
{
    public static class TypeSymbolHelper
    {
        public static IMethodSymbol[] GetMethods( [NotNull] this ITypeSymbol @this, [NotNull] string name )
        {
            if( @this == null )
                throw new ArgumentNullException( nameof(@this) );
            if( name == null )
                throw new ArgumentNullException( nameof(name) );

            return @this.GetMembers( name )
                        .Select( member => member as IMethodSymbol )
                        .Where( member => member != null )
                        .ToArray();
        }


        public static IMethodSymbol GetMethod( [NotNull] this ITypeSymbol @this, [NotNull] string name ) => @this.GetMethods( name )[ 0 ];
    }
}