using System;

using SmartTests.Criterias;



namespace SmartTests.Ranges
{
    public class EnumType: IType
    {
        /// <summary>
        ///     Adds enum values to test for as an equivalence class.
        /// </summary>
        /// <typeparam name="T">An enum type.</typeparam>
        /// <param name="value">A random value within these values.</param>
        /// <param name="values">The values as an equivalence class for the current test.</param>
        /// <returns>The criteria representing the full range.</returns>
        public Criteria Values<T>( out T value, params T[] values )
            where T: struct, IComparable
        {
            var random = new Random();
            value = values[ random.Next( values.Length ) ];
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
        /// <param name="values">The values as an equivalence class for the current test.</param>
        /// <returns>The criteria representing the full range.</returns>
        public Criteria ErrorValues<T>( out T value, params T[] values )
            where T: struct, IComparable
        {
            var random = new Random();
            value = values[ random.Next( values.Length ) ];
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
}