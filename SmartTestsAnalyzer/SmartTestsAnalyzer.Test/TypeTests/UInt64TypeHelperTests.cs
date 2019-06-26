using NUnit.Framework;



namespace SmartTestsAnalyzer.Test.TypeTests
{
    [TestFixture]
    class UInt64TypeHelperTests: TypeHelperBaseTest
    {
        public UInt64TypeHelperTests()
            : base( "ulong", "UInt64Range" )
        { }
    }
}