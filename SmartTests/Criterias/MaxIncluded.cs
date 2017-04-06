using JetBrains.Annotations;



namespace SmartTests.Criterias
{
    [PublicAPI]
    public class MaxIncluded: Criteria
    {
        public static readonly Criteria IsBelowMax = new MaxIncluded();
        public static readonly Criteria IsMax = new MaxIncluded();
        [Error]
        public static readonly Criteria IsAboveMax = new MaxIncluded();
    }
}