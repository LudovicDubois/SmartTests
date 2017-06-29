using JetBrains.Annotations;



namespace SmartTests.Criterias
{
    /// <summary>
    ///     Provides Criterions for a collection management
    /// </summary>
    [PublicAPI]
    public class Collection: Criteria
    {
        /// <summary>
        ///     When the collection is empty
        /// </summary>
        public static readonly Criteria IsEmpty = new Collection();
        /// <summary>
        ///     When the collection has 1 item only
        /// </summary>
        public static readonly Criteria HasOneItem = new Collection();
        /// <summary>
        ///     When the collection has more than one item
        /// </summary>
        public static readonly Criteria HasManyItems = new Collection();
    }
}