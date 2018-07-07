namespace SmartTests.Criterias
{
    /// <summary>
    ///     Provides a single criteria that is always valid.
    /// </summary>
    public class AnyValue: Criteria
    {
        /// <summary>
        ///     When the value is always valid
        /// </summary>
        public static readonly Criteria IsValid = new AnyValue();
    }
}