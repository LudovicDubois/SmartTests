using System.Data;

using SmartTestsAnalyzer;



namespace SmartTestsExtension.Results
{
    public class MemberResult: NameResult
    {
        public MemberResult( string name )
            : base( name )
        {
            Items.Columns.Add( "Test" );
            Items.Columns.Add( "TestFileName" );
            Items.Columns.Add( "TestLine" );
            Items.Columns.Add( "TestLocation" );
            Items.Columns.Add( "HasError" );
            Items.Columns.Add( "IsMissing" );
        }


        public DataTable Items { get; } = new DataTable();


        public void Synchronize( MemberTestCases testCases ) => FillWith( testCases.CasesAndOr );


        private void FillWith( CasesAndOr casesAndOr )
        {
            Items.Clear();
            foreach( var casesAnd in casesAndOr.CasesAnd )
                FillWith( casesAnd );
        }


        private void FillWith( CasesAnd casesAnd )
        {
            var row = Items.NewRow();
            if( !string.IsNullOrEmpty( casesAnd.TestName ) )
            {
                row[ "Test" ] = casesAnd.TestClassName + '.' + casesAnd.TestName;
                row[ "TestLocation" ] = casesAnd.TestFileName + ':' + casesAnd.TestLine;
                row[ "TestFileName" ] = casesAnd.TestFileName;
                row[ "TestLine" ] = casesAnd.TestLine;
            }
            row[ "HasError" ] = casesAnd.HasError;
            row[ "IsMissing" ] = casesAnd.IsMissing;

            foreach( var cases in casesAnd.Cases )
            {
                foreach( var expression in cases.Value.Expressions )
                {
                    var criterion = expression.Split( '.' );
                    if( !Items.Columns.Contains( criterion[ 0 ] ) )
                        Items.Columns.Add( criterion[ 0 ] );
                    row[ criterion[ 0 ] ] = criterion[ 1 ];
                }
            }

            Items.Rows.Add( row );
        }
    }
}