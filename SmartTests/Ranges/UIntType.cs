using System;

using SmartTests.Criterias;
using SmartTests.Helpers;



namespace SmartTests.Ranges
{
    /// <summary>
    ///     Represents a Range of integer values (with several chunks)
    /// </summary>
    public class UIntType: NumericType<uint, UIntType>
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
        public override Criteria GetValidValue( out uint value )
        {
            // Ensure values are well distributed
            var max = MinValue;
            foreach( var chunk in Chunks )
                max += chunk.IncludedMax - chunk.IncludedMin + 1; // +1 because both are included

            var random = new Random();
            value = (uint)random.NextLong( MinValue, max );
            if( max == MaxValue )
                return AnyValue.IsValid;

            max = uint.MinValue;
            foreach( var chunk in Chunks )
            {
                var min = max;
                max += chunk.IncludedMax - chunk.IncludedMin + 1;
                if( value >= max )
                    continue;
                value = value - min + chunk.IncludedMin;
                return AnyValue.IsValid;
            }

            throw new NotImplementedException();
        }


        /// <inheritdoc />
        protected override string ToString( uint value )
        {
            if( value == uint.MaxValue )
                return "uint.MaxValue";
            return value.ToString();
        }


        /// <inheritdoc />
        public override string ToString() => ToString( "UInt" );
    }


    /// <summary>
    ///     A helper type to create <see cref="INumericType{T}" /> for uint from a uint.
    /// </summary>
    public static class UIntTypeHelper
    {
        /// <summary>
        ///     Adds a chunk of numeric values
        /// </summary>
        /// <param name="this">A uint we do not care about, except to know to create a <see cref="INumericType{T}" /> for uint.</param>
        /// <param name="min">The min value (included) of the chunk.</param>
        /// <param name="max">The max value (included) of the chunk.</param>
        /// <returns>Return a new <see cref="INumericType{T}" /> for uint so that adding chunks can be chained.</returns>
        public static INumericType<uint> Range( this uint @this, uint min, uint max ) => SmartTest.UInt.Range( min, max );


        /// <summary>
        ///     Adds a chunk of numeric values
        /// </summary>
        /// <param name="this">A uint we do not care about, except to know to create a <see cref="INumericType{T}" /> for uint.</param>
        /// <param name="min">The min value (included) of the chunk.</param>
        /// <param name="minIncluded"><c>true</c> to include min, <c>false</c> otherwise.</param>
        /// <param name="max">The max value (included) of the chunk.</param>
        /// <param name="maxIncluded"><c>true</c> to include max, <c>false</c> otherwise.</param>
        /// <returns>Return a new <see cref="INumericType{T}" /> for uint so that adding chunks can be chained.</returns>
        public static INumericType<uint> Range( this uint @this, uint min, bool minIncluded, uint max, bool maxIncluded ) => SmartTest.UInt.Range( min, minIncluded, max, maxIncluded );


        /// <summary>
        ///     Adds a chunk of numeric values above a min
        /// </summary>
        /// <param name="this">A uint we do not care about, except to know to create a <see cref="INumericType{T}" /> for uint.</param>
        /// <param name="min">The min value (included) of the created chunk.</param>
        /// <returns>Return a new <see cref="INumericType{T}" /> for uint so that adding chunks can be chained.</returns>
        public static INumericType<uint> AboveOrEqual( this uint @this, uint min ) => SmartTest.UInt.AboveOrEqual( min );


        /// <summary>
        ///     Adds a chunk of numeric values above max
        /// </summary>
        /// <param name="this">A uint we do not care about, except to know to create a <see cref="INumericType{T}" /> for uint.</param>
        /// <param name="min">The min value (excluded) of the created chunk.</param>
        /// <returns>Return a new <see cref="INumericType{T}" /> for uint so that adding chunks can be chained.</returns>
        public static INumericType<uint> Above( this uint @this, uint min ) => SmartTest.UInt.Above( min );


        /// <summary>
        ///     Adds a chunk of numeric values below or equal to min
        /// </summary>
        /// <param name="this">A uint we do not care about, except to know to create a <see cref="INumericType{T}" /> for uint.</param>
        /// <param name="max">The max value (included) of the range.</param>
        /// <returns>Return a new <see cref="INumericType{T}" /> for uint so that adding chunks can be chained.</returns>
        public static INumericType<uint> BelowOrEqual( this uint @this, uint max ) => SmartTest.UInt.BelowOrEqual( max );


        /// <summary>
        ///     Adds a chunk of numeric values below or equal to min
        /// </summary>
        /// <param name="this">A uint we do not care about, except to know to create a <see cref="INumericType{T}" /> for uint.</param>
        /// <param name="max">The max value (included) of the range.</param>
        /// <returns>Return a new <see cref="INumericType{T}" /> for uint so that adding chunks can be chained.</returns>
        public static INumericType<uint> Below( this uint @this, uint max ) => SmartTest.UInt.Below( max );


        /// <summary>
        ///     Adds a chunk of numeric values below or equal to min
        /// </summary>
        /// <param name="this">A uint we do not care about, except to know to create a <see cref="INumericType{T}" /> for uint.</param>
        /// <param name="value">A random value within this range.</param>
        /// <returns>Return a new <see cref="INumericType{T}" /> for uint so that adding chunks can be chained.</returns>
        public static INumericType<uint> Value( this uint @this, uint value ) => SmartTest.UInt.Value( value );
    }
}