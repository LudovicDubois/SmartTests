using System;

using JetBrains.Annotations;



namespace SmartTests
{
    [PublicAPI]
    public abstract class Criteria
    {
        public static Criteria operator &( [NotNull] Criteria case1, [NotNull] Criteria case2 )
        {
            if( case1 == null )
                throw new ArgumentNullException( nameof( case1 ) );
            if( case2 == null )
                throw new ArgumentNullException( nameof( case2 ) );

            return new AndCriteria( case1, case2 );
        }


        public static Criteria operator |( [NotNull] Criteria case1, [NotNull] Criteria case2 )
        {
            if( case1 == null )
                throw new ArgumentNullException( nameof( case1 ) );
            if( case2 == null )
                throw new ArgumentNullException( nameof( case2 ) );

            return new OrCriteria( case1, case2 );
        }


        private class AndCriteria: Criteria
        {
            public AndCriteria( Criteria case1, Criteria case2 )
            {}
        }


        private class OrCriteria: Criteria
        {
            public OrCriteria( Criteria case1, Criteria case2 )
            {}
        }
    }
}