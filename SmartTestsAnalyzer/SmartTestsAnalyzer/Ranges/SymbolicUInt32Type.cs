// ReSharper disable UnusedMember.Global


namespace SmartTestsAnalyzer.Ranges
{
    /// <summary>
    ///     Represents a Range of integer values (with several chunks)
    /// </summary>
    public class SymbolicUInt32Type: SymbolicNumericType<uint>
    {
        /// <inheritdoc />
        protected override SymbolicConstant<uint> MinValue { get; } = new SymbolicConstant<uint>( uint.MinValue );
        /// <inheritdoc />
        protected override SymbolicConstant<uint> MaxValue { get; } = new SymbolicConstant<uint>( uint.MaxValue );


        /// <inheritdoc />
        public override string ToString() => ToString( "UInt32Range" );
    }
}