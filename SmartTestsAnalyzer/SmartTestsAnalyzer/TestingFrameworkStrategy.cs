using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;



namespace SmartTestsAnalyzer
{
    public static class TestingFrameworks
    {
        private static readonly List<TestingFrameworkStrategy> _TestingFrameworks = new List<TestingFrameworkStrategy>
                                                                                    {
                                                                                        new NUnitStrategy()
                                                                                    };


        public static bool IsTestMethod( this IMethodSymbol method, SemanticModel model ) => _TestingFrameworks.Any( strategy => strategy.IsTestMethod( model, method ) );
        public static bool IsTestClass( this ITypeSymbol type, SemanticModel model ) => _TestingFrameworks.Any( strategy => strategy.IsTestClass( model, type ) );
    }


    public abstract class TestingFrameworkStrategy
    {
        public abstract bool IsTestMethod( SemanticModel model, IMethodSymbol method );
        public abstract bool IsTestClass( SemanticModel model, ITypeSymbol type );

        protected static bool HasAttribute( ISymbol symbol, INamedTypeSymbol type ) => symbol.GetAttributes().Any( attribute => attribute.AttributeClass == type );
    }


    public class NUnitStrategy: TestingFrameworkStrategy
    {
        public override bool IsTestMethod( SemanticModel model, IMethodSymbol method ) => HasAttribute( method, model.Compilation.GetTypeByMetadataName( "NUnit.Framework.TestAttribute" ) );


        public override bool IsTestClass( SemanticModel model, ITypeSymbol type ) => HasAttribute( type, model.Compilation.GetTypeByMetadataName( "NUnit.Framework.TestFixtureAttribute" ) );
    }
}