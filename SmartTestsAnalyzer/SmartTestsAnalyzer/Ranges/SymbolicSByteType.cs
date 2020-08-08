// ReSharper disable UnusedMember.Global


namespace SmartTestsAnalyzer.Ranges
{
    /// <summary>
    ///     Represents a Range of integer values (with several chunks)
    /// </summary>
    public class SymbolicSByteType: SymbolicNumericType<sbyte>
    {
        /// <inheritdoc />
        protected override SymbolicConstant<sbyte> MinValue { get; } = new SymbolicConstant<sbyte>( sbyte.MinValue );
        /// <inheritdoc />
        protected override SymbolicConstant<sbyte> MaxValue { get; } = new SymbolicConstant<sbyte>( sbyte.MaxValue );


        /// <inheritdoc />
        public override string ToString() => ToString( "SByteRange" );
    }
}