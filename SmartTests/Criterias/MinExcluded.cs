using JetBrains.Annotations;



namespace SmartTests.Criterias
{
    /// <summary>
    ///     Provides criterions for a value that should be above a lower bound
    /// </summary>
    [PublicAPI]
    public class MinExcluded: Criteria
    {
        /// <summary>
        ///     When the value is below the lower bound
        ///     <para>THIS IS AN ERROR</para>
        /// </summary>
        [Error]
        public static readonly Criteria IsBelowMin = new MinExcluded();
        /// <summary>
        ///     When the value is equal to the lower bound
        ///     <para>THIS IS AN ERROR</para>
        /// </summary>
        [Error]
        public static readonly Criteria IsMin = new MinExcluded();
        /// <summary>
        ///     When the value is above the lower bound
        /// </summary>
        public static readonly Criteria IsAboveMin = new MinExcluded();
    }
}