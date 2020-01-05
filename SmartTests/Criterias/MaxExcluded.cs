namespace SmartTests.Criterias
{
    /// <summary>
    ///     Provides criterions for a value that should be below an upper bound
    /// </summary>
    // ReSharper disable once UnusedMember.Global
    public class MaxExcluded: Criteria
    {
        /// <summary>
        ///     When the value is below the upper bound
        /// </summary>
        // ReSharper disable UnusedMember.Global
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
        // ReSharper restore UnusedMember.Global
    }
}