using JetBrains.Annotations;



namespace SmartTests.Criterias
{
    [PublicAPI]
    public class MinIncMaxExc: Criteria
    {
        [Error]
        public static readonly Criteria IsBelowMin = new MinIncMaxInc();
        public static readonly Criteria IsMin = new MinIncMaxInc();
        public static readonly Criteria IsBetweenMinMax = new MinIncMaxInc();
        [Error]
        public static readonly Criteria IsMax = new MinIncMaxInc();
        [Error]
        public static readonly Criteria IsAboveMax = new MinIncMaxInc();
    }
}