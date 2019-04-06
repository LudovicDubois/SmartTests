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
}