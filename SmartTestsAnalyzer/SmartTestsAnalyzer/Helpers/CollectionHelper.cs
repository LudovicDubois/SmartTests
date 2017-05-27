using System.Collections.Generic;
using System.Linq;



namespace SmartTestsAnalyzer.Helpers
{
    public static class CollectionHelper
    {
        public static bool Equivalent<T>( this ICollection<T> @this, ICollection<T> other )
        {
            return other.Count == @this.Count &&
                   other.All( @this.Contains );
        }
    }
}