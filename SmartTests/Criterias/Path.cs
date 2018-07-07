namespace SmartTests.Criterias
{
    /// <summary>
    ///     Provides criterions for path that should be valid
    /// </summary>
    // ReSharper disable once UnusedMember.Global
    public class Path: Criteria
    {
        /// <summary>
        ///     When the path is <c>null</c>
        ///     <para>THIS IS AN ERROR</para>
        /// </summary>
        [Error]
        public static readonly Criteria IsNull = new Path();
        /// <summary>
        ///     When the path is invalid (contains unauthorized characters for example)
        ///     <para>THIS IS AN ERROR</para>
        /// </summary>
        [Error]
        public static readonly Criteria IsInvalid = new Path();
        /// <summary>
        ///     When the path is valid, but does not reference any resource
        ///     <para>THIS IS AN ERROR</para>
        /// </summary>
        [Error]
        public static readonly Criteria IsNonExistent = new Path();
        /// <summary>
        ///     When the path is valid and references a resource
        /// </summary>
        public static readonly Criteria IsValid = new Path();
    }
}