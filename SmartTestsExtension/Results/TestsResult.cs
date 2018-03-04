using SmartTestsAnalyzer;



namespace SmartTestsExtension.Results
{
    public class TestsResult
    {
        public TestsResult( Tests tests )
        {
            FillWithNamespaces( tests );
        }


        public NameResultList Names { get; } = new NameResultList();


        private void FillWithNamespaces( Tests tests )
        {
            foreach( var test in tests )
                Names.AddOrUpdate( test.Key.Split( '.' ), 0, test.Value );
        }
    }
}