using JetBrains.Annotations;



namespace SmartTests.Criterias
{
    [PublicAPI]
    public class ValidValue: Criteria
    {
        public static readonly Criteria IsValid = new ValidValue();
        [Error]
        public static readonly Criteria IsInvalid = new ValidValue();
    }
}