using System;
using System.Collections.Generic;



namespace SmartTests.Ranges
{
    /// <summary>
    ///     Represents a range of integers af any integer type
    /// </summary>
    public interface IType
    { }


    /// <summary>
    ///     Represents a range of integers of any integer type T
    /// </summary>
    /// <typeparam name="T">The type of integers for this range</typeparam>
    public interface IType<T>: IType
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
        ///     Add a chunk to the range
        /// </summary>
        /// <param name="min">The min value (included) of the chunk.</param>
        /// <param name="max">The max value (included) of the chunk.</param>
        /// <returns>Return <c>this</c> so that adding chunks can be chained.</returns>
        IType<T> Range( T min, T max );


        /// <summary>
        ///     Returns any value for this range (all values have the same probability)
        /// </summary>
        /// <param name="value">A random value within this range.</param>
        /// <returns>Any <see cref="Criteria" /> so that it can be used everywhere a criteria is expected.</returns>
        Criteria GetValue( out T value );
    }
}