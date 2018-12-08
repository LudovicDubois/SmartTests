using SmartTestsAnalyzer;



namespace SmartTestsExtension.Results
{
    public class TestsResult
    {
        public NameResultList Names { get; } = new NameResultList();


        private static int _Generation;


        public void Synchronize( Tests tests )
        {
            ++_Generation;
            foreach( var test in tests )
            {
                var nameResult = Names.Synchronize( _Generation, test.Key.Split( '.' ), 0 );
                nameResult.Synchronize( test.Value );
            }
            Names.RemoveOld( _Generation );
        }
    }
}