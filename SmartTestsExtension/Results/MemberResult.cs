using System.Collections.Generic;
using System.Data;

using SmartTestsAnalyzer;



namespace SmartTestsExtension.Results
{
    public class MemberResult: NameResult
    {
        public MemberResult( string name )
            : base( name )
        {
            _Items = new DataTable();
            _Items.Columns.Add( "Test" );
            _Items.Columns.Add( "TestFileName" );
            _Items.Columns.Add( "TestLine" );
            _Items.Columns.Add( "TestLocation" );
            _Items.Columns.Add( "HasError" );
            _Items.Columns.Add( "IsMissing" );
            Items = new DataView( _Items );
        }


        private readonly DataTable _Items;
        public DataView Items { get; }


        public void Synchronize( MemberTestCases testCases ) => FillWith( testCases.CasesAndOr );


        private void FillWith( CasesAndOr casesAndOr )
        {
            _Items.Clear();
            foreach( var casesAnd in casesAndOr.CasesAnd )
                FillWith( casesAnd );
            Items.Sort = string.Join( ",", GetNewColumnNames() );
        }


        private List<string> GetNewColumnNames()
        {
            var result = new List<string>();
            for( var i = 6; i < _Items.Columns.Count; i++ )
                result.Add( _Items.Columns[ i ].ColumnName );
            return result;
        }


        private void FillWith( CasesAnd casesAnd )
        {
            var row = _Items.NewRow();
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
                    if( !_Items.Columns.Contains( criterion[ 0 ] ) )
                        _Items.Columns.Add( criterion[ 0 ] );
                    row[ criterion[ 0 ] ] = criterion[ 1 ];
                }
            }

            _Items.Rows.Add( row );
        }
    }
}