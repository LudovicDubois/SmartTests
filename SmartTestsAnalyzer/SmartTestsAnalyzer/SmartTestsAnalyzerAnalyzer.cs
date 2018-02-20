using System;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

using Newtonsoft.Json;



namespace SmartTestsAnalyzer
{
    [DiagnosticAnalyzer( LanguageNames.CSharp )]
    public partial class SmartTestsAnalyzerAnalyzer: DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => SmartTestsDiagnostics.DiagnosticDescriptors;


        public Tests Tests { get; private set; }


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
                if( !visitor.IsSmartTestProject )
                    return;

                context.SemanticModel.Compilation.SourceModule.Accept( visitor );
                Tests = visitor.MembersTestCases.MemberCases;

                visitor.Validate( context.ReportDiagnostic );

                var settings = GetSettings( context );
                if( settings != null )
                    if( settings.IsEnabled )
                        File.WriteAllText( settings.FullPath, JsonConvert.SerializeObject( visitor.MembersTestCases ) );
                    else
                        File.Delete( settings.FullPath );
            }
            catch( Exception e )
            {
                Debug.WriteLine( e.Message );
                throw;
            }
        }


        private SmartTestsSettings GetSettings( SemanticModelAnalysisContext context )
        {
            var config = context.Options.AdditionalFiles.SingleOrDefault( file => Path.GetFileName( file.Path ) == "SmartTests.json" );
            if( config == null )
                return null;

            var result = JsonConvert.DeserializeObject<SmartTestsSettings>( config.GetText( context.CancellationToken ).ToString() );
            result.FullPath = Path.Combine( Path.GetDirectoryName( config.Path ), result.File );
            return result;
        }
    }
}