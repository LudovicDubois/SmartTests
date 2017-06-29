using JetBrains.Annotations;



namespace SmartTests.Criterias
{
    /// <summary>
    ///     Provides criterions for a value that should be compared with another value
    /// </summary>
    [PublicAPI]
    public class Comparison: Criteria
    {
        /// <summary>
        ///     When the value is lesser than another value
        /// </summary>
        public static readonly Criteria IsBelow = new Comparison();
        /// <summary>
        ///     When the value is equal to another value
        /// </summary>
        public static readonly Criteria IsEqual = new Comparison();
        /// <summary>
        ///     When the value is greater than another value
        /// </summary>
        public static readonly Criteria IsAbove = new Comparison();
    }
}