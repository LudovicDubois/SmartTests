using Microsoft.CodeAnalysis;



namespace SmartTestsAnalyzer.Criterias
{
    class AtomicCriteria: CriteriaSymbolExpression
    {
        public AtomicCriteria( ISymbol criteria )
        {
            Criteria = criteria;
        }


        public ISymbol Criteria { get; }
    }
}