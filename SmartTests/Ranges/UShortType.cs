using System;

using SmartTests.Criterias;



namespace SmartTests.Ranges
{
    /// <summary>
    ///     Represents a Range of integer values (with several chunks)
    /// </summary>
    public class UShortType: NumericType<ushort, UShortType>
    {
        /// <inheritdoc />
        protected override ushort MinValue => ushort.MinValue;
        /// <inheritdoc />
        protected override ushort MaxValue => ushort.MaxValue;


        /// <inheritdoc />
        protected override ushort GetPrevious( ushort n ) => (ushort)( n - 1 );


        /// <inheritdoc />
        protected override ushort GetNext( ushort n ) => (ushort)( n + 1 );


        /// <inheritdoc />
        public override Criteria GetValidValue( out ushort value )
        {
            // Ensure values are well distributed
            int max = MinValue;
            foreach( var chunk in Chunks )
                max += chunk.IncludedMax - chunk.IncludedMin + 1; // +1 because both are included

            var random = new Random();
            value = (ushort)random.Next( ushort.MinValue, max );
            if( max == MaxValue )
                return AnyValue.IsValid;

            max = ushort.MinValue;
            foreach( var chunk in Chunks )
            {
                var min = max;
                max += chunk.IncludedMax - chunk.IncludedMin + 1;
                if( value >= max )
                    continue;
                value = (ushort)( value - min + chunk.IncludedMin );
                return AnyValue.IsValid;
            }

            throw new NotImplementedException();
        }


        /// <inheritdoc />
        protected override string ToString( ushort value )
        {
            if( value == ushort.MaxValue )
                return "ushort.MaxValue";
            return value.ToString();
        }


        /// <inheritdoc />
        public override string ToString() => ToString( "UShort" );
    }
}