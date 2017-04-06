using JetBrains.Annotations;



namespace SmartTests.Criterias
{
    [PublicAPI]
    public class ValidValue: Criteria
    {
        public static readonly Criteria Valid = new ValidValue();
        [Error]
        public static readonly Criteria Invalid = new ValidValue();
    }
}