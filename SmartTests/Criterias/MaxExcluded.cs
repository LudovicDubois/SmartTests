using JetBrains.Annotations;



namespace SmartTests.Criterias
{
    /// <summary>
    ///     Provides criterions for a value that should be below an upper bound
    /// </summary>
    [PublicAPI]
    public class MaxExcluded: Criteria
    {
        /// <summary>
        ///     When the value is below the upper bound
        /// </summary>
        public static readonly Criteria IsBelowMax = new MaxExcluded();
        /// <summary>
        ///     When the value is equal to the upper bound
        ///     <para>THIS IS AN ERROR</para>
        /// </summary>
        [Error]
        public static readonly Criteria IsMax = new MaxExcluded();
        /// <summary>
        ///     When the value is above the upper bound
        ///     <para>THIS IS AN ERROR</para>
        /// </summary>
        [Error]
        public static readonly Criteria IsAboveMax = new MaxExcluded();
    }
}