using System;
using System.Text;



namespace SmartTests.Ranges
{
    /// <summary>
    ///     A chunk of consecutive integer values (from <see cref="Min" /> to <see cref="Max" />)
    /// </summary>
    /// <typeparam name="T">The numeric type for which a chunk is constructed.</typeparam>
    public struct Chunk<T>
        where T: IComparable<T>
    {
        /// <summary>
        ///     Instantiates a chunk
        /// </summary>
        /// <param name="min">The min value (included) of the chunk.</param>
        /// <param name="max">The max value (included) of the chunk.</param>
        /// <param name="includedMin">
        ///     The minimum value included in this chunk. This is <paramref name="min" /> when <paramref name="min" /> is included,
        ///     the smallest value over <paramref name="min" /> otherwise.
        /// </param>
        /// <param name="includedMax">
        ///     The maximum value included in this chunk. This is <paramref name="max" /> when <paramref name="max" /> is included,
        ///     the smallest value below <paramref name="max" /> otherwise.
        /// </param>
        public Chunk( T min, T max, T includedMin, T includedMax )
        {
            Min = min;
            Max = max;
            IncludedMin = includedMin;
            IncludedMax = includedMax;
        }


        /// <summary>
        ///     The min value (included) of this chunk
        /// </summary>
        public T Min { get; }


        /// <summary>
        ///     The maximum value (included) of this chunk
        /// </summary>
        public T Max { get; }


        /// <summary>
        ///     The minimum value included in this chunk. This is <see cref="Min" /> when <see cref="Min" /> is included,
        ///     the smallest value over <see cref="Min" /> otherwise.
        /// </summary>
        public T IncludedMin { get; }
        /// <summary>
        ///     The maximum value included in this chunk. This is <see cref="Max" /> when <see cref="Max" /> is included,
        ///     the smallest value below <see cref="Max" /> otherwise.
        /// </summary>
        public T IncludedMax { get; }

        /// <summary>
        ///     Indicate if <see cref="Min" /> is included in this chunk.
        /// </summary>
        public bool MinIncluded => Min.CompareTo( IncludedMin ) == 0;

        /// <summary>
        ///     Indicate if <see cref="Min" /> is included in this chunk.
        /// </summary>
        public bool MaxIncluded => Max.CompareTo( IncludedMax ) == 0;


        /// <inheritdoc />
        public override bool Equals( object obj ) => obj?.GetType() == typeof(Chunk<T>) && Equals( (Chunk<T>)obj );


        /// <summary>
        ///     Compare two <c>Chunk</c>
        /// </summary>
        /// <param name="other">The other <see cref="Chunk{T}" /> to compare with</param>
        /// <returns><c>true</c> if <c>this</c> have the same <see cref="Min" /> and <see cref="Max" />; <c>false</c> otherwise</returns>
        // ReSharper disable once MemberCanBePrivate.Global
        public bool Equals( Chunk<T> other ) => Equals( IncludedMin, other.IncludedMin ) && Equals( IncludedMax, other.IncludedMax );


        /// <inheritdoc />
        public override int GetHashCode()
        {
            // Wwe do not care about hash code as we will not used them in dictionary...
            return 0;
        }


        /// <inheritdoc />
        public override string ToString()
        {
            var result = new StringBuilder();
            result.Append( Min.CompareTo( IncludedMin ) == 0 ? '[' : ')' );
            result.Append( Min );
            result.Append( ".." );
            result.Append( Max );
            result.Append( Max.CompareTo( IncludedMax ) == 0 ? ']' : '(' );
            return result.ToString();
        }
    }
}