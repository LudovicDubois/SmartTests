using SmartTestsAnalyzer;



namespace SmartTestsExtension.Results
{
    public class NameResult
    {
        public NameResult( string name )
        {
            Name = name;
        }


        public string Name { get; protected set; }


        public NameResultList SubNames { get; } = new NameResultList();
    }
}