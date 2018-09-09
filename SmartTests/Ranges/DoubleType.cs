using System;

using SmartTests.Criterias;
using SmartTests.Helpers;



namespace SmartTests.Ranges
{
    /// <summary>
    ///     Represents a Range of double values (with several chunks)
    /// </summary>
    public class DoubleType: NumericType<double, DoubleType>
    {
        /// <inheritdoc />
        protected override double MinValue => double.MinValue;
        /// <inheritdoc />
        protected override double MaxValue => double.MaxValue;


        /// <inheritdoc />
        protected override double GetPrevious( double n ) => BitConverterHelper.Previous( n );


        /// <inheritdoc />
        protected override double GetNext( double n ) => BitConverterHelper.Next( n );


        /// <inheritdoc />
        public override Criteria GetValidValue( out double value )
        {
            // Ensure values are well distributed
            var max = double.MinValue;
            foreach( var chunk in Chunks )
                max += chunk.IncludedMax - chunk.IncludedMin;
            var random = new Random();

            if( max == double.MaxValue )
            {
                value = random.NextDouble() * ( double.MaxValue - double.MinValue ) + double.MinValue;
                return AnyValue.IsValid;
            }

            value = random.NextDouble() * ( max - double.MinValue ) + double.MinValue;
            max = double.MinValue;
            foreach( var chunk in Chunks )
            {
                var min = GetNext( max );
                max += chunk.IncludedMax - chunk.IncludedMin;
                if( value > max )
                    continue;
                value = value - min + chunk.IncludedMin;
                return AnyValue.IsValid;
            }

            throw new NotImplementedException();
        }


        /// <inheritdoc />
        protected override string ToString( double value )
        {
            if( value == double.MinValue )
                return "double.MinValue";
            if( value == double.MaxValue )
                return "double.MaxValue";

            return value.ToString();
        }


        /// <inheritdoc />
        public override string ToString() => ToString( "Double" );
    }
}