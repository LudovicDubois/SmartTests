namespace SmartTests.Criterias
{
    /// <summary>
    ///     Provides criterions for a string with specific format
    /// </summary>
    // ReSharper disable once UnusedMember.Global
    public class FormattedString: Criteria
    {
        /// <summary>
        ///     When the value is <c>null</c>
        ///     <para>THIS IS AN ERROR</para>
        /// </summary>
        [Error]
        public static readonly Criteria IsNull = new FormattedString();
        /// <summary>
        ///     When the value is empty (<c>""</c>)
        ///     <para>THIS IS AN ERROR</para>
        /// </summary>
        [Error]
        public static readonly Criteria IsEmpty = new FormattedString();
        /// <summary>
        ///     When the value is not well formatted
        ///     <para>THIS IS AN ERROR</para>
        /// </summary>
        [Error]
        public static readonly Criteria IsInvalid = new FormattedString();
        /// <summary>
        ///     When the value is well formatted
        /// </summary>
        public static readonly Criteria IsValid = new FormattedString();
    }
}