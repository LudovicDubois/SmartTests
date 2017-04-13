namespace SmartTestsAnalyzer.Criterias
{
    class BinaryCriteria: CriteriaSymbolExpression
    {
        protected BinaryCriteria( CriteriaSymbolExpression leftCriteria, CriteriaSymbolExpression rightCriteria )
        {
            LeftCriteria = leftCriteria;
            RightCriteria = rightCriteria;
        }


        public CriteriaSymbolExpression LeftCriteria { get; }
        public CriteriaSymbolExpression RightCriteria { get; }
    }
}