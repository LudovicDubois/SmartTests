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
    public partial class SmartTestsAnalyzerAnalyzer: DiagnosticAnalyzer
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
            context.RegisterSemanticModelAction( Analyze );
        }


        private void Analyze( SemanticModelAnalysisContext context )
        {
            try
            {
                var visitor = new TestVisitor( context );
                context.SemanticModel.Compilation.SourceModule.Accept( visitor );

                //context.ReportDiagnostic( new );
            }
            catch( Exception e )
            {
                throw;
            }
        }
    }
}