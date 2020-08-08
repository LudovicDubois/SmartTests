using System;
using System.Collections.Generic;

using SmartTests.Ranges;

// ReSharper disable UnusedMember.Global


namespace SmartTestsAnalyzer.Ranges
{
    /// <summary>
    ///     Represents a range of integers of any integer type T
    /// </summary>
    /// <typeparam name="T">The type of integers for this range</typeparam>
    public interface ISymbolicNumericType<T>: IType
        where T: IComparable<T>
    {
        /// <summary>
        ///     Minimum value for type T
        /// </summary>
        SymbolicConstant<T> MinValue { get; }
        /// <summary>
        ///     Maximum value for type T
        /// </summary>
        SymbolicConstant<T> MaxValue { get; }


        /// <summary>
        ///     The sorted list of non-overlapping chunks
        /// </summary>
        List<SymbolicChunk<T>> Chunks { get; }


        /// <summary>
        ///     Removes the provided Range from this Range
        /// </summary>
        /// <param name="range">The Range to remove from <c>this</c>.</param>
        /// <param name="errors">The Range of the several times removed values.</param>
        void RemoveRange( ISymbolicNumericType<T> range, ISymbolicNumericType<T> errors );


        /// <summary>
        ///     Adds a chunk of numeric values
        /// </summary>
        /// <param name="min">The min value (included) of the chunk.</param>
        /// <param name="max">The max value (included) of the chunk.</param>
        /// <returns>Return <c>this</c> so that adding chunks can be chained.</returns>
        ISymbolicNumericType<T> Range( SymbolicConstant<T> min, SymbolicConstant<T> max );


        /// <summary>
        ///     Adds a chunk of numeric values
        /// </summary>
        /// <param name="min">The min value (included) of the chunk.</param>
        /// <param name="minIncluded"><c>true</c> to include min, <c>false</c> otherwise.</param>
        /// <param name="max">The max value (included) of the chunk.</param>
        /// <param name="maxIncluded"><c>true</c> to include max, <c>false</c> otherwise.</param>
        /// <returns>Return <c>this</c> so that adding chunks can be chained.</returns>
        ISymbolicNumericType<T> Range( SymbolicConstant<T> min, bool minIncluded, SymbolicConstant<T> max, bool maxIncluded );


        /// <summary>
        ///     Adds a chunk of numeric values above a min
        /// </summary>
        /// <param name="min">The min value (included) of the created chunk.</param>
        /// <returns>returns <c>this</c>.</returns>
        ISymbolicNumericType<T> AboveOrEqual( SymbolicConstant<T> min );


        /// <summary>
        ///     Adds a chunk of numeric values above min
        /// </summary>
        /// <param name="min">The min value (excluded) of the created chunk.</param>
        /// <returns>returns <c>this</c>.</returns>
        ISymbolicNumericType<T> Above( SymbolicConstant<T> min );


        /// <summary>
        ///     Adds a chunk of numeric values below or equal to max
        /// </summary>
        /// <param name="max">The max value (included) of the range.</param>
        /// <returns>returns <c>this</c>.</returns>
        ISymbolicNumericType<T> BelowOrEqual( SymbolicConstant<T> max );


        /// <summary>
        ///     Adds a chunk of numeric values below max
        /// </summary>
        /// <param name="max">The max value (excluded) of the created chunk.</param>
        /// <returns>returns <c>this</c>.</returns>
        ISymbolicNumericType<T> Below( SymbolicConstant<T> max );


        /// <summary>
        ///     Creates a range of one numeric value
        /// </summary>
        /// <param name="value">The minimum and maximum value of this range.</param>
        /// <returns>returns <c>this</c>.</returns>
        ISymbolicNumericType<T> Value( SymbolicConstant<T> value );
    }
}