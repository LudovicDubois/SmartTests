using NUnit.Framework;



namespace SmartTestsAnalyzer.Test.TypeTests
{
    [TestFixture]
    class UInt32TypeTests: TypeBaseTests
    {
        public UInt32TypeTests()
            : base( "uint", "UInt32Range" )
        { }
    }
}