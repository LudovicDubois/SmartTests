using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SmartTests.Criterias;



namespace SmartTests.Ranges
{
    /// <summary>
    ///     Represents a Range of integer values (with several chunks)
    /// </summary>
    public class IntRange
    {
        /// <summary>
        ///     Creates a new Range of integer with no chunk
        /// </summary>
        public IntRange()
        {
            Chunks = new List<Chunk>();
        }


        /// <summary>
        ///     Creates a new Range of integer with only one chunk
        /// </summary>
        /// <param name="min">The min value (included) of the chunk.</param>
        /// <param name="max">The max value (included) of the chunk.</param>
        public IntRange( int min, int max )
        {
            Chunks = new List<Chunk>
                     {
                         new Chunk( min, max )
                     };
        }


        /// <summary>
        ///     The sorted list of non-overlapping chunks
        /// </summary>
        public List<Chunk> Chunks { get; }


        /// <summary>
        ///     Add a chunk to the range
        /// </summary>
        /// <param name="min">The min value (included) of the chunk.</param>
        /// <param name="max">The max value (included) of the chunk.</param>
        /// <returns>Return <c>this</c> so that adding chunks can be chained.</returns>
        public IntRange Add( int min, int max )
        {
            if( min > max )
                throw new ArgumentException( "min should be lower or equal to max" );

            if( Chunks.Count == 0 )
            {
                Chunks.Add( new Chunk( min, max ) );
                return this;
            }

            var lastChunk = Chunks.Last();
            if( min > lastChunk.Max )
            {
                if( min - 1 == lastChunk.Max )
                {
                    // Why not having the right Chunk at first?
                    Chunks.RemoveAt( Chunks.Count - 1 );
                    min = lastChunk.Min;
                }
                Chunks.Add( new Chunk( min, max ) );
                return this;
            }

            // Not in the right order!
            GetLocation( min, out var minInChunk, out var minIndex );
            GetLocation( max, out var maxInChunk, out var maxIndex );

            if( minInChunk )
                // In a chunk
                min = Chunks[ minIndex ].Min;
            if( maxInChunk )
                // In a chunk
                max = Chunks[ maxIndex ].Max;

            if( minInChunk )
            {
                if( maxInChunk )
                    Chunks.RemoveRange( minIndex, maxIndex - minIndex + 1 );
                else
                    Chunks.RemoveRange( minIndex, maxIndex - minIndex );
            }
            else
            {
                if( maxInChunk )
                    Chunks.RemoveRange( minIndex, maxIndex - minIndex + 1 );
                else
                    Chunks.RemoveRange( minIndex, maxIndex - minIndex );
            }
            Chunks.Insert( minIndex, new Chunk( min, max ) );
            return this;
        }


        private void GetLocation( int value, out bool inChunk, out int chunkIndex )
        {
            for( chunkIndex = 0; chunkIndex < Chunks.Count; chunkIndex++ )
            {
                if( value < Chunks[ chunkIndex ].Min )
                {
                    inChunk = value == Chunks[ chunkIndex ].Min - 1;
                    return;
                }
                if( Chunks[ chunkIndex ].Min <= value &&
                    value <= ( Chunks[ chunkIndex ].Max == int.MaxValue ? int.MaxValue : Chunks[ chunkIndex ].Max + 1 ) )
                {
                    inChunk = true;
                    return;
                }
            }
            inChunk = false;
        }


        /// <summary>
        ///     Returns any value for this range (all values have the same probability)
        /// </summary>
        /// <param name="value">A random value within this range.</param>
        /// <returns>Any <see cref="Criteria" /> so that it can be used everywhere a criteria is expected.</returns>
        public Criteria GetValue( out int value )
        {
            // Ensure values are well distributed
            var max = int.MinValue;
            foreach( var chunk in Chunks )
                max += chunk.Max - chunk.Min;
            var random = new Random();

            if( max == int.MaxValue )
            {
                value = random.Next();
                return AnyValue.IsValid;
            }

            value = random.Next( int.MinValue, max );
            max = int.MinValue;
            foreach( var chunk in Chunks )
            {
                var min = max + 1;
                max += chunk.Max - chunk.Min;
                if( value > max )
                    continue;
                value = value - min + chunk.Min;
                return AnyValue.IsValid;
            }

            throw new NotImplementedException();
        }


        /// <summary>
        ///     Add a chunk and return a random value from the new Range
        /// </summary>
        /// <param name="min">The min value (included) of the chunk.</param>
        /// <param name="max">The max value (included) of the chunk.</param>
        /// <param name="value">A random value within this range.</param>
        /// <returns>Any <see cref="Criteria" /> so that it can be used everywhere a criteria is expected.</returns>
        /// <seealso cref="Add(int,int)" />
        /// <seealso cref="GetValue" />
        public Criteria Add( int min, int max, out int value ) => Add( min, max ).GetValue( out value );


        /// <inheritdoc />
        public override bool Equals( object obj ) => Equals( obj as IntRange );


        /// <summary>
        ///     Compare two IntRange
        /// </summary>
        /// <param name="other">The other IntRange to compare with.</param>
        /// <returns>
        ///     <c>true</c> if <c>this</c> and <paramref name="other" /> have the same <see cref="Chunks" />; <c>false</c>
        ///     otherwise
        /// </returns>
        protected bool Equals( IntRange other ) => other?.GetType() == typeof(IntRange) && Equals( Chunks, other.Chunks );


        /// <inheritdoc />
        public override int GetHashCode() => Chunks?.GetHashCode() ?? 0;


        /// <inheritdoc />
        public override string ToString()
        {
            var result = new StringBuilder( "Range" );
            foreach( var chunk in Chunks )
                result.Append( $"({( chunk.Min == int.MinValue ? "int.MinValue" : chunk.Min.ToString() )}, {( chunk.Max == int.MaxValue ? "int.MaxValue" : chunk.Max.ToString() )}).Add" );
            result.Length -= 4;

            return result.ToString();
        }


        /// <summary>
        ///     A chunk of consecutive integer values (from <see cref="Min" /> to <see cref="Max" />)
        /// </summary>
        public struct Chunk
        {
            /// <summary>
            ///     Instantiates a chunk
            /// </summary>
            /// <param name="min">The min value (included) of the chunk.</param>
            /// <param name="max">The max value (included) of the chunk.</param>
            /// <exception cref="ArgumentException">If <paramref name="min" /> &gt; <paramref name="max" /></exception>
            public Chunk( int min, int max )
            {
                if( min > max )
                    throw new ArgumentException( "min should be lower or equal to max" );
                Min = min;
                Max = max;
            }


            /// <summary>
            ///     The min value (included) of this chunk
            /// </summary>
            public int Min { get; }

            /// <summary>
            ///     The maximum value (included) of this chunk
            /// </summary>
            public int Max { get; }


            /// <inheritdoc />
            public override bool Equals( object obj ) => obj?.GetType() == typeof(Chunk) && Equals( (Chunk)obj );


            /// <summary>
            ///     Compare two <c>Chunk</c>
            /// </summary>
            /// <param name="other">The other <see cref="Chunk" /> to compare with</param>
            /// <returns><c>true</c> if <c>this</c> have the same <see cref="Min" /> and <see cref="Max" />; <c>false</c> otherwise</returns>
            public bool Equals( Chunk other ) => Min == other.Min && Max == other.Max;


            /// <inheritdoc />
            public override int GetHashCode()
            {
                unchecked
                {
                    return ( Min * 397 ) ^ Max;
                }
            }


            /// <inheritdoc />
            public override string ToString() => $"[{Min}..{Max}]";
        }
    }
}