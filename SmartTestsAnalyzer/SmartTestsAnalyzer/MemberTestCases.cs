using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

using JetBrains.Annotations;

using Microsoft.CodeAnalysis;



namespace SmartTestsAnalyzer
{
    /// <summary>
    ///     Test Cases for a tested member, i.e. all combined criteria (normalized form) for a tested member.
    /// </summary>
    public class MemberTestCases
    {
        public static string NoParameter => "<No Parameter!>";


        public MemberTestCases( TestedMember testedMember )
        {
            TestedMember = testedMember;
        }


        public TestedMember TestedMember { get; }
        public Dictionary<string, CombinedCriteriasCollection> Criterias { get; } = new Dictionary<string, CombinedCriteriasCollection>();


        public void Add( string parameterName, [NotNull] CombinedCriteriasCollection criterias )
        {
            if( criterias == null )
                throw new ArgumentNullException( nameof(criterias) );

            CombinedCriteriasCollection currentCriterias;
            if( !Criterias.TryGetValue( parameterName ?? NoParameter, out currentCriterias ) )
            {
                Criterias[ parameterName ?? NoParameter ] = criterias;
                return;
            }

            currentCriterias.Add( criterias );
        }


        public void Validate( Action<Diagnostic> reportError )
        {
            if( ValidateParameterNames( reportError ) )
                ValidateCriterias( reportError );
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
            // Should not have any parameter name in cases
            var result = true;
            foreach( var criteria in Criterias )
                if( criteria.Key != NoParameter )
                {
                    result = false;
                    reportError( SmartTestsDiagnostics.CreateWrongParameterName( TestedMember, criteria.Key, criteria.Value.ParameterNameExpression ) );
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
                if( Criterias.Count == 1 && Criterias.First().Key == NoParameter )
                    return true;
            }

            var result = true;
            foreach( var criterias in Criterias )
            {
                var parameterName = criterias.Key;
                if( parameterName == NoParameter )
                    continue;

                if( parameters.Remove( parameterName ) )
                    continue;

                // This parameter name does not exist
                result = false;
                reportError( SmartTestsDiagnostics.CreateWrongParameterName( TestedMember, parameterName, criterias.Value.ParameterNameExpression ) );
            }

            // Remaining parameters have no Case!
            foreach( var parameter in parameters )
                reportError( SmartTestsDiagnostics.CreateMissingParameterCase( TestedMember, parameter, Criterias.First().Value.CaseExpression ) );
            return result;
        }


        private void ValidateCriterias( Action<Diagnostic> reportError )
        {
            foreach( var criterias in Criterias )
                criterias.Value.Validate( TestedMember, criterias.Key, reportError );
        }
    }
}