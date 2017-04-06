using JetBrains.Annotations;



namespace SmartTests.Criterias
{
    [PublicAPI]
    public class MinIncluded : Criteria
    {
        [Error]
        public static readonly Criteria IsBelowMin = new MinIncluded();
        public static readonly Criteria IsMin = new MinIncluded();
        public static readonly Criteria IsAboveMin = new MinIncluded();
    }
}