using JetBrains.Annotations;



namespace SmartTests.Criterias
{
    [PublicAPI]
    public class MinExcluded: Criteria
    {
        [Error]
        public static readonly Criteria IsBelowMin = new MinExcluded();
        [Error]
        public static readonly Criteria IsMin = new MinExcluded();
        public static readonly Criteria IsAboveMin = new MinExcluded();
    }
}