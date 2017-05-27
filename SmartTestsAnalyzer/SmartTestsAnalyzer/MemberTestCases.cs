using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

using JetBrains.Annotations;

using Microsoft.CodeAnalysis;



namespace SmartTestsAnalyzer
{
    /// <summary>
    ///     Test Cases for a tested member, i.e. all combined cases (normalized form) for a tested member.
    /// </summary>
    public class MemberTestCases
    {
        public static string NoParameter => "<No Parameter!>";


        public MemberTestCases( TestedMember testedMember )
        {
            TestedMember = testedMember;
        }


        public TestedMember TestedMember { get; }
        public CasesAndOr Cases { get; } = new CasesAndOr();


        public void CombineAnd( [NotNull] CasesAndOr cases )
        {
            if( cases == null )
                throw new ArgumentNullException( nameof(cases) );

            Cases.CombineAnd( cases );
        }


        public void CombineOr( [NotNull] CasesAndOr cases )
        {
            if( cases == null )
                throw new ArgumentNullException( nameof(cases) );

            Cases.CombineOr( cases );
        }


        public void Validate( INamedTypeSymbol errorType, Action<Diagnostic> reportError )
        {
            if( ValidateParameterNames( reportError ) )
                ValidateCriterias( errorType, reportError );
        }


        private static readonly List<string> _Value = new List<string>
                                                      {
                                                          "value"
                                                      };


        private bool ValidateParameterNames( Action<Diagnostic> reportError )
        {
            switch( TestedMember.Kind )
            {
                case TestedMemberKind.Method:
                    return ValidateParameterNames( reportError,
                                                   GetTestedParameterNames( ( (IMethodSymbol)TestedMember.Symbol ).Parameters ) );

                case TestedMemberKind.PropertyGet:
                    return ValidateNoParameterNames( reportError );

                case TestedMemberKind.PropertySet:
                    return ValidateParameterNames( reportError, _Value );

                case TestedMemberKind.IndexerGet:
                    return ValidateParameterNames( reportError,
                                                   GetTestedParameterNames( ( (IPropertySymbol)TestedMember.Symbol ).Parameters ) );

                case TestedMemberKind.IndexerSet:
                    var names = GetTestedParameterNames( ( (IPropertySymbol)TestedMember.Symbol ).Parameters );
                    names.Add( "value" );
                    return ValidateParameterNames( reportError, names );

                default:
                    throw new NotImplementedException();
            }
        }


        private bool ValidateNoParameterNames( Action<Diagnostic> reportError )
        {
            var result = true;
            foreach( var casesAnd in Cases.CasesAnd )
            foreach( var pair in casesAnd.Cases )
                if( pair.Key != NoParameter )
                {
                    result = false;
                    reportError( SmartTestsDiagnostics.CreateWrongParameterName( TestedMember, pair.Key, pair.Value.ParameterNameExpression ) );
                }
            return result;
        }


        private List<string> GetTestedParameterNames( ImmutableArray<IParameterSymbol> parameters )
            => parameters.Where( p => p.RefKind != RefKind.Out ).Select( p => p.Name ).ToList();


        private bool ValidateParameterNames( Action<Diagnostic> reportError, List<string> parameters )
        {
            if( parameters.Count <= 1 )
            {
                // 0 or 1 parameter: the parameter name is not mandatory
                if( Cases.CasesAnd.Count == 1 &&
                    Cases.CasesAnd[ 0 ].Cases.Count == 1 &&
                    Cases.CasesAnd.First().Cases.Keys.First() == NoParameter )
                    return true;
            }

            var result = true;
            foreach( var casesAnd in Cases.CasesAnd )
            {
                var unusedParameters = parameters.ToList();
                bool hasAnonymousCase = false;
                foreach( var aCase in casesAnd.Cases.Values )
                {
                    if( aCase.ParameterName == NoParameter )
                    {
                        hasAnonymousCase = true;
                        continue;
                    }

                    if( unusedParameters.Remove( aCase.ParameterName ) )
                        continue;

                    // This parameter name does not exist
                    result = false;
                    reportError( SmartTestsDiagnostics.CreateWrongParameterName( TestedMember, aCase.ParameterName, aCase.ParameterNameExpression ) );
                }

                if( casesAnd.HasError )
                    continue;

                // Remaining parameters have no Case!
                if( parameters.Count == 1 &&
                    hasAnonymousCase )
                    // When 1 parameter, parameter name is not mandatory
                    continue;

                foreach( var parameter in unusedParameters )
                {
                    result = false;
                    reportError( SmartTestsDiagnostics.CreateMissingParameterCase( TestedMember, parameter, casesAnd.Cases.First().Value.CaseExpressions.First() ) );
                }
            }

            return result;
        }


        private void ValidateCriterias( INamedTypeSymbol errorType, Action<Diagnostic> reportError )
        {
            Cases.Validate( TestedMember, errorType, reportError );
        }
    }
}