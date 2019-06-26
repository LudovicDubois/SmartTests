using NUnit.Framework;



namespace SmartTestsAnalyzer.Test.TypeTests
{
    [TestFixture]
    class SByteTypeHelperTests: TypeHelperBaseTest
    {
        public SByteTypeHelperTests()
            : base( "sbyte", "SByteRange" )
        { }
    }
}