namespace SmartTests.Criterias
{
    /// <summary>
    ///     Provides criterions for two values comparison
    /// </summary>
    // ReSharper disable once UnusedMember.Global
    public class Equality: Criteria
    {
        /// <summary>
        ///     When two values are different
        /// </summary>
        // ReSharper disable UnusedMember.Global
        public static readonly Criteria AreDifferent = new Equality();
        /// <summary>
        ///     When two values are similar (<c>object.Equals</c> returns <c>true</c>, but <c>object.ReferenceEquals</c> returns
        ///     <c>false</c>)
        /// </summary>
        public static readonly Criteria AreEqual = new Equality();
        /// <summary>
        ///     When two values are identical (<c>object.ReferenceEquals</c> returns <c>true</c>)
        /// </summary>
        public static readonly Criteria AreSame = new Equality();
        // ReSharper restore UnusedMember.Global
    }
}