namespace SmartTests.Criterias
{
    /// <summary>
    ///     Provides Criterions for a collection management
    /// </summary>
    // ReSharper disable once UnusedMember.Global
    public class Collection: Criteria
    {
        /// <summary>
        ///     When the collection is empty
        /// </summary>
        // ReSharper disable UnusedMember.Global
        public static readonly Criteria IsEmpty = new Collection();
        /// <summary>
        ///     When the collection has 1 item only
        /// </summary>
        public static readonly Criteria HasOneItem = new Collection();
        /// <summary>
        ///     When the collection has more than one item
        /// </summary>
        public static readonly Criteria HasManyItems = new Collection();
        // ReSharper restore UnusedMember.Global
    }
}