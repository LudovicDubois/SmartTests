using NUnit.Framework;



namespace SmartTestsAnalyzer.Test.TypeTests
{
    [TestFixture]
    class SingleTypeTests: TypeBaseTests
    {
        public SingleTypeTests()
            : base( "float", "SingleRange" )
        { }
    }
}