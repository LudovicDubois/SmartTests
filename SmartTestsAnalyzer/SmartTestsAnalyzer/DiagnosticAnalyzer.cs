using System;
using System.Collections.Immutable;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

using SmartTestsAnalyzer.Helpers;



namespace SmartTestsAnalyzer
{
    [DiagnosticAnalyzer( LanguageNames.CSharp )]
    public class SmartTestsAnalyzerAnalyzer: DiagnosticAnalyzer
    {
        public const string DiagnosticId = "SmartTestsAnalyzer";

        // You can change these strings in the Resources.resx file. If you do not want your analyzer to be localize-able, you can use regular strings for Title and MessageFormat.
        // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/Localizing%20Analyzers.md for more on localization
        private static readonly LocalizableString _Title = new LocalizableResourceString( nameof( Resources.AnalyzerTitle ), Resources.ResourceManager, typeof(Resources) );
        private static readonly LocalizableString _MessageFormat = new LocalizableResourceString( nameof( Resources.AnalyzerMessageFormat ), Resources.ResourceManager, typeof(Resources) );
        private static readonly LocalizableString _Description = new LocalizableResourceString( nameof( Resources.AnalyzerDescription ), Resources.ResourceManager, typeof(Resources) );
        private const string _Category = "Tests";

        private static readonly DiagnosticDescriptor _Rule = new DiagnosticDescriptor( DiagnosticId, _Title, _MessageFormat, _Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: _Description );

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create( _Rule );


        public override void Initialize( AnalysisContext context )
        {
            // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/Analyzer%20Actions%20Semantics.md for more information
            context.RegisterSyntaxNodeAction( AnalyzeMethod, SyntaxKind.MethodDeclaration ); // Tests are methods
        }


        private static void AnalyzeMethod( SyntaxNodeAnalysisContext analysisContext )
        {
            try
            {
                var methodSyntax = (MethodDeclarationSyntax)analysisContext.Node;
                var methodSymbol = analysisContext.SemanticModel.GetDeclaredSymbol( methodSyntax );

                ExpressionSyntax criterias;
                ISymbol memberSymbol;
                if( !FindCriterias( analysisContext, methodSymbol, out criterias, out memberSymbol ) )
                    return;

                var diagnostic = Diagnostic.Create( _Rule, methodSymbol.Locations[ 0 ], methodSymbol.Name );
                analysisContext.ReportDiagnostic( diagnostic );
            }
            catch( Exception e )
            {
                throw;
            }
        }


        private const string _SmartTestClassName = "SmartTests.SmartTest";


        private static bool FindCriterias( SyntaxNodeAnalysisContext context, IMethodSymbol method,
                                           out ExpressionSyntax criterias, out ISymbol memberSymbol )
        {
            criterias = null;
            memberSymbol = null;
            if( method.MethodKind != MethodKind.Ordinary )
                return false;

            var model = context.SemanticModel;
            if( !method.IsTestMethod( model ) )
                return false;

            var declaringType = method.ReceiverType;
            if( !declaringType.IsTestClass( model ) )
                return false;

            // It is a test
            // Does it has criterias?
            var invocations = context.Node.DescendantNodes().OfType<InvocationExpressionSyntax>();
            var smartTest = model.Compilation.GetTypeByMetadataName( _SmartTestClassName );
            var runTestMethods = smartTest.GetMethods( "RunTest" );
            var caseMethods = smartTest.GetMethods( "Case" );
            foreach( var runTestInvocationSyntax in invocations )
                if( FindCriterias( model, runTestInvocationSyntax, runTestMethods, caseMethods,
                                   ref criterias, ref memberSymbol ) )
                    return true;
            return false;
        }


        private static bool FindCriterias( SemanticModel model, InvocationExpressionSyntax runTestInvocationSyntax, IMethodSymbol[] runTestMethods, IMethodSymbol[] caseMethods,
                                           ref ExpressionSyntax criterias, ref ISymbol memberSymbol )
        {
            criterias = null;
            memberSymbol = null;
            if( !model.HasMethod( runTestInvocationSyntax, runTestMethods ) )
                return false;

            // We have a call to SmartTest.RunTests method
            // => Collect criterias
            var argument0Syntax = runTestInvocationSyntax.GetArgument( 0 );
            if( argument0Syntax == null )
                return false;

            var arg0InvocationSyntax = argument0Syntax.Expression as InvocationExpressionSyntax;
            if( !model.HasMethod( arg0InvocationSyntax, caseMethods ) )
                return false;
            // ReSharper disable once AssignNullToNotNullAttribute
            criterias = arg0InvocationSyntax.GetArgument( 0 )?.Expression;

            // Search for the tests method
            var argument1Syntax = runTestInvocationSyntax.GetArgument( 1 );
            var lambda = argument1Syntax.Expression as ParenthesizedLambdaExpressionSyntax;
            if( lambda == null )
                return false;
            memberSymbol = model.GetSymbolInfo( lambda.Body ).Symbol;
            return memberSymbol != null;
        }
    }
}