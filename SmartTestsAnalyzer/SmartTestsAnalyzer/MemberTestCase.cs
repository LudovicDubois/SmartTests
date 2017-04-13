using Microsoft.CodeAnalysis;

using SmartTestsAnalyzer.Criterias;



namespace SmartTestsAnalyzer
{
    class MemberTestCase
    {
        public MemberTestCase( ISymbol member, CriteriaSymbolExpression criteria )
        {
            Member = member;
            Criterias = criteria;
        }


        public ISymbol Member { get; }
        public CriteriaSymbolExpression Criterias { get; }
    }
}