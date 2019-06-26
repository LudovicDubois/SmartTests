using NUnit.Framework;



namespace SmartTestsAnalyzer.Test.TypeTests
{
    [TestFixture]
    class ByteTypeHelperTests: TypeHelperBaseTest
    {
        public ByteTypeHelperTests()
            : base( "byte", "ByteRange" )
        { }
    }
}