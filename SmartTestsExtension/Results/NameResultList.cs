using System.Collections.Generic;
using System.Linq;

using SmartTestsAnalyzer;



namespace SmartTestsExtension.Results
{
    public class NameResultList: List<NameResult>
    {
        public void AddOrUpdate( string[] names, int index, MemberTestCases cases )
        {
            if( index == names.Length - 1 )
            {
                // All names were retrieved
                // => last level is member name
                Add( new MemberResult( names[ index ], cases ) );
                return;
            }

            var currentName = names[ index ];
            var current = this.FirstOrDefault( named => named.Name == currentName );
            if( current == null )
            {
                // Not found
                // => Create it
                current = new NameResult( currentName );
                Add( current );
            }

            current.SubNames.AddOrUpdate( names, index + 1, cases );
        }
    }
}