using System;

// ReSharper disable UnusedMember.Global


namespace SmartTestsAnalyzer.Ranges
{
    /// <summary>
    ///     Represents a Range of DateTime values (with several chunks)
    /// </summary>
    public class SymbolicDateTimeType: SymbolicNumericType<DateTime>
    {
        /// <inheritdoc />
        protected override SymbolicConstant<DateTime> MinValue { get; } = new SymbolicConstant<DateTime>( DateTime.MinValue );
        /// <inheritdoc />
        protected override SymbolicConstant<DateTime> MaxValue { get; } = new SymbolicConstant<DateTime>( DateTime.MaxValue );


        /// <inheritdoc />
        public override string ToString() => ToString( "DateTimeRange" );
    }
}