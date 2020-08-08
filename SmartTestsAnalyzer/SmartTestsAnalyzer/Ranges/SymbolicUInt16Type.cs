// ReSharper disable UnusedMember.Global


namespace SmartTestsAnalyzer.Ranges
{
    /// <summary>
    ///     Represents a Range of integer values (with several chunks)
    /// </summary>
    public class SymbolicUInt16Type: SymbolicNumericType<ushort>
    {
        /// <inheritdoc />
        protected override SymbolicConstant<ushort> MinValue { get; } = new SymbolicConstant<ushort>( ushort.MinValue );
        /// <inheritdoc />
        protected override SymbolicConstant<ushort> MaxValue { get; } = new SymbolicConstant<ushort>( ushort.MaxValue );


        /// <inheritdoc />
        public override string ToString() => ToString( "UInt16Range" );
    }
}