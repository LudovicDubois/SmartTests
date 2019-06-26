using NUnit.Framework;



namespace SmartTestsAnalyzer.Test.TypeTests
{
    [TestFixture]
    class Int64TypeHelperTests: TypeHelperBaseTest
    {
        public Int64TypeHelperTests()
            : base( "long", "Int64Range" )
        { }
    }
}