using System.Collections.Generic;

using Microsoft.CodeAnalysis;



namespace SmartTestsAnalyzer
{
    class MembersTestCases
    {
        public Dictionary<ISymbol, List<MemberTestCase>> MemberCases { get; } = new Dictionary<ISymbol, List<MemberTestCase>>();


        public void Add( MemberTestCase testCase )
        {
            List<MemberTestCase> cases;
            if( !MemberCases.TryGetValue( testCase.Member, out cases ) )
            {
                cases = new List<MemberTestCase>();
                MemberCases[ testCase.Member ] = cases;
            }

            cases.Add( testCase );
        }
    }
}