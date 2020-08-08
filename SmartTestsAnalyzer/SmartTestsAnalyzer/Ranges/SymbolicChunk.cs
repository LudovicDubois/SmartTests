using System;
using System.Text;



namespace SmartTestsAnalyzer.Ranges
{
    /// <summary>
    ///     A chunk of consecutive integer values (from <see cref="Min" /> to <see cref="Max" />)
    /// </summary>
    /// <typeparam name="T">The numeric type for which a chunk is constructed.</typeparam>
    public readonly struct SymbolicChunk<T>: IEquatable<SymbolicChunk<T>>
        where T: IComparable<T>
    {
        /// <summary>
        ///     Instantiates a chunk
        /// </summary>
        /// <param name="min">The min value (included) of the chunk.</param>
        /// <param name="max">The max value (included) of the chunk.</param>
        /// <param name="minIncluded">
        ///     The minimum value included in this chunk. This is <paramref name="min" /> when <paramref name="min" /> is included,
        ///     the smallest value over <paramref name="min" /> otherwise.
        /// </param>
        /// <param name="maxIncluded">
        ///     The maximum value included in this chunk. This is <paramref name="max" /> when <paramref name="max" /> is included,
        ///     the smallest value below <paramref name="max" /> otherwise.
        /// </param>
        public SymbolicChunk( SymbolicConstant<T> min, SymbolicConstant<T> max, bool minIncluded, bool maxIncluded )
        {
            Min = min;
            Max = max;
            MinIncluded = minIncluded;
            MaxIncluded = maxIncluded;
        }


        /// <summary>
        ///     The min value (included) of this chunk
        /// </summary>
        public SymbolicConstant<T> Min { get; }


        /// <summary>
        ///     The maximum value (included) of this chunk
        /// </summary>
        public SymbolicConstant<T> Max { get; }


        /// <summary>
        ///     Indicates whether <see cref="Min" /> is included or not in the Range.
        /// </summary>
        public bool MinIncluded { get; }
        /// <summary>
        ///     Indicates whether <see cref="Max" /> is included or not in the Range.
        /// </summary>
        public bool MaxIncluded { get; }


        public void Deconstruct( out SymbolicConstant<T> min, out bool minIncluded, out SymbolicConstant<T> max, out bool maxIncluded )
        {
            min = Min;
            minIncluded = MinIncluded;
            max = Max;
            maxIncluded = MaxIncluded;
        }


        /// <inheritdoc />
        public override bool Equals( object obj ) => obj?.GetType() == typeof(SymbolicChunk<T>) && Equals( (SymbolicChunk<T>)obj );


        /// <summary>
        ///     Compare two <c>Chunk</c>
        /// </summary>
        /// <param name="other">The other <see cref="SymbolicChunk{T}" /> to compare with</param>
        /// <returns><c>true</c> if <c>this</c> have the same <see cref="Min" /> and <see cref="Max" />; <c>false</c> otherwise</returns>
        // ReSharper disable once MemberCanBePrivate.Global
        public bool Equals( SymbolicChunk<T> other ) => Equals( MinIncluded, other.MinIncluded ) && Equals( MaxIncluded, other.MaxIncluded );


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
            result.Append( MinIncluded ? '[' : ')' );
            result.Append( Min );
            result.Append( ".." );
            result.Append( Max );
            result.Append( MaxIncluded ? ']' : '(' );
            return result.ToString();
        }
    }
}