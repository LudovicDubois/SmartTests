using NUnit.Framework;



namespace SmartTestsAnalyzer.Test.TypeTests
{
    [TestFixture]
    class Int16TypeHelperTests: TypeHelperBaseTest
    {
        public Int16TypeHelperTests()
            : base( "short", "Int16Range" )
        { }
    }
}