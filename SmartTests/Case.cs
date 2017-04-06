using System;
using System.Collections;
using System.Collections.Generic;

using JetBrains.Annotations;



namespace SmartTests
{
    [PublicAPI]
    public class Case: IEnumerable<Case>
    {
        protected Case()
        {}


        public Case( [NotNull] Criteria criteria )
        {
            if( criteria == null )
                throw new ArgumentNullException( nameof( criteria ) );
            Criteria = criteria;
        }


        public Case( string parameterName, [NotNull] Criteria criteria )
        {
            if( criteria == null )
                throw new ArgumentNullException( nameof( criteria ) );
            ParameterName = parameterName;
            Criteria = criteria;
        }


        public string ParameterName { get; }
        public Criteria Criteria { get; }


        public static implicit operator Case( [NotNull] Criteria criteria ) => new Case( criteria );


        public static Case operator &( [NotNull] Case case1, [NotNull] Case case2 )
        {
            if( case1 == null )
                throw new ArgumentNullException( nameof( case1 ) );
            if( case2 == null )
                throw new ArgumentNullException( nameof( case2 ) );

            return case1.Combine( case2 );
        }


        protected virtual Case Combine( [NotNull] Case other )
        {
            if( other == null )
                throw new ArgumentNullException( nameof( other ) );
            return new MultiCase
                   {
                       this,
                       other
                   };
        }


        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();


        public virtual IEnumerator<Case> GetEnumerator()
        {
            yield return this;
        }


        private class MultiCase: Case
        {
            public List<Case> Cases { get; } = new List<Case>();

            public void Add( Case cas ) => Cases.Add( cas );


            protected override Case Combine( Case other )
            {
                Cases.Add( other );
                return this;
            }


            public override IEnumerator<Case> GetEnumerator() => Cases.GetEnumerator();
        }
    }
}