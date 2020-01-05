namespace SmartTests.Criterias
{
    /// <summary>
    ///     Provides criterions for the validity of a value (can be either valid or not)
    /// </summary>
    /// <remarks>
    ///     Can be useful for nullable references where <c>null</c> is invalid, for example.
    /// </remarks>
    // ReSharper disable once UnusedMember.Global
    public class ValidValue: Criteria
    {
        /// <summary>
        ///     When the value is valid
        /// </summary>
        // ReSharper disable UnusedMember.Global
        public static readonly Criteria IsValid = new ValidValue();
        /// <summary>
        ///     When the value is invalid
        ///     <para>THIS IS AN ERROR</para>
        /// </summary>
        [Error]
        public static readonly Criteria IsInvalid = new ValidValue();
        // ReSharper restore UnusedMember.Global
    }
}