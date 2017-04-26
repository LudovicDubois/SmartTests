using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using SmartTestsAnalyzer.Helpers;



namespace SmartTestsAnalyzer
{
    static class SmartTestsDiagnostics
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

        private static readonly DiagnosticDescriptor _WrongParameterName = new DiagnosticDescriptor( "SmartTestsAnalyzer_WrongParameterName",
                                                                                                     LocalizeString( nameof( Resources.WrongParameterName_Title ) ),
                                                                                                     LocalizeString( nameof( Resources.WrongParameterName_MessageFormat ) ),
                                                                                                     _Category,
                                                                                                     DiagnosticSeverity.Error,
                                                                                                     true,
                                                                                                     LocalizeString( nameof( Resources.WrongParameterName_Description ) ) );

        public static ImmutableArray<DiagnosticDescriptor> DiagnosticDescriptors { get; } = ImmutableArray.Create( _MissingCases,
                                                                                                                   _MissingParameterCases,
                                                                                                                   _WrongParameterName
                                                                                                                 );


        public static Diagnostic CreateMissingCase( ISymbol testedMember, string parameterName, IEnumerable<ExpressionSyntax> criterias, string errorMessage )
        {
            var rule = parameterName == MemberTestCases.NoParameter ? _MissingCases : _MissingParameterCases;
            return Diagnostic.Create( rule,
                                      criterias.First().GetLocation(),
                                      criterias.Skip( 1 ).Select( criteria => criteria.GetLocation() ),
                                      testedMember.GetTypeAndMemberName(),
                                      errorMessage,
                                      parameterName
                                    );
        }

        public static Diagnostic CreateWrongParameterName( ISymbol testedMember, string parameterName, ExpressionSyntax parameterNameExpression )
        {
            return Diagnostic.Create( _WrongParameterName,
                                      parameterNameExpression?.GetLocation(),
                                      testedMember.GetTypeAndMemberName(),
                                      parameterName
                                    );
        }

        /*
                context.ReportDiagnostic(Diagnostic.Create(rule,
                                                             criteriaExpression.First().GetLocation(),
                                                             criteriaExpression.Skip(1).Select(criteria => criteria.GetLocation()),
                                                             testedMember.GetTypeAndMemberName(),
                                                             errorMessage,
                                                             parameterName));
*/

    }
}