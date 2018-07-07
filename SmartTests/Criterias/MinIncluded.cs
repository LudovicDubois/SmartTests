namespace SmartTests.Criterias
{
    /// <summary>
    ///     Provides criterions for a value that should be above or equal to a lower bound
    /// </summary>
    // ReSharper disable once UnusedMember.Global
    public class MinIncluded: Criteria
    {
        /// <summary>
        ///     When the value is below the lower bound
        ///     <para>THIS IS AN ERROR</para>
        /// </summary>
        [Error]
        public static readonly Criteria IsBelowMin = new MinIncluded();
        /// <summary>
        ///     When the value is equal to the lower bound
        /// </summary>
        public static readonly Criteria IsMin = new MinIncluded();
        /// <summary>
        ///     When the value is above the lower bound
        /// </summary>
        public static readonly Criteria IsAboveMin = new MinIncluded();
    }
}