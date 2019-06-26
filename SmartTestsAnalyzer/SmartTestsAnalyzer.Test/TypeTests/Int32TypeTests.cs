using NUnit.Framework;



namespace SmartTestsAnalyzer.Test.TypeTests
{
    [TestFixture]
    class Int32TypeTests: TypeBaseTests
    {
        public Int32TypeTests()
            : base( "int", "Int32Range" )
        { }
    }
}