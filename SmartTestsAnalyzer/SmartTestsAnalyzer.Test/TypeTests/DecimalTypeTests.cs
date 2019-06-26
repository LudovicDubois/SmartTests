using NUnit.Framework;



namespace SmartTestsAnalyzer.Test.TypeTests
{
    [TestFixture]
    class DecimalTypeTests: TypeBaseTests
    {
        public DecimalTypeTests()
            : base( "decimal", "DecimalRange" )
        { }
    }
}