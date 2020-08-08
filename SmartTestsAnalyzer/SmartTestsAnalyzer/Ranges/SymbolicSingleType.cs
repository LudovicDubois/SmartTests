// ReSharper disable UnusedMember.Global


namespace SmartTestsAnalyzer.Ranges
{
    /// <summary>
    ///     Represents a Range of integer values (with several chunks)
    /// </summary>
    public class SymbolicSingleType: SymbolicNumericType<float>
    {
        /// <inheritdoc />
        protected override SymbolicConstant<float> MinValue { get; } = new SymbolicConstant<float>( float.MinValue );
        /// <inheritdoc />
        protected override SymbolicConstant<float> MaxValue { get; } = new SymbolicConstant<float>( float.MaxValue );


        /// <inheritdoc />
        public override string ToString() => ToString( "SingleRange" );
    }
}