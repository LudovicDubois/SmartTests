// ReSharper disable UnusedMember.Global


namespace SmartTestsAnalyzer.Ranges
{
    /// <summary>
    ///     Represents a Range of integer values (with several chunks)
    /// </summary>
    public class SymbolicByteType: SymbolicNumericType<byte>
    {
        /// <inheritdoc />
        protected override SymbolicConstant<byte> MinValue { get; } = new SymbolicConstant<byte>( byte.MinValue );
        /// <inheritdoc />
        protected override SymbolicConstant<byte> MaxValue { get; } = new SymbolicConstant<byte>( byte.MaxValue );

        public override string ToString() => ToString( "ByteRange" );
    }
}