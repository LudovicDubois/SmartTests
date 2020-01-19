namespace SmartTests.Criterias
{
    /// <summary>
    ///     Provides criterions for an optional parameter
    /// </summary>
    // ReSharper disable once UnusedMember.Global
    public class Optional: Criteria
    {
        /// <summary>
        ///     When the optional value is not provided
        /// </summary>
        // ReSharper disable UnusedMember.Global
        public static readonly Criteria Absent = new ValidString();
        /// <summary>
        ///     When the optional value is provided
        /// </summary>
        public static readonly Criteria Present = new ValidString();
        // ReSharper restore UnusedMember.Global
    }
}