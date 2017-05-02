using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;



namespace SmartTestsAnalyzer
{
    [DiagnosticAnalyzer( LanguageNames.CSharp )]
    public partial class SmartTestsAnalyzerAnalyzer: DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => SmartTestsDiagnostics.DiagnosticDescriptors;


        public Dictionary<ISymbol, MemberTestCases> Tests { get; private set; }


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

                visitor.MembersTestCases.Validate( context.ReportDiagnostic );
            }
            catch( Exception e )
            {
                Debug.WriteLine( e.Message );
                throw;
            }
        }
    }
}