using NUnit.Framework;



namespace SmartTestsAnalyzer.Test.TypeTests
{
    [TestFixture]
    class UInt16TypeTests: TypeBaseTests
    {
        public UInt16TypeTests()
            : base( "ushort", "UInt16Range" )
        { }
    }
}