using JetBrains.Annotations;



namespace SmartTests.Criterias
{
    [PublicAPI]
    public class AnyValue: Criteria
    {
        public static readonly Criteria IsValid = new AnyValue();
    }
}