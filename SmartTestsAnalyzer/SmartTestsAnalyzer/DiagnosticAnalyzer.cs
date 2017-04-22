using System;
using System.Collections.Immutable;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

using SmartTestsAnalyzer.Helpers;



namespace SmartTestsAnalyzer
{
    [DiagnosticAnalyzer( LanguageNames.CSharp )]
    public partial class SmartTestsAnalyzerAnalyzer: DiagnosticAnalyzer
    {
        // You can change these strings in the Resources.resx file. If you do not want your analyzer to be localize-able, you can use regular strings for Title and MessageFormat.
        // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/Localizing%20Analyzers.md for more on localization
        private const string _Category = "Tests";

        private static LocalizableResourceString LocalizeString( string resourceId ) => new LocalizableResourceString( resourceId, Resources.ResourceManager, typeof(Resources) );
        private static readonly DiagnosticDescriptor _MissingCases = new DiagnosticDescriptor( "SmartTestsAnalyzer_MissingCases",
                                                                                               LocalizeString( nameof( Resources.MissingCases_Title ) ),
                                                                                               LocalizeString( nameof( Resources.MissingCases_MessageFormat ) ),
                                                                                               _Category,
                                                                                               DiagnosticSeverity.Warning,
                                                                                               true,
                                                                                               LocalizeString( nameof( Resources.MissingCases_Description ) ) );
        private static readonly DiagnosticDescriptor _MissingParameterCases = new DiagnosticDescriptor( "SmartTestsAnalyzer_MissingParameterCases",
                                                                                                        LocalizeString( nameof( Resources.MissingParameterCases_Title ) ),
                                                                                                        LocalizeString( nameof( Resources.MissingParameterCases_MessageFormat ) ),
                                                                                                        _Category,
                                                                                                        DiagnosticSeverity.Warning,
                                                                                                        true,
                                                                                                        LocalizeString( nameof( Resources.MissingParameterCases_Description ) ) );

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create( _MissingCases,
                                                                                                            _MissingParameterCases );


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
                if( !visitor.IsTestProject )
                    return;

                context.SemanticModel.Compilation.SourceModule.Accept( visitor );

                visitor.MembersTestCases.Validate( ( criteriaExpression, testedMember, parameterName, errorMessage ) =>
                                                   {
                                                       var rule = string.IsNullOrEmpty( parameterName ) ? _MissingCases : _MissingParameterCases;
                                                       context.ReportDiagnostic( Diagnostic.Create( rule,
                                                                                                    criteriaExpression.First().GetLocation(),
                                                                                                    criteriaExpression.Skip( 1 ).Select( criteria => criteria.GetLocation() ),
                                                                                                    testedMember.GetTypeAndMemberName(),
                                                                                                    errorMessage,
                                                                                                    parameterName ) );
                                                   } );
            }
            catch( Exception e )
            {
                throw;
            }
        }
    }
}