namespace SmartTestsAnalyzer.Criterias
{
    class ImpliesCriteria: BinaryCriteria
    {
        public ImpliesCriteria( CriteriaSymbolExpression leftCriteria, CriteriaSymbolExpression rightCriteria )
            : base( leftCriteria, rightCriteria )
        {}
    }
}