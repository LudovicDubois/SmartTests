using NUnit.Framework;



namespace SmartTestsAnalyzer.Test.TypeTests
{
    [TestFixture]
    class Int32TypeHelperTests: TypeHelperBaseTest
    {
        public Int32TypeHelperTests()
            : base( "int", "Int32Range" )
        { }
    }
}