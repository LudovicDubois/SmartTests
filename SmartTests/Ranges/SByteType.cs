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
            int max = MinValue;
            foreach( var chunk in Chunks )
                max += chunk.IncludedMax - chunk.IncludedMin + 1; // +1 because both are included

            var random = new Random();
            value = (sbyte)random.Next( MinValue, max );
            if( max == MaxValue )
                return AnyValue.IsValid;

            max = MinValue;
            foreach( var chunk in Chunks )
            {
                var min = max;
                max += chunk.IncludedMax - chunk.IncludedMin + 1;
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