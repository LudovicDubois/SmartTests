using NUnit.Framework;



namespace SmartTestsAnalyzer.Test.TypeTests
{
    [TestFixture]
    class SingleHelperTypeTests: TypeHelperBaseTest
    {
        public SingleHelperTypeTests()
            : base( "float", "SingleRange" )
        { }
    }
}