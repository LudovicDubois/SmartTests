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
        public Dictionary<TestedMember, MemberTestCases> MemberCases { get; } = new Dictionary<TestedMember, MemberTestCases>();


        public MemberTestCases GetOrCreate( TestedMember testedMember )
        {
            MemberTestCases cases;
            if( MemberCases.TryGetValue( testedMember, out cases ) )
            {
                return cases;
            }

            cases = new MemberTestCases( testedMember );
            MemberCases[ testedMember ] = cases;
            return cases;
        }


        public void Validate( INamedTypeSymbol errorType, Action<Diagnostic> reportError )
        {
            foreach( var memberCasesValue in MemberCases.Values )
                memberCasesValue.Validate( errorType, reportError );
        }
    }
}