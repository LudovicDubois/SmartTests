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

        private static readonly DiagnosticDescriptor _WrongParameterName = new DiagnosticDescriptor( "SmartTestsAnalyzer_WrongParameterName",
                                                                                                     LocalizeString( nameof(Resources.WrongParameterName_Title) ),
                                                                                                     LocalizeString( nameof(Resources.WrongParameterName_MessageFormat) ),
                                                                                                     _Category,
                                                                                                     DiagnosticSeverity.Error,
                                                                                                     true,
                                                                                                     LocalizeString( nameof(Resources.WrongParameterName_Description) ) );
        private static readonly DiagnosticDescriptor _WrongParameterType = new DiagnosticDescriptor( "SmartTestsAnalyzer_WrongParameterType",
                                                                                                     LocalizeString( nameof(Resources.WrongParameterType_Title) ),
                                                                                                     LocalizeString( nameof(Resources.WrongParameterType_MessageFormat) ),
                                                                                                     _Category,
                                                                                                     DiagnosticSeverity.Error,
                                                                                                     true,
                                                                                                     LocalizeString( nameof(Resources.WrongParameterType_Description) ) );
        private static readonly DiagnosticDescriptor _WrongParameterPath = new DiagnosticDescriptor( "SmartTestsAnalyzer_WrongParameterPath",
                                                                                                     LocalizeString( nameof(Resources.WrongParameterPath_Title) ),
                                                                                                     LocalizeString( nameof(Resources.WrongParameterPath_MessageFormat) ),
                                                                                                     _Category,
                                                                                                     DiagnosticSeverity.Error,
                                                                                                     true,
                                                                                                     LocalizeString( nameof(Resources.WrongParameterPath_Description) ) );

        private static readonly DiagnosticDescriptor _MissingParameterCase = new DiagnosticDescriptor( "SmartTestsAnalyzer_MissingParameterCase",
                                                                                                       LocalizeString( nameof(Resources.MissingParameterCase_Title) ),
                                                                                                       LocalizeString( nameof(Resources.MissingParameterCase_MessageFormat) ),
                                                                                                       _Category,
                                                                                                       DiagnosticSeverity.Error,
                                                                                                       true,
                                                                                                       LocalizeString( nameof(Resources.MissingParameterCase_Description) ) );

        private static readonly DiagnosticDescriptor _ConstantExpected = new DiagnosticDescriptor( "SmartTestsAnalyzer_NotAConstant",
                                                                                                   LocalizeString( nameof(Resources.NotAConstant_Title) ),
                                                                                                   LocalizeString( nameof(Resources.NotAConstant_MessageFormat) ),
                                                                                                   _Category,
                                                                                                   DiagnosticSeverity.Error,
                                                                                                   true,
                                                                                                   LocalizeString( nameof(Resources.NotAConstant_Description) ) );
        private static readonly DiagnosticDescriptor _DateCreationExpected = new DiagnosticDescriptor( "SmartTestsAnalyzer_NotADateCreation",
                                                                                                       LocalizeString( nameof(Resources.NotADate_Title) ),
                                                                                                       LocalizeString( nameof(Resources.NotADate_MessageFormat) ),
                                                                                                       _Category,
                                                                                                       DiagnosticSeverity.Error,
                                                                                                       true,
                                                                                                       LocalizeString( nameof(Resources.NotADate_Description) ) );

        private static readonly DiagnosticDescriptor _MinShouldBeLessThanMax = new DiagnosticDescriptor( "SmartTestsAnalyzer_MinShouldBeLessThanMax",
                                                                                                         LocalizeString( nameof(Resources.MinShouldBeLessThanMax_Title) ),
                                                                                                         LocalizeString( nameof(Resources.MinShouldBeLessThanMax_MessageFormat) ),
                                                                                                         _Category,
                                                                                                         DiagnosticSeverity.Error,
                                                                                                         true,
                                                                                                         LocalizeString( nameof(Resources.MinShouldBeLessThanMax_Description) ) );

        public static ImmutableArray<DiagnosticDescriptor> DiagnosticDescriptors { get; } = ImmutableArray.Create( _MissingCases,
                                                                                                                   _WrongParameterName,
                                                                                                                   _WrongParameterType,
                                                                                                                   _WrongParameterPath,
                                                                                                                   _MissingParameterCase,
                                                                                                                   _ConstantExpected,
                                                                                                                   _DateCreationExpected,
                                                                                                                   _MinShouldBeLessThanMax
                                                                                                                 );


        public static Diagnostic CreateMissingCases( TestedMember testedMember, IEnumerable<ExpressionSyntax> criterias, string errorMessage )
        {
            var criteriaList = criterias.ToList();
            var first = criteriaList.First();
            criteriaList.RemoveAt( 0 );
            return Diagnostic.Create( _MissingCases,
                                      first.GetLocation(),
                                      criteriaList.Select( criteria => criteria.GetLocation() ),
                                      testedMember.ToString(),
                                      errorMessage
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


        public static Diagnostic CreateWrongParameterType( TestedMember testedMember, string parameterName, string parameterType, ExpressionSyntax parameterNameExpression )
        {
            return Diagnostic.Create( _WrongParameterType,
                                      parameterNameExpression?.GetLocation(),
                                      testedMember.ToString(),
                                      parameterType, parameterName
                                    );
        }


        public static Diagnostic CreateWrongParameterPath( TestedMember testedMember, string parameterName, string parameterPath, ExpressionSyntax parameterNameExpression )
        {
            return Diagnostic.Create( _WrongParameterPath,
                                      parameterNameExpression?.GetLocation(),
                                      testedMember.ToString(),
                                      parameterPath, parameterName
                                    );
        }


        public static Diagnostic CreateMissingParameterCase( TestedMember testedMember, string parameterName, SyntaxNode casesExpression )
        {
            return Diagnostic.Create( _MissingParameterCase,
                                      casesExpression.GetLocation(),
                                      testedMember.ToString(),
                                      parameterName
                                    );
        }


        public static Diagnostic CreateNotAConstant( SyntaxNode expression )
        {
            return Diagnostic.Create( _ConstantExpected,
                                      expression.GetLocation()
                                    );
        }


        public static Diagnostic CreateNotADateCreation( SyntaxNode expression )
        {
            return Diagnostic.Create( _DateCreationExpected,
                                      expression.GetLocation()
                                    );
        }


        public static Diagnostic CreateMinShouldBeLessThanMax( InvocationExpressionSyntax node, string min, string max )
        {
            return Diagnostic.Create( _MinShouldBeLessThanMax,
                                      node.GetLocation(),
                                      min,
                                      max );
        }
    }
}