using NUnit.Framework;



namespace SmartTestsAnalyzer.Test.TypeTests
{
    [TestFixture]
    class DoubleTypeTests: TypeBaseTests
    {
        public DoubleTypeTests()
            : base( "double", "DoubleRange" )
        { }
    }
}