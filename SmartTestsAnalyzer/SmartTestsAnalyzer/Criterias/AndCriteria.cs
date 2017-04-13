namespace SmartTestsAnalyzer.Criterias
{
    class AndCriteria: BinaryCriteria
    {
        public AndCriteria( CriteriaSymbolExpression leftCriteria, CriteriaSymbolExpression rightCriteria )
            : base( leftCriteria, rightCriteria )
        {}
    }
}