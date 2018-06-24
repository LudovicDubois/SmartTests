using System;



namespace SmartTests.Ranges
{
    /// <summary>
    ///     A chunk of consecutive integer values (from <see cref="Min" /> to <see cref="Max" />)
    /// </summary>
    public struct Chunk<T>
        where T: IComparable<T>
    {
        /// <summary>
        ///     Instantiates a chunk
        /// </summary>
        /// <param name="min">The min value (included) of the chunk.</param>
        /// <param name="max">The max value (included) of the chunk.</param>
        /// <exception cref="ArgumentException">If <paramref name="min" /> &gt; <paramref name="max" /></exception>
        public Chunk( T min, T max )
        {
            Min = min;
            Max = max;
        }


        /// <summary>
        ///     The min value (included) of this chunk
        /// </summary>
        public T Min { get; }

        /// <summary>
        ///     The maximum value (included) of this chunk
        /// </summary>
        public T Max { get; }


        /// <inheritdoc />
        public override bool Equals( object obj ) => obj?.GetType() == typeof(Chunk<T>) && Equals( (Chunk<T>)obj );


        /// <summary>
        ///     Compare two <c>Chunk</c>
        /// </summary>
        /// <param name="other">The other <see cref="Chunk{T}" /> to compare with</param>
        /// <returns><c>true</c> if <c>this</c> have the same <see cref="Min" /> and <see cref="Max" />; <c>false</c> otherwise</returns>
        public bool Equals( Chunk<T> other ) => Equals( Min, other.Min ) && Equals( Max, other.Max );


        /// <inheritdoc />
        public override int GetHashCode()
        {
            // Wwe do not care about hash code as we will not used in dictionary...
            return 0;
        }


        /// <inheritdoc />
        public override string ToString() => $"[{Min}..{Max}]";
    }
}