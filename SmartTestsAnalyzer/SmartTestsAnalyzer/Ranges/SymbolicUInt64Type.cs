// ReSharper disable UnusedMember.Global


namespace SmartTestsAnalyzer.Ranges
{
    /// <summary>
    ///     Represents a Range of integer values (with several chunks)
    /// </summary>
    public class SymbolicUInt64Type: SymbolicNumericType<ulong>
    {
        /// <inheritdoc />
        protected override SymbolicConstant<ulong> MinValue { get; } = new SymbolicConstant<ulong>( ulong.MinValue );
        /// <inheritdoc />
        protected override SymbolicConstant<ulong> MaxValue { get; } = new SymbolicConstant<ulong>( ulong.MaxValue );


        /// <inheritdoc />
        public override string ToString() => ToString( "UInt64Range" );
    }
}