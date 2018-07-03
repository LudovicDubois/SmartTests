using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SmartTests.Criterias;
using SmartTests.Helpers;



namespace SmartTests.Ranges
{
    /// <summary>
    ///     Represents a Range of long values (with several chunks)
    /// </summary>
    public class LongType: IType<long>
    {
        long IType<long>.MinValue => long.MinValue;
        long IType<long>.MaxValue => long.MaxValue;

        long IType<long>.GetPrevious( long n ) => n - 1;
        long IType<long>.GetNext( long n ) => n + 1;


        /// <inheritdoc />
        public List<Chunk<long>> Chunks { get; } = new List<Chunk<long>>();


        /// <inheritdoc />
        public IType<long> Range( long min, long max )
        {
            if( min > max )
                throw new ArgumentException( "min should be lower or equal to max" );

            if( Chunks.Count == 0 )
            {
                Chunks.Add( new Chunk<long>( min, max ) );
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

                Chunks.Add( new Chunk<long>( min, max ) );
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

            Chunks.Insert( minIndex, new Chunk<long>( min, max ) );
            return this;
        }


        private void GetLocation( long value, out bool inChunk, out int chunkIndex )
        {
            for( chunkIndex = 0; chunkIndex < Chunks.Count; chunkIndex++ )
            {
                if( value < Chunks[ chunkIndex ].Min )
                {
                    inChunk = value == Chunks[ chunkIndex ].Min - 1;
                    return;
                }

                if( Chunks[ chunkIndex ].Min <= value &&
                    value <= ( Chunks[ chunkIndex ].Max == long.MaxValue ? long.MaxValue : Chunks[ chunkIndex ].Max + 1 ) )
                {
                    inChunk = true;
                    return;
                }
            }

            inChunk = false;
        }


        /// <inheritdoc />
        public Criteria GetValue( out long value )
        {
            // Ensure values are well distributed
            var max = long.MinValue;
            foreach( var chunk in Chunks )
                max += chunk.Max - chunk.Min;
            var random = new Random();

            if( max == long.MaxValue )
            {
                value = random.Next();
                return AnyValue.IsValid;
            }

            value = random.NextLong( long.MinValue, max );
            max = long.MinValue;
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


        /// <inheritdoc />
        public override bool Equals( object obj ) => Equals( obj as LongType );


        /// <summary>
        ///     Compare two IntRange
        /// </summary>
        /// <param name="other">The other IntRange to compare with.</param>
        /// <returns>
        ///     <c>true</c> if <c>this</c> and <paramref name="other" /> have the same <see cref="Chunks" />; <c>false</c>
        ///     otherwise
        /// </returns>
        protected bool Equals( LongType other ) => other?.GetType() == typeof(LongType) && Equals( Chunks, other.Chunks );


        /// <inheritdoc />
        public override int GetHashCode() => Chunks?.GetHashCode() ?? 0;


        /// <inheritdoc />
        public override string ToString()
        {
            var result = new StringBuilder( "Long" );
            foreach( var chunk in Chunks )
                result.Append( $".Range({( chunk.Min == long.MinValue ? "long.MinValue" : chunk.Min.ToString() )}, {( chunk.Max == long.MaxValue ? "long.MaxValue" : chunk.Max.ToString() )})" );
            return result.ToString();
        }
    }
}