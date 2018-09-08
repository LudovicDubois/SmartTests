using System;

using SmartTests.Criterias;
using SmartTests.Helpers;



namespace SmartTests.Ranges
{
    /// <summary>
    ///     Represents a Range of float values (with several chunks)
    /// </summary>
    public class FloatType: NumericType<float, FloatType>
    {
        /// <inheritdoc />
        protected override float MinValue => float.MinValue;
        /// <inheritdoc />
        protected override float MaxValue => float.MaxValue;


        /// <inheritdoc />
        protected override float GetPrevious( float n ) => BitConverterHelper.Previous( n );


        /// <inheritdoc />
        protected override float GetNext( float n ) => BitConverterHelper.Next( n );


        /// <inheritdoc />
        public override Criteria GetValidValue( out float value )
        {
            // Ensure values are well distributed
            var max = float.MinValue;
            foreach( var chunk in Chunks )
                max += chunk.IncludedMax - chunk.IncludedMin;
            var random = new Random();

            if( max == float.MaxValue )
            {
                value = (float)( random.NextDouble() * ( float.MaxValue - float.MinValue ) + float.MinValue );
                return AnyValue.IsValid;
            }

            value = (float)( random.NextDouble() * ( max - float.MinValue ) + float.MinValue );
            max = float.MinValue;
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
        protected override string ToString( float value )
        {
            if( value == float.MinValue )
                return "float.MinValue";
            if( value == float.MaxValue )
                return "float.MaxValue";

            return value.ToString();
        }


        /// <inheritdoc />
        public override string ToString() => ToString( "Float" );
    }
}