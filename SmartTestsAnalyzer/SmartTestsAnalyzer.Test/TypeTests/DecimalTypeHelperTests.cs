using NUnit.Framework;



namespace SmartTestsAnalyzer.Test.TypeTests
{
    [TestFixture]
    class DecimalTypeHelperTests: TypeHelperBaseTest
    {
        public DecimalTypeHelperTests()
            : base( "decimal", "Decimal" )
        { }
    }
}