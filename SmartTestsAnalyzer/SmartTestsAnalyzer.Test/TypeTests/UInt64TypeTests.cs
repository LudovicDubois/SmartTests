using NUnit.Framework;



namespace SmartTestsAnalyzer.Test.TypeTests
{
    [TestFixture]
    class UInt64TypeTests: TypeBaseTests
    {
        public UInt64TypeTests()
            : base( "ulong", "UInt64Range" )
        { }
    }
}