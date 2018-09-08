using System;

using SmartTests.Criterias;
using SmartTests.Helpers;



namespace SmartTests.Ranges
{
    /// <summary>
    ///     Represents a Range of long values (with several chunks)
    /// </summary>
    public class LongType: NumericType<long, LongType>
    {
        /// <inheritdoc />
        protected override long MinValue => long.MinValue;
        /// <inheritdoc />
        protected override long MaxValue => long.MaxValue;


        /// <inheritdoc />
        protected override long GetPrevious( long n ) => n - 1;


        /// <inheritdoc />
        protected override long GetNext( long n ) => n + 1;


        /// <inheritdoc />
        public override Criteria GetValidValue( out long value )
        {
            // Ensure values are well distributed
            var max = long.MinValue;
            foreach( var chunk in Chunks )
                max += chunk.IncludedMax - chunk.IncludedMin;
            var random = new Random();

            value = random.NextLong( long.MinValue, max );
            max = long.MinValue;
            foreach( var chunk in Chunks )
            {
                var min = max + 1;
                max += chunk.IncludedMax - chunk.IncludedMin;
                if( value > max )
                    continue;
                value = value - min + chunk.IncludedMin;
                return AnyValue.IsValid;
            }

            throw new NotImplementedException();
        }


        /// <inheritdoc />
        protected override string ToString( long value )
        {
            if( value == long.MinValue )
                return "long.MinValue";
            if( value == long.MaxValue )
                return "long.MaxValue";
            return value.ToString();
        }


        /// <inheritdoc />
        public override string ToString() => ToString( "Long" );
    }
}