using Microsoft.CodeAnalysis;



namespace SmartTestsAnalyzer.TestFrameworks
{
    public class NUnitStrategy: AttributedTestingFramework
    {
        public NUnitStrategy( Compilation compilation )
            : base( compilation, "NUnit.Framework.TestFixtureAttribute", "NUnit.Framework.TestAttribute" )
        { }
    }
}