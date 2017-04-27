using System;
using System.Collections.Generic;
using System.Linq;

using JetBrains.Annotations;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;



namespace SmartTestsAnalyzer
{
    /// <summary>
    ///     Test Cases for a tested member, i.e. all combined criteria (normalized form) for a tested member.
    /// </summary>
    class MemberTestCases
    {
        public static string NoParameter => "<No Parameter!>";


        public MemberTestCases( ISymbol testedMember )
        {
            TestedMember = testedMember;
        }


        public ISymbol TestedMember { get; }
        public Dictionary<string, CombinedCriteriasCollection> Criterias { get; } = new Dictionary<string, CombinedCriteriasCollection>();


        public void Add( string parameterName, [NotNull] CombinedCriteriasCollection criterias )
        {
            if( criterias == null )
                throw new ArgumentNullException( nameof( criterias ) );

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


        private bool ValidateParameterNames( Action<Diagnostic> reportError )
        {
            var methodSymbol = TestedMember as IMethodSymbol;
            return methodSymbol == null 
                ? ValidateNoParameterNames( reportError ) 
                : ValidateParameterNames( reportError, methodSymbol );
        }


        private bool ValidateNoParameterNames( Action<Diagnostic> reportError )
        {
            return true;
        }


        private bool ValidateParameterNames( Action<Diagnostic> reportError, IMethodSymbol methodSymbol )
        {
            if( methodSymbol.Parameters.Length <= 1 )
            {
                // 0 or 1 parameter: the parameter name is not mandatory
                if( Criterias.Count == 1 && Criterias.First().Key == NoParameter )
                    return true;
            }

            var parameters = methodSymbol.Parameters.Where( p => p.RefKind != RefKind.Out ).ToDictionary( par => par.Name );
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
                reportError( SmartTestsDiagnostics.CreateWrongParameterName( methodSymbol, parameterName, criterias.Value.ParameterNameExpression ) );
            }

            // Remaining parameters have no Case!
            foreach( var parameter in parameters )
                reportError( SmartTestsDiagnostics.CreateMissingParameterCase( methodSymbol, parameter.Key, Criterias.First().Value.CasesExpression ) );
            return result;
        }


        private void ValidateCriterias( Action<Diagnostic> reportError )
        {
            foreach( var criterias in Criterias )
                criterias.Value.Validate( TestedMember, criterias.Key, reportError );
        }
    }
}