using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;



namespace SmartTestsAnalyzer
{
    /// <summary>
    ///     Test cases for all tested members, i.e. all combined criteria (normalized form) for all tested members.
    /// </summary>
    class MembersTestCases
    {
        public Dictionary<ISymbol, MemberTestCases> MemberCases { get; } = new Dictionary<ISymbol, MemberTestCases>();


        public MemberTestCases GetOrCreate( ISymbol testedMember )
        {
            MemberTestCases cases;
            if( MemberCases.TryGetValue( testedMember, out cases ) )
                return cases;

            cases = new MemberTestCases( testedMember );
            MemberCases[ testedMember ] = cases;
            return cases;
        }


        public void Add( ISymbol testedMember, string parameterName, CombinedCriteriasCollection criterias )
        {
            MemberTestCases cases;
            if( !MemberCases.TryGetValue( testedMember, out cases ) )
            {
                cases = new MemberTestCases( testedMember );
                MemberCases[ testedMember ] = cases;
            }
            cases.Add( parameterName, criterias );
        }


        public void Validate( Action<Diagnostic> reportError )
        {
            foreach( var memberCasesValue in MemberCases.Values )
                memberCasesValue.Validate( reportError );
        }
    }
}