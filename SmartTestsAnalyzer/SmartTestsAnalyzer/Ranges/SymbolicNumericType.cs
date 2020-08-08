using System;
using System.Collections.Generic;
using System.Text;

using SmartTests.Ranges;



namespace SmartTestsAnalyzer.Ranges
{
    /// <summary>
    ///     Represents a Range of numeric values (with several chunks)
    /// </summary>
    /// <typeparam name="T">The type of integers for this range</typeparam>
    public abstract class SymbolicNumericType<T>: ISymbolicNumericType<T>
        where T: IComparable<T>
    {
        #region MinValue Property

        SymbolicConstant<T> ISymbolicNumericType<T>.MinValue => MinValue;

        /// <inheritdoc cref="INumericType{T}.MinValue" />
        protected abstract SymbolicConstant<T> MinValue { get; }

        #endregion


        #region MaxValue Property

        SymbolicConstant<T> ISymbolicNumericType<T>.MaxValue => MaxValue;

        /// <inheritdoc cref="INumericType{T}.MaxValue" />
        protected abstract SymbolicConstant<T> MaxValue { get; }

        #endregion


        /// <inheritdoc />
        public List<SymbolicChunk<T>> Chunks { get; } = new List<SymbolicChunk<T>>();


        /// <inheritdoc />
        public void RemoveRange( ISymbolicNumericType<T> range, ISymbolicNumericType<T> errors )
        {
            var order = new SymbolicOrder<T>( range.MinValue, range.MaxValue );
            order.AddRange( Chunks );
            order.AddRange( range.Chunks );
            range.Chunks.ForEach( chunk => RemoveRange( order, chunk, errors ) );
        }


        /// <summary>
        ///     Remove
        ///     <param name="remove"></param>
        ///     from the current Range.
        /// </summary>
        /// <param name="order">The Partial Order of SymbolicConstants.</param>
        /// <param name="remove"></param>
        /// <param name="errors">The values removed several times.</param>
        /// <returns>The Range of the errors (removed multiple times)</returns>
        private void RemoveRange( SymbolicOrder<T> order, SymbolicChunk<T> remove, ISymbolicNumericType<T> errors )
        {
            var comparable = false;
            foreach( var source in Chunks.ToArray() )
            {
                var beforeSource = order.LessThan( remove.Max, source.Min );
                if( beforeSource == true )
                    // Remove before Source
                    // => do nothing
                    continue;

                var afterSource = order.GreaterThan( remove.Min, source.Max );
                if( afterSource == true )
                    // Remove after Source
                    // => Do nothing
                    continue;

                if( beforeSource == null || afterSource == null )
                    // Unable to compare
                    continue;

                comparable = true;
                Chunks.Remove( source );
                var (sourceMin, sourceMinIncluded, sourceMax, sourceMaxIncluded) = source;
                var (removeMin, removeMinIncluded, removeMax, removeMaxIncluded) = remove;
                if( order.LessThan( removeMin, sourceMin ) == true && order.LessThan( sourceMin, removeMax ) == true )
                {
                    // Remove starts too low
                    errors.Range( removeMin, removeMinIncluded, sourceMin, !sourceMinIncluded );
                    removeMin = sourceMin;
                    removeMaxIncluded = sourceMinIncluded;
                }

                if( order.LessThan( removeMin, sourceMax ) == true && order.LessThan( sourceMax, removeMax ) == true )
                {
                    // Remove ends too high
                    errors.Range( sourceMax, !sourceMaxIncluded, removeMax, removeMaxIncluded );
                    removeMax = sourceMax;
                    removeMaxIncluded = sourceMaxIncluded;
                }

                if( removeMin.Equals( sourceMin ) )
                {
                    if( removeMinIncluded != sourceMinIncluded )
                    {
                        if( removeMinIncluded )
                            errors.Value( removeMin );
                        else
                            Value( removeMin );
                        removeMinIncluded = false;
                    }

                    sourceMin = removeMax;
                    sourceMinIncluded = !removeMaxIncluded;
                }

                if( removeMax.Equals( sourceMax ) )
                {
                    if( removeMaxIncluded != sourceMaxIncluded )
                    {
                        if( removeMaxIncluded )
                            errors.Value( sourceMax );
                        else
                            Value( sourceMax );
                        removeMaxIncluded = false;
                    }

                    sourceMax = removeMin;
                    sourceMaxIncluded = !removeMinIncluded;
                }

                if( order.LessThan( sourceMin, removeMin ) == true &&
                    order.LessThan( removeMin, sourceMax ) == true &&
                    !sourceMin.Equals( removeMax ) ||
                    !removeMin.Equals( sourceMax ) &&
                    order.LessThan( sourceMin, removeMax ) == true &&
                    order.LessThan( removeMax, sourceMax ) == true )
                {
                    Range( sourceMin, sourceMinIncluded, removeMin, !removeMinIncluded );
                    Range( removeMax, !removeMaxIncluded, sourceMax, sourceMaxIncluded );
                }
                else
                {
                    if( order.LessThan( sourceMin, sourceMax ) != false ||
                        sourceMinIncluded && sourceMaxIncluded )
                        Range( sourceMin, sourceMinIncluded, sourceMax, sourceMaxIncluded );
                }
            }

            if( !comparable )
            {
                // Add ?..remove.Min and remove.Max..?
                Chunks.Clear();
                Chunks.Add( new SymbolicChunk<T>( new SymbolicConstant<T>(), remove.Min, true, !remove.MinIncluded ) );
                Chunks.Add( new SymbolicChunk<T>( remove.Max, new SymbolicConstant<T>(), !remove.MaxIncluded, true ) );
            }
        }


        /// <inheritdoc />
        public ISymbolicNumericType<T> Range( SymbolicConstant<T> min, bool minIncluded, SymbolicConstant<T> max, bool maxIncluded )
        {
            if( min.ConstantGreaterThan( max ) )
                throw new ArgumentException( "min should be lower or equal to max" );
            // Should not be the min for any other Chunk
            if( min.Symbolic )
                Chunks.ForEach( chunk =>
                                {
                                    if( chunk.Min.Equals( min ) )
                                        throw new ArgumentException( "min already declared" );
                                } );

            // Should not be the min for any other Chunk
            if( max.Symbolic )
                Chunks.ForEach( chunk =>
                                {
                                    if( chunk.Max.Equals( max ) )
                                        throw new ArgumentException( "max already declared" );
                                } );

            Chunks.Add( new SymbolicChunk<T>( min, max, minIncluded, maxIncluded ) );
            return this;
        }


        /// <inheritdoc />
        public ISymbolicNumericType<T> Range( SymbolicConstant<T> min, SymbolicConstant<T> max ) => Range( min, true, max, true );


        /// <inheritdoc />
        public ISymbolicNumericType<T> AboveOrEqual( SymbolicConstant<T> min ) => Range( min, MaxValue );


        /// <inheritdoc />
        public ISymbolicNumericType<T> Above( SymbolicConstant<T> min ) => Range( min, false, MaxValue, true );


        /// <inheritdoc />
        public ISymbolicNumericType<T> BelowOrEqual( SymbolicConstant<T> max ) => Range( MinValue, max );


        /// <inheritdoc />
        public ISymbolicNumericType<T> Below( SymbolicConstant<T> max ) => Range( MinValue, true, max, false );


        /// <inheritdoc />
        public ISymbolicNumericType<T> Value( SymbolicConstant<T> value ) => Range( value, value );


        /*
        /// <summary>
        ///     Returns the numeric value as a string.
        /// </summary>
        /// <param name="value">The value to convert to a string.</param>
        /// <returns>The converted value as a string, using MinValue and MaxValue as needed.</returns>
        protected abstract string ToString( T value );
        */
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
                if( chunk.Min.Equals( chunk.Max ) &&
                    chunk.MinIncluded && chunk.MaxIncluded )
                {
                    result.Append( $".Value({chunk.Min})" );
                    continue;
                }

                if( chunk.MinIncluded && chunk.Min.Equals( MinValue ) )
                {
                    if( chunk.MaxIncluded && chunk.Max.Equals( MaxValue ) )
                    {
                        // Full range
                        result.Append( $".Range({MinValue}, {MaxValue})" );
                        continue;
                    }

                    // Below
                    result.Append( chunk.MaxIncluded
                                       ? $".BelowOrEqual({chunk.Max})"
                                       : $".Below({chunk.Max})" );
                    continue;
                }

                if( chunk.MaxIncluded && chunk.Max.Equals( MaxValue ) )
                {
                    // Above
                    result.Append( chunk.MinIncluded
                                       ? $".AboveOrEqual({chunk.Min})"
                                       : $".Above({chunk.Min})" );
                    continue;
                }

                // Sub range
                if( chunk.MinIncluded && chunk.MaxIncluded )
                {
                    result.Append( $".Range({chunk.Min}, {chunk.Max})" );
                    continue;
                }

                result.Append( $".Range({chunk.Min}, {ToString( chunk.MinIncluded )}, {chunk.Max}, {ToString( chunk.MaxIncluded )})" );
            }

            return result.ToString();
        }
    }
}