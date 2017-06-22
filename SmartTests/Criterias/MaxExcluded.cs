using JetBrains.Annotations;



namespace SmartTests.Criterias
{
    [PublicAPI]
    public class MaxExcluded: Criteria
    {
        public static readonly Criteria IsBelowMax = new MaxExcluded();
        [Error]
        public static readonly Criteria IsMax = new MaxExcluded();
        [Error]
        public static readonly Criteria IsAboveMax = new MaxExcluded();
    }
}