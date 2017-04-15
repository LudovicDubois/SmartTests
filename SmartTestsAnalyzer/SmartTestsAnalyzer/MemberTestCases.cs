using System;
using System.Collections.Generic;

using JetBrains.Annotations;

using Microsoft.CodeAnalysis;



namespace SmartTestsAnalyzer
{
    /// <summary>
    ///     Test Cases for a tested member, i.e. all combined criteria (normalized form) for a tested member.
    /// </summary>
    class MemberTestCases
    {
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
            if( !Criterias.TryGetValue( parameterName, out currentCriterias ) )
            {
                Criterias[ parameterName ] = criterias;
                return;
            }

            currentCriterias.Add( criterias );
        }


        public void Validate()
        {
            foreach( var criterias in Criterias.Values )
                criterias.Validate();
        }
    }
}