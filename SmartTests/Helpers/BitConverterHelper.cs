using System;
using System.Runtime.InteropServices;



namespace SmartTests.Helpers
{
    /// <summary>
    ///     An helper class for <see cref="BitConverter" />.
    /// </summary>
    public static class BitConverterHelper
    {
        /// <summary>
        ///     Convert a float value to the bits of its representation
        /// </summary>
        /// <param name="f">The float to be represented as an integer.</param>
        /// <returns>An integer representation of the float.</returns>
        /// <seealso cref="Int32BitsToFloat"/>
        public static int FloatToInt32Bits( float f )
        {
            var bits = default(FloatUnion);
            bits.FloatData = f;
            return bits.IntData;
        }


        /// <summary>
        ///     Convert the bits of its representation to a float
        /// </summary>
        /// <param name="i">The integer representation of the float.</param>
        /// <returns>A float from its integer representation.</returns>
        /// <seealso cref="FloatToInt32Bits"/>
        public static float Int32BitsToFloat( int i )
        {
            var bits = default(FloatUnion);
            bits.IntData = i;
            return bits.FloatData;
        }


        /// <summary>
        ///     A union used to convert between a float and the bits of its representation
        /// </summary>
        [StructLayout( LayoutKind.Explicit )]
        private struct FloatUnion
        {
            [FieldOffset( 0 )]
            public int IntData;
            [FieldOffset( 0 )]
            public float FloatData;
        }


        /// <summary>
        ///     Returns the greatest float that is smaller than <paramref name="n" />.
        /// </summary>
        /// <param name="n">The number for which the previous value is to be computed.</param>
        /// <returns>The greatest value that is smaller than <paramref name="n" /></returns>
        public static float Previous( float n )
        {
            if( n == 0 )
                return -Int32BitsToFloat( FloatToInt32Bits( 0 ) + 1 );
            return Int32BitsToFloat( FloatToInt32Bits( n ) - 1 );
        }


        /// <summary>
        ///     Returns the smallest float that is greater than <paramref name="n" />.
        /// </summary>
        /// <param name="n">The number for which the following value is to be computed.</param>
        /// <returns>The smallest value that is greater than <paramref name="n" /></returns>
        public static float Next( float n ) => Int32BitsToFloat( FloatToInt32Bits( n ) + 1 );


        /// <summary>
        ///     Returns the greatest double that is smaller than <paramref name="n" />.
        /// </summary>
        /// <param name="n">The number for which the previous value is to be computed.</param>
        /// <returns>The greatest value that is smaller than <paramref name="n" /></returns>
        public static double Previous( double n )
        {
            if( n == 0 )
                return -BitConverter.Int64BitsToDouble( BitConverter.DoubleToInt64Bits( n ) + 1 );
            return BitConverter.Int64BitsToDouble( BitConverter.DoubleToInt64Bits( n ) - 1 );
        }


        /// <summary>
        ///     Returns the smallest double that is greater than <paramref name="n" />.
        /// </summary>
        /// <param name="n">The number for which the following value is to be computed.</param>
        /// <returns>The smallest value that is greater than <paramref name="n" /></returns>
        public static double Next( double n ) => BitConverter.Int64BitsToDouble( BitConverter.DoubleToInt64Bits( n ) + 1 );
    }
}