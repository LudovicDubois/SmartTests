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
            var max = MinValue;
            foreach( var chunk in Chunks )
                max += GetNext( chunk.IncludedMax - chunk.IncludedMin ); // GetNext because both are included

            var random = new Random();
            value = random.NextDouble() * ( max - MinValue ) + MinValue;
            if( max == MaxValue )
                return AnyValue.IsValid;

            max = MinValue;
            foreach( var chunk in Chunks )
            {
                var min = max;
                max += GetNext( chunk.IncludedMax - chunk.IncludedMin );
                if( value >= max )
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