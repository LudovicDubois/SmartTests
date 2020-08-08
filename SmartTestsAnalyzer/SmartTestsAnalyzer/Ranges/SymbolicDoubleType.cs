// ReSharper disable UnusedMember.Global


namespace SmartTestsAnalyzer.Ranges
{
    /// <summary>
    ///     Represents a Range of integer values (with several chunks)
    /// </summary>
    public class SymbolicDoubleType: SymbolicNumericType<double>
    {
        /// <inheritdoc />
        protected override SymbolicConstant<double> MinValue { get; } = new SymbolicConstant<double>( double.MinValue );
        /// <inheritdoc />
        protected override SymbolicConstant<double> MaxValue { get; } = new SymbolicConstant<double>( double.MaxValue );


        /// <inheritdoc />
        public override string ToString() => ToString( "DoubleRange" );
    }
}