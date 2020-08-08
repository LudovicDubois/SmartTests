// ReSharper disable UnusedMember.Global


namespace SmartTestsAnalyzer.Ranges
{
    /// <summary>
    ///     Represents a Range of integer values (with several chunks)
    /// </summary>
    public class SymbolicInt64Type: SymbolicNumericType<long>
    {
        /// <inheritdoc />
        protected override SymbolicConstant<long> MinValue { get; } = new SymbolicConstant<long>( long.MinValue );
        /// <inheritdoc />
        protected override SymbolicConstant<long> MaxValue { get; } = new SymbolicConstant<long>( long.MaxValue );


        /// <inheritdoc />
        public override string ToString() => ToString( "Int64Range" );
    }
}