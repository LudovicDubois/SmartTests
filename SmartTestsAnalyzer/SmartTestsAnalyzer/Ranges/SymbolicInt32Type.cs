// ReSharper disable UnusedMember.Global


namespace SmartTestsAnalyzer.Ranges
{
    /// <summary>
    ///     Represents a Range of integer values (with several chunks)
    /// </summary>
    public class SymbolicInt32Type: SymbolicNumericType<int>
    {
        /// <inheritdoc />
        protected override SymbolicConstant<int> MinValue { get; } = new SymbolicConstant<int>( int.MinValue );
        /// <inheritdoc />
        protected override SymbolicConstant<int> MaxValue { get; } = new SymbolicConstant<int>( int.MaxValue );


        /// <inheritdoc />
        public override string ToString() => ToString( "Int32Range" );
    }
}