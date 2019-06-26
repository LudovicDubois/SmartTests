using NUnit.Framework;



namespace SmartTestsAnalyzer.Test.TypeTests
{
    [TestFixture]
    class UInt16TypeHelperTests: TypeHelperBaseTest
    {
        public UInt16TypeHelperTests()
            : base( "ushort", "UInt16Range" )
        { }
    }
}