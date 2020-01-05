using System;



namespace SmartTests
{
    /// <summary>
    ///     An expression of Criterion to specify the goal of your test.
    /// </summary>
    public abstract class Criteria
    {
        /// <summary>
        ///     Combines two orthogonal <see cref="Criteria" />s.
        /// </summary>
        /// <param name="criteria1">The first <see cref="Criteria" /> to combine.</param>
        /// <param name="criteria2">The second <see cref="Criteria" /> to combine.</param>
        /// <returns>The combined <see cref="Criteria" />.</returns>
        /// <exception cref="ArgumentNullException">
        ///     If either <paramref name="criteria1" /> or <paramref name="criteria2" /> are
        ///     <c>null</c>.
        /// </exception>
        /// <remarks>
        ///     Use &amp; operator to combine orthogonal <see cref="Criteria" />s.
        /// </remarks>
        /// <seealso cref="op_BitwiseOr" />
        public static Criteria operator &( Criteria criteria1, Criteria criteria2 )
        {
            if( criteria1 == null )
                throw new ArgumentNullException( nameof(criteria1) );
            if( criteria2 == null )
                throw new ArgumentNullException( nameof(criteria2) );

            return new AndCriteria( criteria1, criteria2 );
        }


        /// <summary>
        ///     Combines two criterions of the same <see cref="Criteria" />.
        /// </summary>
        /// <param name="criteria1">The first <see cref="Criteria" /> to combine.</param>
        /// <param name="criteria2">The second <see cref="Criteria" /> to combine.</param>
        /// <returns>The combined <see cref="Criteria" />.</returns>
        /// <exception cref="ArgumentNullException">
        ///     If either <paramref name="criteria1" /> or <paramref name="criteria2" /> are
        ///     <c>null</c>.
        /// </exception>
        /// <remarks>
        ///     Use | operator to combine criterias of the same <see cref="Criteria" />.
        /// </remarks>
        /// <seealso cref="op_BitwiseAnd" />
        public static Criteria operator |( Criteria criteria1, Criteria criteria2 )
        {
            if( criteria1 == null )
                throw new ArgumentNullException( nameof(criteria1) );
            if( criteria2 == null )
                throw new ArgumentNullException( nameof(criteria2) );

            return new OrCriteria( criteria1, criteria2 );
        }


        // ReSharper disable UnusedParameter.Local
        private class AndCriteria: Criteria
        {
            public AndCriteria( Criteria case1, Criteria case2 )
            { }
        }


        private class OrCriteria: Criteria
        {
            public OrCriteria( Criteria case1, Criteria case2 )
            { }
        }
        // ReSharper restore UnusedParameter.Local
    }
}