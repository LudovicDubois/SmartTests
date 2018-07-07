using System;
using System.Collections;
using System.Collections.Generic;



namespace SmartTests
{
    /// <summary>
    ///     A combination of <see cref="Criteria" /> expression for a parameter of the tested member.
    /// </summary>
    public class Case: IEnumerable<Case>
    {
        /// <summary>
        ///     Creates an instance of <see cref="Case" /> without any <see cref="SmartTests.Criteria" />.
        /// </summary>
        protected Case()
        { }


        /// <summary>
        ///     Creates an instance of <see cref="Case" /> for a global <see cref="SmartTests.Criteria" /> expression.
        /// </summary>
        /// <param name="criteria">The <see cref="SmartTests.Criteria" /> expression.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="criteria" /> is <c>null</c>.</exception>
        public Case( Criteria criteria )
        {
            if( criteria == null )
                throw new ArgumentNullException( nameof(criteria) );
            Criteria = criteria;
        }


        /// <summary>
        ///     Creates an instance of <see cref="Case" /> for a parameter specific <see cref="SmartTests.Criteria" /> expression.
        /// </summary>
        /// <param name="parameterName">
        ///     The name of the parameter for which to associate a <see cref="SmartTests.Criteria" />
        ///     expression.
        /// </param>
        /// <param name="criteria">
        ///     The <see cref="SmartTests.Criteria" /> expression for the provided
        ///     <paramref name="parameterName" />.
        /// </param>
        /// <exception cref="ArgumentNullException">If <paramref name="criteria" /> is <c>null</c>.</exception>
        public Case( string parameterName, Criteria criteria )
        {
            if( criteria == null )
                throw new ArgumentNullException( nameof(criteria) );
            ParameterName = parameterName;
            Criteria = criteria;
        }


        /// <summary>
        ///     The name of the parameter for which the <see cref="SmartTests.Criteria" /> expression belongs to, if any.
        /// </summary>
        public string ParameterName { get; }
        /// <summary>
        ///     The <see cref="SmartTests.Criteria" /> expression.
        /// </summary>
        public Criteria Criteria { get; }


        /// <summary>
        ///     Combines two <see cref="Case" />s.
        /// </summary>
        /// <param name="case1">The first <see cref="Case" /> to combine.</param>
        /// <param name="case2">The second <see cref="Case" /> to combine.</param>
        /// <returns>The combined <see cref="Case" />.</returns>
        /// <exception cref="ArgumentNullException">
        ///     If either <paramref name="case1" /> or <paramref name="case2" /> are
        ///     <c>null</c>.
        /// </exception>
        /// <remarks>
        ///     Use it to combine multiple cases, for example when testing a method/indexer/constructor with several
        ///     parameters.
        /// </remarks>
        public static Case operator &( Case case1, Case case2 )
        {
            if( case1 == null )
                throw new ArgumentNullException( nameof(case1) );
            if( case2 == null )
                throw new ArgumentNullException( nameof(case2) );

            return case1.Combine( case2 );
        }


        internal virtual Case Combine( Case other )
        {
            if( other == null )
                throw new ArgumentNullException( nameof(other) );
            return new MultiCase
                   {
                       this,
                       other
                   };
        }


        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();


        /// <inheritdoc />
        public virtual IEnumerator<Case> GetEnumerator()
        {
            yield return this;
        }


        private class MultiCase: Case
        {
            public List<Case> Cases { get; } = new List<Case>();

            public void Add( Case cas ) => Cases.Add( cas );


            internal override Case Combine( Case other )
            {
                Cases.Add( other );
                return this;
            }


            public override IEnumerator<Case> GetEnumerator() => Cases.GetEnumerator();
        }
    }
}