// ReSharper disable UnusedMember.Global


namespace SmartTestsAnalyzer.Ranges
{
    /// <summary>
    ///     Represents a Range of integer values (with several chunks)
    /// </summary>
    public class SymbolicInt16Type: SymbolicNumericType<short>
    {
        /// <inheritdoc />
        protected override SymbolicConstant<short> MinValue { get; } = new SymbolicConstant<short>( short.MinValue );
        /// <inheritdoc />
        protected override SymbolicConstant<short> MaxValue { get; } = new SymbolicConstant<short>( short.MaxValue );


        /// <inheritdoc />
        public override string ToString() => ToString( "Int16Range" );
    }
}