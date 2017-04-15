using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using Microsoft.CodeAnalysis;

using SmartTestsAnalyzer.Helpers;



namespace SmartTestsAnalyzer
{
    public class TestingFrameworks
    {
        public TestingFrameworks( SemanticModel model )
        {
            _TestingFrameworks.Add( new NUnitStrategy( model ) );
        }


        private readonly List<TestingFrameworkStrategy> _TestingFrameworks = new List<TestingFrameworkStrategy>();

        public bool IsTestClass( ITypeSymbol type ) => _TestingFrameworks.Any( strategy => strategy.IsTestClass( type ) );
        public bool IsTestMethod( IMethodSymbol method ) => _TestingFrameworks.Any( strategy => strategy.IsTestMethod( method ) );
    }


    public abstract class TestingFrameworkStrategy
    {
        public abstract bool IsTestClass( ITypeSymbol type );
        public abstract bool IsTestMethod( IMethodSymbol method );
    }


    public class NUnitStrategy: TestingFrameworkStrategy
    {
        public NUnitStrategy( SemanticModel model )
        {
            _TestFixtureAttribute = model.Compilation.GetTypeByMetadataName( "NUnit.Framework.TestFixtureAttribute" );
            Debug.Assert( _TestFixtureAttribute != null );
            _TestAttribute = model.Compilation.GetTypeByMetadataName( "NUnit.Framework.TestAttribute" );
            Debug.Assert( _TestAttribute != null );
        }


        private readonly INamedTypeSymbol _TestFixtureAttribute;
        private readonly INamedTypeSymbol _TestAttribute;

        public override bool IsTestClass( ITypeSymbol type ) => type.HasAttribute( _TestFixtureAttribute );

        public override bool IsTestMethod( IMethodSymbol method ) => method.HasAttribute( _TestAttribute );
    }
}