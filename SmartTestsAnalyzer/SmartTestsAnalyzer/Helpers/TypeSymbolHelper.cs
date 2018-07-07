using System;
using System.Linq;

using Microsoft.CodeAnalysis;



namespace SmartTestsAnalyzer.Helpers
{
    public static class TypeSymbolHelper
    {
        public static IMethodSymbol[] GetMethods( this ITypeSymbol @this, string name )
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


        public static IMethodSymbol GetMethod( this ITypeSymbol @this, string name ) => @this.GetMethods( name )[ 0 ];
    }
}