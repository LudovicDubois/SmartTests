#if !EXTENSION
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;

using Microsoft.CodeAnalysis;

using Newtonsoft.Json;

#endif

// ReSharper disable MemberCanBePrivate.Global


namespace SmartTestsAnalyzer
{
    /// <summary>
    ///     Test Cases for a tested member, i.e. all combined cases (normalized form) for a tested member.
    /// </summary>
    // ReSharper disable once ClassNeverInstantiated.Global
    public class MemberTestCases
    {
#if EXTENSION
        // ReSharper disable UnusedMember.Global
        public string TestedType { get; set; }
        public TestedMemberKind TestedMemberKind { get; set; }
        public string TestedMemberName { get; set; }
        // ReSharper restore UnusedMember.Global

        // ReSharper disable once UnusedAutoPropertyAccessor.Global
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
            if( ValidateParameters( reportError ) )
                ValidateCriterias( errorType, reportError );
        }


        private static readonly List<string> _Value = new List<string>
                                                      {
                                                          "value"
                                                      };


        private bool ValidateParameters( Action<Diagnostic> reportError )
        {
            switch( TestedMember.Kind )
            {
                case TestedMemberKind.Method:
                    return ValidateParameters( reportError,
                                               GetTestedParameters( ( (IMethodSymbol)TestedMember.Symbol ).Parameters ) );

                case TestedMemberKind.PropertyGet:
                    return ValidateNoParameterNames( reportError );

                case TestedMemberKind.PropertySet:
                    return ValidateParameters( reportError, _Value.Select( v => Tuple.Create( v, (ITypeSymbol)null ) ).ToList() );

                case TestedMemberKind.IndexerGet:
                    return ValidateParameters( reportError,
                                               GetTestedParameters( ( (IPropertySymbol)TestedMember.Symbol ).Parameters ) );

                case TestedMemberKind.IndexerSet:
                    var names = GetTestedParameters( ( (IPropertySymbol)TestedMember.Symbol ).Parameters );
                    names.Add( Tuple.Create( "value", (ITypeSymbol)null ) );
                    return ValidateParameters( reportError, names );

                default:
                    throw new NotImplementedException();
            }
        }


        private bool ValidateNoParameterNames( Action<Diagnostic> reportError )
        {
            var result = true;
            foreach( var casesAnd in CasesAndOr.CasesAnd )
                foreach( var aCase in casesAnd.Cases.Values )
                    if( aCase.TestedParameter.Name != Case.NoParameter )
                    {
                        result = false;
                        reportError( SmartTestsDiagnostics.CreateWrongParameterName( TestedMember, aCase.TestedParameter.Name, aCase.TestedParameter.Expression ) );
                    }

            return result;
        }


        private List<Tuple<string, ITypeSymbol>> GetTestedParameters( ImmutableArray<IParameterSymbol> parameters )
            => parameters.Where( p => p.RefKind != RefKind.Out ).Select( p => Tuple.Create( p.Name, p.Type ) ).ToList();


        private bool ValidateParameters( Action<Diagnostic> reportError, List<Tuple<string, ITypeSymbol>> testedParameters )
        {
            if( testedParameters.Count <= 1 )
            {
                // 0 or 1 parameter: the parameter name is not mandatory
                if( CasesAndOr.CasesAnd.Count == 1 &&
                    CasesAndOr.CasesAnd[ 0 ].Cases.Count == 1 &&
                    CasesAndOr.CasesAnd.First().Cases.Values.First().TestedParameter.Name == Case.NoParameter )
                    return true;
            }

            var result = true;
            foreach( var casesAnd in CasesAndOr.CasesAnd )
            {
                var untestedParameters = testedParameters.ToDictionary( p => p.Item1 );
                var hasAnonymousCase = false;
                foreach( var aCase in casesAnd.Cases.Values )
                {
                    if( aCase.TestedParameter.Name == Case.NoParameter )
                    {
                        hasAnonymousCase = true;
                        continue;
                    }

                    if( !untestedParameters.TryGetValue( aCase.TestedParameter.Name, out var testedParameter ) )
                    {
                        // This parameter name does not exist
                        result = false;
                        reportError( SmartTestsDiagnostics.CreateWrongParameterName( TestedMember, aCase.ParameterName, aCase.TestedParameter.Expression ) );
                        continue;
                    }

                    // This parameter is tested
                    if( aCase.TestedParameter.IsLambda )
                    {
                        if( testedParameter.Item2 != null &&
                            !testedParameter.Item2.Equals( aCase.TestedParameter.Type ) )
                        {
                            // This parameter type is not the right one
                            result = false;
                            reportError( SmartTestsDiagnostics.CreateWrongParameterType( TestedMember, aCase.ParameterName, aCase.TestedParameter.Type.ToString(), aCase.TestedParameter.Expression ) );
                        }

                        if( aCase.TestedParameter.PathHasErrors )
                        {
                            result = false;
                            reportError( SmartTestsDiagnostics.CreateWrongParameterPath( TestedMember, aCase.ParameterName, aCase.TestedParameter.Path, aCase.TestedParameter.PathExpression ) );
                        }
                    }

                    untestedParameters.Remove( aCase.TestedParameter.Name );
                }

                if( casesAnd.HasError )
                    continue;

                // Remaining parameters have no Case!
                if( testedParameters.Count == 1 &&
                    hasAnonymousCase )
                    // When 1 parameter, parameter name is not mandatory
                    continue;

                foreach( var parameter in untestedParameters )
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