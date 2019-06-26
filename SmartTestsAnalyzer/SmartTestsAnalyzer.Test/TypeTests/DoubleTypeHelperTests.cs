using NUnit.Framework;



namespace SmartTestsAnalyzer.Test.TypeTests
{
    [TestFixture]
    class DoubleTypeHelperTests: TypeHelperBaseTest
    {
        public DoubleTypeHelperTests()
            : base( "double", "DoubleRange" )
        { }
    }
}