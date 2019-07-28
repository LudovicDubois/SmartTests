using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;



namespace SmartTests.Ranges
{
    /// <summary>
    ///     Represents a Range of numeric values (with several chunks)
    /// </summary>
    /// <typeparam name="T">The type of integers for this range</typeparam>
    /// <typeparam name="TType">The explicit Range Type (should be itself in sub-classes)</typeparam>
    public abstract class NumericType<T, TType>: INumericType<T>
        where T: IComparable<T>
        where TType: class, INumericType<T>
    {
        #region MinValue Property

        T INumericType<T>.MinValue => MinValue;

        /// <inheritdoc cref="INumericType{T}.MinValue" />
        protected abstract T MinValue { get; }

        #endregion


        #region MaxValue Property

        T INumericType<T>.MaxValue => MaxValue;

        /// <inheritdoc cref="INumericType{T}.MaxValue" />
        protected abstract T MaxValue { get; }

        #endregion


        #region GetPrevious Method

        T INumericType<T>.GetPrevious( T n ) => GetPrevious( n );


        /// <inheritdoc cref="INumericType{T}.GetPrevious(T)" />
        protected abstract T GetPrevious( T n );

        #endregion


        #region GetNext Method

        T INumericType<T>.GetNext( T n ) => GetNext( n );


        /// <inheritdoc cref="INumericType{T}.GetNext(T)" />
        protected abstract T GetNext( T n );

        #endregion


        /// <inheritdoc />
        public List<Chunk<T>> Chunks { get; } = new List<Chunk<T>>();


        /// <inheritdoc />
        public INumericType<T> Range( T min, bool minIncluded, T max, bool maxIncluded )
        {
            var includedMin = minIncluded ? min : GetNext( min );
            var includedMax = maxIncluded ? max : GetPrevious( max );

            if( includedMin.CompareTo( includedMax ) > 0 )
                throw new ArgumentException( "min should be lower or equal to max" );

            if( Chunks.Count == 0 )
            {
                Chunks.Add( new Chunk<T>( min, max, includedMin, includedMax ) );
                return this;
            }

            var lastChunk = Chunks.Last();
            if( includedMin.CompareTo( lastChunk.IncludedMax ) > 0 )
            {
                if( GetPrevious( includedMin ).CompareTo( lastChunk.IncludedMax ) == 0 )
                {
                    // Why not having the right Chunk at first?
                    Chunks.RemoveAt( Chunks.Count - 1 );
                    min = lastChunk.Min;
                    includedMin = lastChunk.IncludedMin;
                }

                Chunks.Add( new Chunk<T>( min, max, includedMin, includedMax ) );
                return this;
            }

            // Not in the right order!
            GetMinLocation( includedMin, out var minInChunk, out var minIndex );
            GetMaxLocation( includedMax, out var maxInChunk, out var maxIndex );

            if( minInChunk )
            {
                // In a chunk
                min = Chunks[ minIndex ].Min;
                includedMin = Chunks[ minIndex ].IncludedMin;
            }

            if( maxInChunk )
            {
                // In a chunk
                max = Chunks[ maxIndex ].Max;
                includedMax = Chunks[ maxIndex ].IncludedMax;
            }


            if( minInChunk )
            {
                if( maxInChunk )
                    Chunks.RemoveRange( minIndex, maxIndex - minIndex + 1 );
                else
                    Chunks.RemoveRange( minIndex, maxIndex - minIndex + 1 );
            }
            else
            {
                if( maxInChunk )
                    Chunks.RemoveRange( minIndex, maxIndex - minIndex + 1 );
                else
                    Chunks.RemoveRange( minIndex, maxIndex - minIndex + 1 );
            }

            Chunks.Insert( minIndex, new Chunk<T>( min, max, includedMin, includedMax ) );
            return this;
        }


        /// <inheritdoc />
        public INumericType<T> Range( T min, T max ) => Range( min, true, max, true );


        /// <inheritdoc />
        public Criteria Range( T min, T max, out T value, params T[] avoidedValues ) => Range( min, max ).GetValidValue( out value, avoidedValues );


        /// <inheritdoc />
        public Criteria Range( T min, bool minIncluded, T max, bool maxIncluded, out T value, params T[] avoidedValues ) => Range( min, minIncluded, max, maxIncluded ).GetValidValue( out value, avoidedValues );


        /// <inheritdoc />
        public INumericType<T> AboveOrEqual( T min ) => Range( min, MaxValue );


        /// <inheritdoc />
        public Criteria AboveOrEqual( T min, out T value ) => Range( min, MaxValue, out value );


        /// <inheritdoc />
        public INumericType<T> Above( T min ) => Range( min, false, MaxValue, true );


        /// <inheritdoc />
        public Criteria Above( T min, out T value ) => Range( min, false, MaxValue, true, out value );


        /// <inheritdoc />
        public INumericType<T> BelowOrEqual( T max ) => Range( MinValue, max );


        /// <inheritdoc />
        public Criteria BelowOrEqual( T max, out T value ) => Range( MinValue, max, out value );


        /// <inheritdoc />
        public INumericType<T> Below( T max ) => Range( MinValue, true, max, false );


        /// <inheritdoc />
        public Criteria Below( T max, out T value ) => Range( MinValue, true, max, false, out value );


        /// <inheritdoc />
        public INumericType<T> Value( T value ) => Range( value, value );


        /// <inheritdoc />
        public Criteria Value( T val, out T value ) => Range( val, val, out value );


        private void GetMinLocation( T value, out bool inChunk, out int chunkIndex )
        {
            for( chunkIndex = 0; chunkIndex < Chunks.Count; chunkIndex++ )
            {
                if( value.CompareTo( Chunks[ chunkIndex ].IncludedMin ) < 0 )
                    break;

                if( Chunks[ chunkIndex ].IncludedMin.CompareTo( value ) <= 0 &&
                    value.CompareTo( Chunks[ chunkIndex ].IncludedMax.CompareTo( MaxValue ) == 0
                                         ? MaxValue
                                         : GetNext( Chunks[ chunkIndex ].IncludedMax ) ) <= 0 )
                {
                    inChunk = true;
                    return;
                }
            }

            inChunk = false;
        }


        private void GetMaxLocation( T value, out bool inChunk, out int chunkIndex )
        {
            for( chunkIndex = Chunks.Count - 1; chunkIndex >= 0; chunkIndex-- )
            {
                if( value.CompareTo( Chunks[ chunkIndex ].IncludedMax ) >= 0 )
                    break;

                if( Chunks[ chunkIndex ].IncludedMax.CompareTo( value ) > 0 &&
                    value.CompareTo( Chunks[ chunkIndex ].IncludedMin.CompareTo( MinValue ) == 0
                                         ? MinValue
                                         : GetPrevious( Chunks[ chunkIndex ].IncludedMin ) ) >= 0 )
                {
                    inChunk = true;
                    return;
                }
            }

            inChunk = false;
        }


        /// <inheritdoc />
        public abstract Criteria GetValidValue( out T value, params T[] avoidedValues );


        /// <inheritdoc />
        public Criteria GetErrorValue( out T value ) => GetValidValue( out value );


        /// <inheritdoc />
        public override bool Equals( object obj ) => Equals( obj as TType );


        /// <summary>
        ///     Compare two NumericType
        /// </summary>
        /// <param name="other">The other NumericType to compare to.</param>
        /// <returns>
        ///     <c>true</c> if <c>this</c> and <paramref name="other" /> have the same <see cref="Chunks" />; <c>false</c>
        ///     otherwise
        /// </returns>
        protected bool Equals( TType other ) => other?.GetType() == typeof(TType) && Equals( Chunks, other.Chunks );


        /// <inheritdoc />
        public override int GetHashCode() => Chunks?.GetHashCode() ?? 0;


        /// <summary>
        ///     Returns the numeric value as a string.
        /// </summary>
        /// <param name="value">The value to convert to a string.</param>
        /// <returns>The converted value as a string, using MinValue and MaxValue as needed.</returns>
        protected abstract string ToString( T value );


        private static string ToString( bool value ) => value ? "true" : "false";


        /// <summary>
        ///     Computes the string representing this range, when its type as a string is provided
        /// </summary>
        /// <param name="type">The type name represented by this instance.</param>
        /// <returns>The string representation of this instance.</returns>
        protected string ToString( string type )
        {
            var result = new StringBuilder( type );
            foreach( var chunk in Chunks )
            {
                if( chunk.IncludedMin.CompareTo( chunk.IncludedMax ) == 0 )
                {
                    result.Append( $".Value({ToString( chunk.IncludedMin )})" );
                    continue;
                }

                if( chunk.IncludedMin.CompareTo( MinValue ) == 0 )
                {
                    if( chunk.IncludedMax.CompareTo( MaxValue ) == 0 )
                    {
                        // Full range
                        result.Append( $".Range({ToString( MinValue )}, {ToString( MaxValue )})" );
                        continue;
                    }

                    // Below
                    result.Append( chunk.MaxIncluded
                                       ? $".BelowOrEqual({ToString( chunk.Max )})"
                                       : $".Below({ToString( chunk.Max )})" );
                    continue;
                }

                if( chunk.IncludedMax.CompareTo( MaxValue ) == 0 )
                {
                    // Above
                    result.Append( chunk.MinIncluded
                                       ? $".AboveOrEqual({ToString( chunk.Min )})"
                                       : $".Above({ToString( chunk.Min )})" );
                    continue;
                }

                // Sub range
                if( chunk.MinIncluded && chunk.MaxIncluded )
                {
                    result.Append( $".Range({ToString( chunk.Min )}, {ToString( chunk.Max )})" );
                    continue;
                }

                result.Append( $".Range({ToString( chunk.Min )}, {ToString( chunk.MinIncluded )}, {ToString( chunk.Max )}, {ToString( chunk.MaxIncluded )})" );
            }

            return result.ToString();
        }
    }
}