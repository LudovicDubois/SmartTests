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
            var nUnit = new NUnitStrategy( compilation );
            if( nUnit.IsValid )
                _TestingFrameworks.Add( nUnit );

            var msTest = new MSTestStrategy( compilation );
            if( msTest.IsValid )
                _TestingFrameworks.Add( msTest );

            var xunit = new XUnitStrategy( compilation );
            if( xunit.IsValid )
                _TestingFrameworks.Add( xunit );
        }


        // ReSharper disable once InconsistentNaming
        public bool IsTestProject => _TestingFrameworks.Count > 0;

        private readonly List<TestingFrameworkStrategy> _TestingFrameworks = new List<TestingFrameworkStrategy>();


        // ReSharper disable once InconsistentNaming
        public bool IsTestClass( ITypeSymbol type ) => _TestingFrameworks.Any( strategy => strategy.IsTestClass( type ) );


        // ReSharper disable once InconsistentNaming
        public bool IsTestMethod( IMethodSymbol method ) => _TestingFrameworks.Any( strategy => strategy.IsTestMethod( method ) );
    }


    public abstract class TestingFrameworkStrategy
    {
        // ReSharper disable once InconsistentNaming
        // ReSharper disable once UnusedMemberInSuper.Global
        public abstract bool IsValid { get; }


        // ReSharper disable once InconsistentNaming
        public abstract bool IsTestClass( ITypeSymbol type );


        // ReSharper disable once InconsistentNaming
        public abstract bool IsTestMethod( IMethodSymbol method );
    }


    public abstract class AttributedTestingFramework: TestingFrameworkStrategy
    {
        private readonly INamedTypeSymbol _TestClassAttribute;
        private readonly INamedTypeSymbol[] _TestMethodAttributes;


        protected AttributedTestingFramework( Compilation compilation, string testClassAttribute, params string[] testMethodAttributes )
        {
            _TestClassAttribute = compilation.GetTypeByMetadataName( testClassAttribute );
            if( _TestClassAttribute == null )
                // This project does not contain testClassAttribute
                return;
            _TestMethodAttributes = testMethodAttributes.Select( compilation.GetTypeByMetadataName ).ToArray();
        }


        public override bool IsValid => _TestClassAttribute != null && _TestMethodAttributes != null;

        public override bool IsTestClass( ITypeSymbol type ) => type.HasAttribute( _TestClassAttribute );

        public override bool IsTestMethod( IMethodSymbol method ) => _TestMethodAttributes.Any( method.HasAttribute );
    }
}