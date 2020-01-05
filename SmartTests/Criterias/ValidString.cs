namespace SmartTests.Criterias
{
    /// <summary>
    ///     Provides criterions for a string
    /// </summary>
    // ReSharper disable once UnusedMember.Global
    public class ValidString: Criteria
    {
        /// <summary>
        ///     When the value is <c>null</c>
        ///     <para>THIS IS AN ERROR</para>
        /// </summary>
        // ReSharper disable UnusedMember.Global
        [Error]
        public static readonly Criteria IsNull = new ValidString();
        /// <summary>
        ///     When the value is empty (<c>""</c>)
        ///     <para>THIS IS AN ERROR</para>
        /// </summary>
        [Error]
        public static readonly Criteria IsEmpty = new ValidString();
        /// <summary>
        ///     When the value is not well formatted
        /// </summary>
        public static readonly Criteria HasContent = new ValidString();
        // ReSharper restore UnusedMember.Global
    }
}