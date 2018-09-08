using System;

using SmartTests.Criterias;
using SmartTests.Helpers;



namespace SmartTests.Ranges
{
    /// <summary>
    ///     Represents a Range of long values (with several chunks)
    /// </summary>
    public class ULongType: NumericType<ulong, ULongType>
    {
        /// <inheritdoc />
        protected override ulong MinValue => ulong.MinValue;
        /// <inheritdoc />
        protected override ulong MaxValue => ulong.MaxValue;


        /// <inheritdoc />
        protected override ulong GetPrevious( ulong n ) => n - 1;


        /// <inheritdoc />
        protected override ulong GetNext( ulong n ) => n + 1;


        /// <inheritdoc />
        public override Criteria GetValidValue( out ulong value )
        {
            // Ensure values are well distributed
            var max = ulong.MinValue;
            foreach( var chunk in Chunks )
                max += chunk.IncludedMax - chunk.IncludedMin;

            var random = new Random();
            value = random.NextULong( ulong.MinValue, max );
            max = ulong.MinValue;
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
        protected override string ToString( ulong value )
        {
            if( value == ulong.MaxValue )
                return "ulong.MaxValue";
            return value.ToString();
        }


        /// <inheritdoc />
        public override string ToString() => ToString( "ULong" );
    }
}