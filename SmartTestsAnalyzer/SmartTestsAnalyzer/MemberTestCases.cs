#if !EXTENSION
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

using Microsoft.CodeAnalysis;

using Newtonsoft.Json;

#endif



namespace SmartTestsAnalyzer
{
    /// <summary>
    ///     Test Cases for a tested member, i.e. all combined cases (normalized form) for a tested member.
    /// </summary>
    public class MemberTestCases
    {
#if EXTENSION
        public string TestedType { get; set; }
        public TestedMemberKind TestedMemberKind { get; set; }
        public string TestedMemberName { get; set; }

        public CasesAndOr CasesAndOr { get; set; }

#else
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


        public void CombineOr( CasesAndOr cases )
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
                                                   GetTestedParameters( ( (IMethodSymbol)TestedMember.Symbol ).Parameters ) );

                case TestedMemberKind.PropertyGet:
                    return ValidateNoParameterNames( reportError );

                case TestedMemberKind.PropertySet:
                    return ValidateParameterNames( reportError, _Value.Select( v => Tuple.Create( v, (ITypeSymbol)null ) ).ToList() );

                case TestedMemberKind.IndexerGet:
                    return ValidateParameterNames( reportError,
                                                   GetTestedParameters( ( (IPropertySymbol)TestedMember.Symbol ).Parameters ) );

                case TestedMemberKind.IndexerSet:
                    var names = GetTestedParameters( ( (IPropertySymbol)TestedMember.Symbol ).Parameters );
                    names.Add( Tuple.Create( "value", (ITypeSymbol)null ) );
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


        private List<Tuple<string, ITypeSymbol>> GetTestedParameters( ImmutableArray<IParameterSymbol> parameters )
            => parameters.Where( p => p.RefKind != RefKind.Out ).Select( p => Tuple.Create( p.Name, p.Type ) ).ToList();


        private bool ValidateParameterNames( Action<Diagnostic> reportError, List<Tuple<string, ITypeSymbol>> parameters )
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
                var unusedParameters = parameters.ToDictionary( p => p.Item1 );
                var hasAnonymousCase = false;
                foreach( var aCase in casesAnd.Cases.Values )
                {
                    if( aCase.ParameterName == Case.NoParameter )
                    {
                        hasAnonymousCase = true;
                        continue;
                    }

                    if( !unusedParameters.TryGetValue( aCase.ParameterName, out var lambdaParameter ) )
                    {
                        // This parameter name does not exist
                        result = false;
                        reportError( SmartTestsDiagnostics.CreateWrongParameterName( TestedMember, aCase.ParameterName, aCase.ParameterNameExpression ) );
                        continue;

                    }

                    if( lambdaParameter.Item2 != null &&
                        aCase.ParameterType != null &&
                        !lambdaParameter.Item2.Equals( aCase.ParameterType ) )
                    {
                        // This parameter type is not the right one
                        result = false;
                        reportError( SmartTestsDiagnostics.CreateWrongParameterType( TestedMember, aCase.ParameterName, aCase.ParameterType.ToString(), aCase.ParameterNameExpression ) );
                    }

                    unusedParameters.Remove( aCase.ParameterName );
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
                    reportError( SmartTestsDiagnostics.CreateMissingParameterCase( TestedMember, parameter.Key, casesAnd.Cases.First().Value.CaseExpressions.First() ) );
                }
            }

            return result;
        }


        private void ValidateCriterias( INamedTypeSymbol errorType, Action<Diagnostic> reportError ) => CasesAndOr.Validate( TestedMember, errorType, reportError );
#endif
    }
}