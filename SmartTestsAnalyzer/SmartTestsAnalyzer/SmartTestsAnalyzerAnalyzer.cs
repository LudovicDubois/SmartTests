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
                        File.WriteAllText( settings.FullPath, JsonConvert.SerializeObject( Tests ) );
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
            {
                var projectFolder = GetProjectFolder( context.SemanticModel );
                if( projectFolder == null )
                    return null;

                var result2 = new SmartTestsSettings();
                result2.FullPath = Path.Combine( projectFolder, result2.File );
                return result2;
            }

            var result = JsonConvert.DeserializeObject<SmartTestsSettings>( config.GetText( context.CancellationToken ).ToString() );
            result.FullPath = Path.Combine( Path.GetDirectoryName( config.Path ), result.File );
            return result;
        }


        const string _AssemblyInfoPath = @"\Properties\AssemblyInfo.cs";


        private string GetProjectFolder( SemanticModel semanticModel )
        {
            // Hope we will find a Properties\AssemblyInfo.cs file
            foreach( var attribute in semanticModel.Compilation.Assembly.GetAttributes() )
            {
                var fileName = attribute.ApplicationSyntaxReference.SyntaxTree.FilePath;
                if( fileName.EndsWith( _AssemblyInfoPath ) )
                    return fileName.Substring( 0, fileName.Length - _AssemblyInfoPath.Length );
            }
            return null;
        }
    }
}