using Microsoft.CodeAnalysis;



namespace SmartTestsAnalyzer.TestFrameworks
{
    public class MSTestStrategy: AttributedTestingFramework
    {
        public MSTestStrategy( Compilation compilation )
            : base( compilation, "Microsoft.VisualStudio.TestTools.UnitTesting.TestClassAttribute", "Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute" )
        { }
    }
}