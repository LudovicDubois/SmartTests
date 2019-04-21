using System;

using SmartTests.Criterias;



namespace SmartTests.Ranges
{
    /// <summary>
    ///     Represents one or more enum values
    /// </summary>
    public class EnumType: IType
    {
        /// <summary>
        ///     Adds enum values to test for as an equivalence class.
        /// </summary>
        /// <typeparam name="T">An enum type.</typeparam>
        /// <param name="value">A random value within these values.</param>
        /// <param name="firstValue">To be sure there is at least one value.</param>
        /// <param name="values">The other values as an equivalence class for the current test.</param>
        /// <returns>The criteria representing the full range.</returns>
        public Criteria Values<T>( out T value, T firstValue, params T[] values )
            where T: struct, IComparable
        {
            var random = new Random();
            var index = random.Next( values.Length + 1 );
            value = index == values.Length ? firstValue : values[ index ];
            return AnyValue.IsValid;
        }


        /// <summary>
        ///     Adds an enum value to test for.
        /// </summary>
        /// <typeparam name="T">An enum type.</typeparam>
        /// <param name="value">A single value of the enum type.</param>
        /// <returns>The criteria representing the full range.</returns>
        public Criteria Value<T>( T value )
            where T: struct, IComparable
            => AnyValue.IsValid;


        /// <summary>
        ///     Adds enum values (considered an error) to test for as an equivalence class.
        /// </summary>
        /// <typeparam name="T">An enum type.</typeparam>
        /// <param name="value">A random value within these values.</param>
        /// <param name="firstValue">To be sure there is at least one value.</param>
        /// <param name="values">The other values as an equivalence class for the current test.</param>
        /// <returns>The criteria representing the full range.</returns>
        public Criteria ErrorValues<T>( out T value, T firstValue, params T[] values )
            where T: struct, IComparable
        {
            var random = new Random();
            var index = random.Next( values.Length + 1 );
            value = index == values.Length ? firstValue : values[ index ];
            return AnyValue.IsValid;
        }


        /// <summary>
        ///     Adds an enum value (considered an error) to test for.
        /// </summary>
        /// <typeparam name="T">An enum type.</typeparam>
        /// <param name="value">A single value of the enum type.</param>
        /// <returns>The criteria representing the full range.</returns>
        public Criteria ErrorValue<T>( T value )
            where T: struct, IComparable
            => AnyValue.IsValid;
    }


    public static class EnumTypeHelper
    {
        public class PlaceHolder<T>
            where T: struct, IComparable
        { }


        /// <summary>
        ///     Adds enum values to test for as an equivalence class.
        /// </summary>
        /// <typeparam name="T">An enum type.</typeparam>
        /// <param name="firstValue">To be sure there is at least one value.</param>
        /// <param name="values">The other values as an equivalence class for the current test.</param>
        /// <returns>The criteria representing the full range.</returns>
        public static PlaceHolder<T> Values<T>( this T @this, params T[] values )
            where T: struct, IComparable
            => new PlaceHolder<T>();
    }
}