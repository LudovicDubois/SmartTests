using System;

using SmartTests.Criterias;



namespace SmartTests.Ranges
{
    /// <summary>
    ///     Represents a Range of short values (with several chunks)
    /// </summary>
    public class ShortType: NumericType<short, ShortType>
    {
        /// <inheritdoc />
        protected override short MinValue => short.MinValue;
        /// <inheritdoc />
        protected override short MaxValue => short.MaxValue;


        /// <inheritdoc />
        protected override short GetPrevious( short n ) => (short)( n - 1 );


        /// <inheritdoc />
        protected override short GetNext( short n ) => (short)( n + 1 );


        /// <inheritdoc />
        public override Criteria GetValidValue( out short value )
        {
            // Ensure values are well distributed
            int max = short.MinValue;
            foreach( var chunk in Chunks )
                max += chunk.IncludedMax - chunk.IncludedMin;

            var random = new Random();
            var val = random.Next( short.MinValue, max );
            max = short.MinValue;
            foreach( var chunk in Chunks )
            {
                var min = max + 1;
                max += chunk.IncludedMax - chunk.IncludedMin;
                if( val > max )
                    continue;
                value = (short)( val - min + chunk.IncludedMin );
                return AnyValue.IsValid;
            }

            throw new NotImplementedException();
        }


        /// <inheritdoc />
        protected override string ToString( short value )
        {
            if( value == short.MinValue )
                return "short.MinValue";
            if( value == short.MaxValue )
                return "short.MaxValue";
            return value.ToString();
        }


        /// <inheritdoc />
        public override string ToString() => ToString( "Short" );
    }
}