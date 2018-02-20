using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

using JetBrains.Annotations;

using Microsoft.CodeAnalysis;

using Newtonsoft.Json;



namespace SmartTestsAnalyzer
{
    /// <summary>
    ///     Test Cases for a tested member, i.e. all combined cases (normalized form) for a tested member.
    /// </summary>
    [UsedImplicitly( ImplicitUseTargetFlags.WithMembers )]
    public class MemberTestCases
    {
        public MemberTestCases( TestedMember testedMember )
        {
            TestedMember = testedMember;
            TestedMemberKind = testedMember.Kind;
            TestedType = testedMember.Symbol.ContainingType.ToDisplayString( SymbolDisplayFormat.FullyQualifiedFormat );
            TestedMemberName = testedMember.Symbol.Name;
        }


        [JsonIgnore]
        public TestedMember TestedMember { get; }

        public string TestedType { get; }
        public TestedMemberKind TestedMemberKind { get; }
        public string TestedMemberName { get; }

        public CasesAndOr CasesAndOr { get; } = new CasesAndOr();


        public void CombineOr( [NotNull] CasesAndOr cases )
        {
            if( cases == null )
                throw new ArgumentNullException( nameof(cases) );

            CasesAndOr.CombineOr( cases );
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
            foreach( var casesAnd in CasesAndOr.CasesAnd )
            foreach( var pair in casesAnd.Cases )
                if( pair.Key != Case.NoParameter )
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
                if( CasesAndOr.CasesAnd.Count == 1 &&
                    CasesAndOr.CasesAnd[ 0 ].Cases.Count == 1 &&
                    CasesAndOr.CasesAnd.First().Cases.Keys.First() == Case.NoParameter )
                    return true;
            }

            var result = true;
            foreach( var casesAnd in CasesAndOr.CasesAnd )
            {
                var unusedParameters = parameters.ToList();
                var hasAnonymousCase = false;
                foreach( var aCase in casesAnd.Cases.Values )
                {
                    if( aCase.ParameterName == Case.NoParameter )
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


        private void ValidateCriterias( INamedTypeSymbol errorType, Action<Diagnostic> reportError ) => CasesAndOr.Validate( TestedMember, errorType, reportError );
    }
}