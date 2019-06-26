using NUnit.Framework;



namespace SmartTestsAnalyzer.Test.TypeTests
{
    [TestFixture]
    class UInt32TypeHelperTests: TypeHelperBaseTest
    {
        public UInt32TypeHelperTests()
            : base( "uint", "UInt32Range" )
        { }
    }
}