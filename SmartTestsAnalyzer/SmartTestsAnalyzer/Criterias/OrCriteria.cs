namespace SmartTestsAnalyzer.Criterias
{
    class OrCriteria: BinaryCriteria
    {
        public OrCriteria( CriteriaSymbolExpression leftCriteria, CriteriaSymbolExpression rightCriteria )
            : base( leftCriteria, rightCriteria )
        {}
    }
}