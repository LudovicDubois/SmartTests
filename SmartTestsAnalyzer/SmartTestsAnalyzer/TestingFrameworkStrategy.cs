using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;

using SmartTestsAnalyzer.Helpers;
using SmartTestsAnalyzer.TestFrameworks;



namespace SmartTestsAnalyzer
{
    public class TestingFrameworks
    {
        public TestingFrameworks( Compilation compilation )
        {
            var nunit = new NUnitStrategy( compilation );
            if( nunit.IsValid )
                _TestingFrameworks.Add( nunit );
        }


        public bool IsTestProject => _TestingFrameworks.Count > 0;

        private readonly List<TestingFrameworkStrategy> _TestingFrameworks = new List<TestingFrameworkStrategy>();

        public bool IsTestClass( ITypeSymbol type ) => _TestingFrameworks.Any( strategy => strategy.IsTestClass( type ) );
        public bool IsTestMethod( IMethodSymbol method ) => _TestingFrameworks.Any( strategy => strategy.IsTestMethod( method ) );
    }


    public abstract class TestingFrameworkStrategy
    {
        public abstract bool IsTestClass( ITypeSymbol type );
        public abstract bool IsTestMethod( IMethodSymbol method );
    }


    public abstract class AttributedTestingFramework: TestingFrameworkStrategy
    {
        private readonly INamedTypeSymbol _TestClassAttribute;
        private readonly INamedTypeSymbol _TestMethodAttribute;


        protected AttributedTestingFramework( Compilation compilation, string testClassAttribute, string testMethodAttribute )
        {
            _TestClassAttribute = compilation.GetTypeByMetadataName( testClassAttribute );
            if( _TestClassAttribute == null )
                // This project does not contain testClassAttribute
                return;
            _TestMethodAttribute = compilation.GetTypeByMetadataName( testMethodAttribute );
        }


        public bool IsValid => _TestClassAttribute != null && _TestMethodAttribute != null;

        public override bool IsTestClass( ITypeSymbol type ) => type.HasAttribute( _TestClassAttribute );

        public override bool IsTestMethod( IMethodSymbol method ) => method.HasAttribute( _TestMethodAttribute );
    }
}