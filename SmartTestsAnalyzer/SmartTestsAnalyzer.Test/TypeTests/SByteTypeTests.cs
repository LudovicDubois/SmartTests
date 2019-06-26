using NUnit.Framework;



namespace SmartTestsAnalyzer.Test.TypeTests
{
    [TestFixture]
    class SByteTypeTests: TypeBaseTests
    {
        public SByteTypeTests()
            : base( "sbyte", "SByteRange" )
        { }
    }
}