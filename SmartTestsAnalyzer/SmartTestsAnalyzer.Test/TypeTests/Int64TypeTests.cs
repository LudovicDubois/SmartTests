using NUnit.Framework;



namespace SmartTestsAnalyzer.Test.TypeTests
{
    [TestFixture]
    class Int64TypeTests: TypeBaseTests
    {
        public Int64TypeTests()
            : base( "long", "Int64Range" )
        { }
    }
}