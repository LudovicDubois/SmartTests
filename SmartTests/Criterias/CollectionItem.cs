using JetBrains.Annotations;



namespace SmartTests.Criterias
{
    /// <summary>
    ///     Provides criterions for a collection item
    /// </summary>
    [PublicAPI]
    public class CollectionItem: Criteria
    {
        /// <summary>
        ///     When the collection item is <c>null</c>
        ///     <para>THIS IS AN ERROR</para>
        /// </summary>
        [Error]
        public static readonly Criteria IsNull = new CollectionItem();
        /// <summary>
        ///     When the item is not in the collection
        /// </summary>
        public static readonly Criteria IsNotInCollection = new CollectionItem();
        /// <summary>
        ///     When the item is in the collection
        /// </summary>
        public static readonly Criteria IsInCollection = new CollectionItem();
    }
}