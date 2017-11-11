using Microsoft.CodeAnalysis;

using SmartTestsAnalyzer.Helpers;



namespace SmartTestsAnalyzer.TestFrameworks
{
    public class XUnitStrategy: TestingFrameworkStrategy
    {
        public XUnitStrategy( Compilation compilation )
        {
            _FactMethodAttribute = compilation.GetTypeByMetadataName( "Xunit.FactAttribute" );
            _TheoryMethodAttribute = compilation.GetTypeByMetadataName( "Xunit.TheoryAttribute" );
        }


        private readonly INamedTypeSymbol _FactMethodAttribute;
        private readonly INamedTypeSymbol _TheoryMethodAttribute;


        public override bool IsValid => _FactMethodAttribute != null && _TheoryMethodAttribute != null;


        public override bool IsTestClass( ITypeSymbol type ) => true;


        public override bool IsTestMethod( IMethodSymbol method ) => method.HasAttribute( _FactMethodAttribute ) || method.HasAttribute( _TheoryMethodAttribute );
    }
}