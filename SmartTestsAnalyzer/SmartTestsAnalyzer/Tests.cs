using System.Collections.Generic;



namespace SmartTestsAnalyzer
{
#if EXTENSION
    public class Tests: Dictionary<string, MemberTestCases>
    { }


#else


    public class Tests: Dictionary<TestedMember, MemberTestCases>
    { }


#endif
}