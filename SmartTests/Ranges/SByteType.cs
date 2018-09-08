using System;

using SmartTests.Criterias;



namespace SmartTests.Ranges
{
    /// <summary>
    ///     Represents a Range of integer values (with several chunks)
    /// </summary>
    public class SByteType: NumericType<sbyte, SByteType>
    {
        /// <inheritdoc />
        protected override sbyte MinValue => sbyte.MinValue;
        /// <inheritdoc />
        protected override sbyte MaxValue => sbyte.MaxValue;


        /// <inheritdoc />
        protected override sbyte GetPrevious( sbyte n ) => (sbyte)( n - 1 );


        /// <inheritdoc />
        protected override sbyte GetNext( sbyte n ) => (sbyte)( n + 1 );


        /// <inheritdoc />
        public override Criteria GetValidValue( out sbyte value )
        {
            // Ensure values are well distributed
            int max = sbyte.MinValue;
            foreach( var chunk in Chunks )
                max += chunk.IncludedMax - chunk.IncludedMin;
            var random = new Random();

            value = (sbyte)random.Next( sbyte.MinValue, max );
            max = sbyte.MinValue;
            foreach( var chunk in Chunks )
            {
                var min = max + 1;
                max += chunk.IncludedMax - chunk.IncludedMin;
                if( value > max )
                    continue;
                value = (sbyte)( value - min + chunk.IncludedMin );
                return AnyValue.IsValid;
            }

            throw new NotImplementedException();
        }


        /// <inheritdoc />
        protected override string ToString( sbyte value )
        {
            if( value == sbyte.MinValue )
                return "sbyte.MinValue";
            if( value == sbyte.MaxValue )
                return "sbyte.MaxValue";
            return value.ToString();
        }


        /// <inheritdoc />
        public override string ToString() => ToString( "SByte" );
    }
}