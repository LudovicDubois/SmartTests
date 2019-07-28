using System;
using System.Collections.Generic;



namespace SmartTests.Ranges
{
    /// <summary>
    ///     Represents a range of integers of any integer type T
    /// </summary>
    /// <typeparam name="T">The type of integers for this range</typeparam>
    public interface INumericType<T>: IType
        where T: IComparable<T>
    {
        /// <summary>
        ///     Minimum value for type T
        /// </summary>
        T MinValue { get; }
        /// <summary>
        ///     Maximum value for type T
        /// </summary>
        T MaxValue { get; }


        /// <summary>
        ///     Returns the value preceding <paramref name="n" />
        /// </summary>
        /// <param name="n">The value for which the precedent value must be computed</param>
        /// <returns>The precedent value of <paramref name="n" /></returns>
        T GetPrevious( T n );


        /// <summary>
        ///     Returns the value following <paramref name="n" />
        /// </summary>
        /// <param name="n">The value for which the following value must be computed</param>
        /// <returns>The following value of <paramref name="n" /></returns>
        T GetNext( T n );


        /// <summary>
        ///     The sorted list of non-overlapping chunks
        /// </summary>
        List<Chunk<T>> Chunks { get; }


        /// <summary>
        ///     Adds a chunk of numeric values
        /// </summary>
        /// <param name="min">The min value (included) of the chunk.</param>
        /// <param name="max">The max value (included) of the chunk.</param>
        /// <returns>Return <c>this</c> so that adding chunks can be chained.</returns>
        INumericType<T> Range( T min, T max );


        /// <summary>
        ///     Adds a chunk of numeric values
        /// </summary>
        /// <param name="min">The min value (included) of the chunk.</param>
        /// <param name="minIncluded"><c>true</c> to include min, <c>false</c> otherwise.</param>
        /// <param name="max">The max value (included) of the chunk.</param>
        /// <param name="maxIncluded"><c>true</c> to include max, <c>false</c> otherwise.</param>
        /// <returns>Return <c>this</c> so that adding chunks can be chained.</returns>
        INumericType<T> Range( T min, bool minIncluded, T max, bool maxIncluded );


        /// <summary>
        ///     Returns any value for this range (all values have the same probability)
        /// </summary>
        /// <param name="value">A random value within this range.</param>
        /// <param name="avoidedValues">
        ///     A value to avoid in the range.
        ///     For example, when testing a property setter with a different value, you do not want the value to be the current
        ///     value, even if it is within the tested range.
        /// </param>
        /// <returns>Any <see cref="Criteria" /> so that it can be used everywhere a criteria is expected.</returns>
        Criteria GetValidValue( out T value, params T[] avoidedValues );


        /// <summary>
        ///     Returns any value for this range (all values have the same probability) as an error value
        /// </summary>
        /// <param name="value">A random value within this range.</param>
        /// <returns>Any <see cref="Criteria" /> so that it can be used everywhere a criteria is expected.</returns>
        Criteria GetErrorValue( out T value );


        /// <summary>
        ///     Adds a chunk of numeric values and returns a criteria for this range
        /// </summary>
        /// <param name="min">The minimum value of the created chunk.</param>
        /// <param name="max">The maximum value of the created chunk.</param>
        /// <param name="value">A random value within this range.</param>
        /// <param name="avoidedValues">
        ///     A value to avoid in the range.
        ///     For example, when testing a property setter with a different value, you do not want the value to be the current
        ///     value, even if it is within the tested range.
        /// </param>
        /// <returns>The criteria representing the full range.</returns>
        /// <seealso cref="Criteria" />
        Criteria Range( T min, T max, out T value, params T[] avoidedValues );


        /// <summary>
        ///     Adds a chunk of numeric values
        /// </summary>
        /// <param name="min">The min value (included) of the chunk.</param>
        /// <param name="minIncluded"><c>true</c> to include min, <c>false</c> otherwise.</param>
        /// <param name="max">The max value (included) of the chunk.</param>
        /// <param name="maxIncluded"><c>true</c> to include max, <c>false</c> otherwise.</param>
        /// <param name="value">A random value within this range.</param>
        /// <param name="avoidedValues">
        ///     A value to avoid in the range.
        ///     For example, when testing a property setter with a different value, you do not want the value to be the current
        ///     value, even if it is within the tested range.
        /// </param>
        /// <returns>Return <c>this</c> so that adding chunks can be chained.</returns>
        Criteria Range( T min, bool minIncluded, T max, bool maxIncluded, out T value, params T[] avoidedValues );


        /// <summary>
        ///     Adds a chunk of numeric values above a min
        /// </summary>
        /// <param name="min">The min value (included) of the created chunk.</param>
        /// <returns>returns <c>this</c>.</returns>
        INumericType<T> AboveOrEqual( T min );


        /// <summary>
        ///     Adds a chunk of numeric values above or equal to a min and returns a criteria for this range
        /// </summary>
        /// <param name="min">The min value (included) of the created chunk.</param>
        /// <param name="value">A random value within this range.</param>
        /// <returns>Any <see cref="Criteria" /> so that it can be used everywhere a criteria is expected.</returns>
        Criteria AboveOrEqual( T min, out T value );


        /// <summary>
        ///     Adds a chunk of numeric values above min
        /// </summary>
        /// <param name="min">The min value (excluded) of the created chunk.</param>
        /// <returns>returns <c>this</c>.</returns>
        INumericType<T> Above( T min );


        /// <summary>
        ///     adds a chunk of numeric values above min and returns a criteria for this range
        /// </summary>
        /// <param name="min">The min value (excluded) of the created chunk.</param>
        /// <param name="value">A random value within this range.</param>
        /// <returns>Any <see cref="Criteria" /> so that it can be used everywhere a criteria is expected.</returns>
        Criteria Above( T min, out T value );


        /// <summary>
        ///     Adds a chunk of numeric values below or equal to max
        /// </summary>
        /// <param name="max">The max value (included) of the range.</param>
        /// <returns>returns <c>this</c>.</returns>
        INumericType<T> BelowOrEqual( T max );


        /// <summary>
        ///     Adds a range of numeric values below or equal to max and returns a criteria for this range
        /// </summary>
        /// <param name="max">The max value (included) of the created chunk.</param>
        /// <param name="value">A random value within this range.</param>
        /// <returns>Any <see cref="Criteria" /> so that it can be used everywhere a criteria is expected.</returns>
        Criteria BelowOrEqual( T max, out T value );


        /// <summary>
        ///     Adds a chunk of numeric values below max
        /// </summary>
        /// <param name="max">The max value (excluded) of the created chunk.</param>
        /// <returns>returns <c>this</c>.</returns>
        INumericType<T> Below( T max );


        /// <summary>
        ///     Creates a range of numeric values below max and returns a criteria for this range
        /// </summary>
        /// <param name="max">The max value (excluded) of the created chunk.</param>
        /// <param name="value">A random value within this range.</param>
        /// <returns>Any <see cref="Criteria" /> so that it can be used everywhere a criteria is expected.</returns>
        Criteria Below( T max, out T value );


        /// <summary>
        ///     Creates a range of one numeric value
        /// </summary>
        /// <param name="value">The minimum and maximum value of this range.</param>
        /// <returns>returns <c>this</c>.</returns>
        INumericType<T> Value( T value );


        /// <summary>
        ///     Creates a range of one numeric value and returns a criteria for this range
        /// </summary>
        /// <param name="val">The minimum and maximum value for the created chunk.</param>
        /// <param name="value">A random value within this range.</param>
        /// <returns>Any <see cref="Criteria" /> so that it can be used everywhere a criteria is expected.</returns>
        Criteria Value( T val, out T value );
    }
}