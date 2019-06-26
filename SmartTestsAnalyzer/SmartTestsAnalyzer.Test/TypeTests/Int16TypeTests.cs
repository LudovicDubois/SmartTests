using NUnit.Framework;



namespace SmartTestsAnalyzer.Test.TypeTests
{
    [TestFixture]
    class Int16TypeTests: TypeBaseTests
    {
        public Int16TypeTests()
            : base( "short", "Int16Range" )
        { }
    }
}