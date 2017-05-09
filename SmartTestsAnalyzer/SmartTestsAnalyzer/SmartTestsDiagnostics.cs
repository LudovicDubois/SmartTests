using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;



namespace SmartTestsAnalyzer
{
    static class SmartTestsDiagnostics
    {
        // You can change these strings in the Resources.resx file. If you do not want your analyzer to be localize-able, you can use regular strings for Title and MessageFormat.
        // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/Localizing%20Analyzers.md for more on localization
        private const string _Category = "Tests";

        private static LocalizableResourceString LocalizeString( string resourceId ) => new LocalizableResourceString( resourceId, Resources.ResourceManager, typeof(Resources) );
        private static readonly DiagnosticDescriptor _MissingCases = new DiagnosticDescriptor( "SmartTestsAnalyzer_MissingCases",
                                                                                               LocalizeString( nameof(Resources.MissingCases_Title) ),
                                                                                               LocalizeString( nameof(Resources.MissingCases_MessageFormat) ),
                                                                                               _Category,
                                                                                               DiagnosticSeverity.Warning,
                                                                                               true,
                                                                                               LocalizeString( nameof(Resources.MissingCases_Description) ) );
        private static readonly DiagnosticDescriptor _MissingParameterCases = new DiagnosticDescriptor( "SmartTestsAnalyzer_MissingParameterCases",
                                                                                                        LocalizeString( nameof(Resources.MissingParameterCases_Title) ),
                                                                                                        LocalizeString( nameof(Resources.MissingParameterCases_MessageFormat) ),
                                                                                                        _Category,
                                                                                                        DiagnosticSeverity.Warning,
                                                                                                        true,
                                                                                                        LocalizeString( nameof(Resources.MissingParameterCases_Description) ) );

        private static readonly DiagnosticDescriptor _WrongParameterName = new DiagnosticDescriptor( "SmartTestsAnalyzer_WrongParameterName",
                                                                                                     LocalizeString( nameof(Resources.WrongParameterName_Title) ),
                                                                                                     LocalizeString( nameof(Resources.WrongParameterName_MessageFormat) ),
                                                                                                     _Category,
                                                                                                     DiagnosticSeverity.Error,
                                                                                                     true,
                                                                                                     LocalizeString( nameof(Resources.WrongParameterName_Description) ) );

        private static readonly DiagnosticDescriptor _MissingParameterCase = new DiagnosticDescriptor( "SmartTestsAnalyzer_MissingParameterCase",
                                                                                                       LocalizeString( nameof(Resources.MissingParameterCase_Title) ),
                                                                                                       LocalizeString( nameof(Resources.MissingParameterCase_MessageFormat) ),
                                                                                                       _Category,
                                                                                                       DiagnosticSeverity.Error,
                                                                                                       true,
                                                                                                       LocalizeString( nameof(Resources.MissingParameterCase_Description) ) );

        public static ImmutableArray<DiagnosticDescriptor> DiagnosticDescriptors { get; } = ImmutableArray.Create( _MissingCases,
                                                                                                                   _MissingParameterCases,
                                                                                                                   _WrongParameterName,
                                                                                                                   _MissingParameterCase
                                                                                                                 );


        public static Diagnostic CreateMissingCase( TestedMember testedMember, string parameterName, IEnumerable<ExpressionSyntax> criterias, string errorMessage )
        {
            var criteriaList = criterias.ToList();
            var first = criteriaList.First();
            criteriaList.RemoveAt( 0 );
            var rule = parameterName == MemberTestCases.NoParameter ? _MissingCases : _MissingParameterCases;
            return Diagnostic.Create( rule,
                                      first.GetLocation(),
                                      criteriaList.Select( criteria => criteria.GetLocation() ),
                                      testedMember.ToString(),
                                      errorMessage,
                                      parameterName
                                    );
        }


        public static Diagnostic CreateWrongParameterName( TestedMember testedMember, string parameterName, ExpressionSyntax parameterNameExpression )
        {
            return Diagnostic.Create( _WrongParameterName,
                                      parameterNameExpression?.GetLocation(),
                                      testedMember.ToString(),
                                      parameterName
                                    );
        }


        public static Diagnostic CreateMissingParameterCase( TestedMember testedMember, string parameterName, SyntaxNode criteriaExpression )
        {
            return Diagnostic.Create( _MissingParameterCase,
                                      criteriaExpression.GetLocation(),
                                      testedMember.ToString(),
                                      parameterName
                                    );
        }
    }
}