using System;
using System.Linq;

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
            if( values == null )
                values = new T[0];

            var random = new Random();
            var index = random.Next( values.Length + 1 );
            value = index == values.Length ? firstValue : values[ index ];
            return AnyValue.IsValid;
        }


        /// <summary>
        ///     Adds enum values to test for as an equivalence class.
        /// </summary>
        /// <typeparam name="T">An enum type.</typeparam>
        /// <param name="values">The other values as an equivalence class for the current test.</param>
        /// <param name="value">A random value within these values.</param>
        /// <param name="avoidedValues">
        ///     A value to avoid in the range.
        ///     For example, when testing a property setter with a different value, you do not want the value to be the current
        ///     value, even if it is within the tested range.
        /// </param>
        /// <returns>The criteria representing the full range.</returns>
        public Criteria GetValidValue<T>( T[] values, out T value, params T[] avoidedValues )
        {
            if( values == null )
                throw new ArgumentNullException( nameof(values) );
            if( values.Length == 0 )
                throw new ArgumentException( "Should have at least one value", nameof(values) );

            var random = new Random();
            while( true )
            {
                value = values[ random.Next( values.Length ) ];
                if( !avoidedValues.Contains( value ) )
                    return AnyValue.IsValid;
            }
        }


        /// <summary>
        ///     Adds an enum value to test for.
        /// </summary>
        /// <typeparam name="T">An enum type.</typeparam>
        /// <param name="value">A single value of the enum type.</param>
        /// <returns>The criteria representing the full range.</returns>
        // ReSharper disable once UnusedParameter.Global
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
            if( values == null )
                values = new T[0];

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
        // ReSharper disable once UnusedParameter.Global
        public Criteria ErrorValue<T>( T value )
            where T: struct, IComparable
            => AnyValue.IsValid;
    }


    /// <summary>
    ///     A helper type to create <see cref="EnumType" />.
    /// </summary>
    public static class EnumTypeHelper
    {
        /// <summary>
        ///     A place holder type to distinguish the right
        ///     <see
        ///         cref="SmartTest.Case{TParam,T}(System.Linq.Expressions.Expression{System.Func{TParam,SmartTests.Ranges.EnumTypeHelper.PlaceHolder{T}}},out T, T[])" />
        ///     .
        /// </summary>
        /// <typeparam name="T"></typeparam>
        // ReSharper disable once UnusedTypeParameter
        public class PlaceHolder<T>
            where T: struct, IComparable
        { }


        /// <summary>
        ///     Adds enum values to test for as an equivalence class.
        /// </summary>
        /// <typeparam name="T">An enum type.</typeparam>
        /// <param name="_">An enum we do not care about, except to know to create a <see cref="EnumType" />.</param>
        /// <param name="values">The values as an equivalence class for the current test.</param>
        /// <returns>The criteria representing the full range.</returns>
        // ReSharper disable once UnusedParameter.Global
        public static PlaceHolder<T> Values<T>( this T _, params T[] values )
            where T: struct, IComparable
            => new PlaceHolder<T>();
    }
}