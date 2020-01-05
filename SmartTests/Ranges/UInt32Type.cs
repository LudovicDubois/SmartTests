using System;
using System.Linq;

using SmartTests.Criterias;
using SmartTests.Helpers;

// ReSharper disable UnusedMember.Global


namespace SmartTests.Ranges
{
    /// <summary>
    ///     Represents a Range of integer values (with several chunks)
    /// </summary>
    public class UInt32Type: NumericType<uint, UInt32Type>
    {
        /// <inheritdoc />
        protected override uint MinValue => uint.MinValue;
        /// <inheritdoc />
        protected override uint MaxValue => uint.MaxValue;


        /// <inheritdoc />
        protected override uint GetPrevious( uint n ) => n - 1;


        /// <inheritdoc />
        protected override uint GetNext( uint n ) => n + 1;


        /// <inheritdoc />
        public override Criteria GetValidValue( out uint value, params uint[] avoidedValues )
        {
            // Ensure values are well distributed
            var max = MinValue;
            foreach( var chunk in Chunks )
                max += chunk.IncludedMax - chunk.IncludedMin + 1; // +1 because both are included

            var random = new Random();
            if( max == MaxValue )
                while( true )
                {
                    value = random.NextUInt32();
                    if( !avoidedValues.Contains( value ) )
                        return AnyValue.IsValid;
                }

            while( true )
            {
                var val = random.NextUInt32( MinValue, max );

                max = MinValue;
                foreach( var chunk in Chunks )
                {
                    var min = max;
                    max += chunk.IncludedMax - chunk.IncludedMin + 1;
                    if( val >= max )
                        continue;
                    value = val - min + chunk.IncludedMin;
                    if( !avoidedValues.Contains( value ) )
                        return AnyValue.IsValid;
                }
            }
        }


        /// <inheritdoc />
        protected override string ToString( uint value )
        {
            if( value == uint.MaxValue )
                return "uint.MaxValue";
            return value.ToString();
        }


        /// <inheritdoc />
        public override string ToString() => ToString( "UInt32Range" );
    }


    /// <summary>
    ///     A helper type to create <see cref="INumericType{T}" /> for uint from a uint.
    /// </summary>
    public static class UInt32TypeHelper
    {
        /// <summary>
        ///     Adds a chunk of numeric values
        /// </summary>
        /// <param name="_">A uint we do not care about, except to know to create a <see cref="INumericType{T}" /> for uint.</param>
        /// <param name="min">The min value (included) of the chunk.</param>
        /// <param name="max">The max value (included) of the chunk.</param>
        /// <returns>Return a new <see cref="INumericType{T}" /> for uint so that adding chunks can be chained.</returns>
        // ReSharper disable once UnusedParameter.Global
        public static INumericType<uint> Range( this uint _, uint min, uint max ) => SmartTest.UInt32Range.Range( min, max );


        /// <summary>
        ///     Adds a chunk of numeric values
        /// </summary>
        /// <param name="_">A uint we do not care about, except to know to create a <see cref="INumericType{T}" /> for uint.</param>
        /// <param name="min">The min value (included) of the chunk.</param>
        /// <param name="minIncluded"><c>true</c> to include min, <c>false</c> otherwise.</param>
        /// <param name="max">The max value (included) of the chunk.</param>
        /// <param name="maxIncluded"><c>true</c> to include max, <c>false</c> otherwise.</param>
        /// <returns>Return a new <see cref="INumericType{T}" /> for uint so that adding chunks can be chained.</returns>
        // ReSharper disable once UnusedParameter.Global
        public static INumericType<uint> Range( this uint _, uint min, bool minIncluded, uint max, bool maxIncluded ) => SmartTest.UInt32Range.Range( min, minIncluded, max, maxIncluded );


        /// <summary>
        ///     Adds a chunk of numeric values above a min
        /// </summary>
        /// <param name="_">A uint we do not care about, except to know to create a <see cref="INumericType{T}" /> for uint.</param>
        /// <param name="min">The min value (included) of the created chunk.</param>
        /// <returns>Return a new <see cref="INumericType{T}" /> for uint so that adding chunks can be chained.</returns>
        // ReSharper disable once UnusedParameter.Global
        public static INumericType<uint> AboveOrEqual( this uint _, uint min ) => SmartTest.UInt32Range.AboveOrEqual( min );


        /// <summary>
        ///     Adds a chunk of numeric values above min
        /// </summary>
        /// <param name="_">A uint we do not care about, except to know to create a <see cref="INumericType{T}" /> for uint.</param>
        /// <param name="min">The min value (excluded) of the created chunk.</param>
        /// <returns>Return a new <see cref="INumericType{T}" /> for uint so that adding chunks can be chained.</returns>
        // ReSharper disable once UnusedParameter.Global
        public static INumericType<uint> Above( this uint _, uint min ) => SmartTest.UInt32Range.Above( min );


        /// <summary>
        ///     Adds a chunk of numeric values below or equal to max
        /// </summary>
        /// <param name="_">A uint we do not care about, except to know to create a <see cref="INumericType{T}" /> for uint.</param>
        /// <param name="max">The max value (included) of the range.</param>
        /// <returns>Return a new <see cref="INumericType{T}" /> for uint so that adding chunks can be chained.</returns>
        // ReSharper disable once UnusedParameter.Global
        public static INumericType<uint> BelowOrEqual( this uint _, uint max ) => SmartTest.UInt32Range.BelowOrEqual( max );


        /// <summary>
        ///     Adds a chunk of numeric values below max
        /// </summary>
        /// <param name="_">A uint we do not care about, except to know to create a <see cref="INumericType{T}" /> for uint.</param>
        /// <param name="max">The max value (included) of the range.</param>
        /// <returns>Return a new <see cref="INumericType{T}" /> for uint so that adding chunks can be chained.</returns>
        // ReSharper disable once UnusedParameter.Global
        public static INumericType<uint> Below( this uint _, uint max ) => SmartTest.UInt32Range.Below( max );


        /// <summary>
        ///     Adds a chunk of one numeric value
        /// </summary>
        /// <param name="_">A uint we do not care about, except to know to create a <see cref="INumericType{T}" /> for uint.</param>
        /// <param name="value">A random value within _ range.</param>
        /// <returns>Return a new <see cref="INumericType{T}" /> for uint so that adding chunks can be chained.</returns>
        // ReSharper disable once UnusedParameter.Global
        public static INumericType<uint> Value( this uint _, uint value ) => SmartTest.UInt32Range.Value( value );
    }
}