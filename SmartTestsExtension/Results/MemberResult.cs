using System.Data;

using SmartTestsAnalyzer;



namespace SmartTestsExtension.Results
{
    public class MemberResult: NameResult
    {
        public MemberResult( string name, MemberTestCases cases )
            : base( name )
        {
            FillWith( cases.CasesAndOr );
        }


        public DataTable Items { get; } = new DataTable();


        private void FillWith( CasesAndOr casesAndOr )
        {
            Items.Columns.Add( "Test" );
            Items.Columns.Add( "TestLocation" );
            Items.Columns.Add( "HasError" );
            Items.Columns.Add( "IsMissing" );

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