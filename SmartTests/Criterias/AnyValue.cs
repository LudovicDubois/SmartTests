using JetBrains.Annotations;



namespace SmartTests.Criterias
{
    [PublicAPI]
    public class AnyValue: Criteria
    {
        public static readonly Criteria Valid = new AnyValue();
    }
}