using System;



namespace SmartTests.Helpers
{
    internal static class RandomHelper
    {
        public static byte NextByte( this Random rnd ) => rnd.NextByte( byte.MinValue, byte.MaxValue );
        public static byte NextByte( this Random rnd, byte min, byte max ) => (byte)rnd.Next( min, max );


        public static sbyte NextSByte( this Random rnd ) => rnd.NextSByte( sbyte.MinValue, sbyte.MaxValue );
        public static sbyte NextSByte( this Random rnd, sbyte min, sbyte max ) => (sbyte)rnd.Next( min, max );


        public static short NextShort( this Random rnd ) => rnd.NextShort( short.MinValue, short.MaxValue );
        public static short NextShort( this Random rnd, short min, short max ) => (short)rnd.Next( min, max );


        public static ushort NextUShort( this Random rnd ) => rnd.NextUShort( ushort.MinValue, ushort.MaxValue );
        public static ushort NextUShort( this Random rnd, ushort min, ushort max ) => (ushort)rnd.Next( min, max );


        public static uint NextUInt( this Random rnd ) => rnd.NextUInt( uint.MinValue, uint.MaxValue );
        public static uint NextUInt( this Random rnd, uint min, uint max ) => (uint)rnd.NextLong( min, max );


        public static long NextLong( this Random rnd )
        {
            var buffer = new byte[8];
            rnd.NextBytes( buffer );
            return BitConverter.ToInt64( buffer, 0 );
        }


        // TODO: not perfectly distributed, but we do not care for tests. If someone want to propose a better way... <G>
        public static long NextLong( this Random rnd, long minValue, long maxValue )
        {
            if( minValue > maxValue )
                throw new ArgumentOutOfRangeException( nameof(minValue), "minValue should be less or equal to maxValue" );

            var min = new Decimal( minValue );
            var max = new Decimal( maxValue );
            return (long)( rnd.NextLong() % ( max - min ) + min );
        }


        public static ulong NextULong( this Random rnd )
        {
            var buffer = new byte[8];
            rnd.NextBytes( buffer );
            return BitConverter.ToUInt64( buffer, 0 );
        }


        // TODO: not perfectly distributed, but we do not care for tests. If someone want to propose a better way... <G>
        public static ulong NextULong( this Random rnd, ulong minValue, ulong maxValue )
        {
            if( minValue > maxValue )
                throw new ArgumentOutOfRangeException( nameof(minValue), "minValue should be less or equal to maxValue" );

            var min = new Decimal( minValue );
            var max = new Decimal( maxValue );
            return (ulong)( rnd.NextULong() % ( max - min ) + min );
        }


        public static decimal NextDecimal( this Random rnd, decimal minValue, decimal maxValue )
        {
            decimal result;
            do
            {
                // The high bits of 0.9999999999999999999999999999m are 542101086.
                result = new decimal( rnd.Next(), rnd.Next(), rnd.Next( 542101087 ), false, 28 );
            } while( result >= 1 );

            return result * ( maxValue - minValue ) + minValue;
        }


        public static DateTime NextDateTime( this Random rnd, DateTime minValue, DateTime maxValue )
        {
            if( minValue > maxValue )
                throw new ArgumentOutOfRangeException( nameof(minValue), "minValue should be less or equal to maxValue" );

            return new DateTime( rnd.NextLong( minValue.Ticks, maxValue.Ticks ) );
        }
    }
}