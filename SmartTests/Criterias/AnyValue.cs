using JetBrains.Annotations;



namespace SmartTests.Criterias
{
    /// <summary>
    ///     Provides a single criteria that is always valid.
    /// </summary>
    [PublicAPI]
    public class AnyValue: Criteria
    {
        /// <summary>
        ///     When the value is always valid
        /// </summary>
        public static readonly Criteria IsValid = new AnyValue();
    }
}