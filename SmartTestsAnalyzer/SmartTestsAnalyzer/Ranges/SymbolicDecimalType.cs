// ReSharper disable UnusedMember.Global


namespace SmartTestsAnalyzer.Ranges
{
    /// <summary>
    ///     Represents a Range of integer values (with several chunks)
    /// </summary>
    public class SymbolicDecimalType: SymbolicNumericType<decimal>
    {
        /// <inheritdoc />
        protected override SymbolicConstant<decimal> MinValue { get; } = new SymbolicConstant<decimal>( decimal.MinValue );
        /// <inheritdoc />
        protected override SymbolicConstant<decimal> MaxValue { get; } = new SymbolicConstant<decimal>( decimal.MaxValue );


        /// <inheritdoc />
        public override string ToString() => ToString( "DecimalRange" );
    }
}