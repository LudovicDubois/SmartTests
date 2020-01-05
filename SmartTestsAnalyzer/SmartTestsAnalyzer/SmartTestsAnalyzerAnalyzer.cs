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
    // ReSharper disable once PartialTypeWithSinglePart
    public partial class SmartTestsAnalyzerAnalyzer: DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => SmartTestsDiagnostics.DiagnosticDescriptors;


        public Tests Tests { get; private set; }


        public override void Initialize( AnalysisContext context )
        {
            // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/Analyzer%20Actions%20Semantics.md for more information
            _AlreadyDone = false;
            context.RegisterSemanticModelAction( Analyze );
        }


        private bool _AlreadyDone; // To avoid repetition of the errors (6 times when compiling and I do not know why :-()


        private void Analyze( SemanticModelAnalysisContext context )
        {
            if( _AlreadyDone )
                return;
            if( context.CancellationToken.IsCancellationRequested )
                return;
            _AlreadyDone = true;
            try
            {
                var visitor = new TestVisitor( context );
                if( !visitor.IsSmartTestProject )
                    return;

                context.SemanticModel.Compilation.SourceModule.Accept( visitor );
                if( context.CancellationToken.IsCancellationRequested )
                    return;
                Tests = visitor.MembersTestCases.MemberCases;

                visitor.Validate( context.ReportDiagnostic );
                if( context.CancellationToken.IsCancellationRequested )
                    return;

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
                var projectFolder = GetProjectFolder( context.SemanticModel.Compilation );
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


        private string GetProjectFolder( Compilation compilation )
        {
            // Hope we will find a Properties\AssemblyInfo.cs file
            foreach( var attribute in compilation.Assembly.GetAttributes() )
            {
                var fileName = attribute.ApplicationSyntaxReference.SyntaxTree.FilePath;
                if( fileName.EndsWith( _AssemblyInfoPath ) )
                    return fileName.Substring( 0, fileName.Length - _AssemblyInfoPath.Length );
            }

            return null;
        }
    }
}