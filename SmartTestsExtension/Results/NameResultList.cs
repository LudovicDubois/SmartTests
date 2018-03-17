using System.Collections.ObjectModel;
using System.Linq;



namespace SmartTestsExtension.Results
{
    public class NameResultList: ObservableCollection<NameResult>
    {
        public MemberResult Synchronize( int generation, string[] names, int index )
        {
            var currentName = names[ index ];
            var current = this.FirstOrDefault( named => named.Name == currentName );
            if( current == null )
            {
                // Not found
                // => Create it

                current = index == names.Length - 1
                              ? new MemberResult( currentName )
                              : new NameResult( currentName );
                Add( current );
            }
            current.Generation = generation;

            index++;
            if( index == names.Length )
                return (MemberResult)current;

            return current.SubNames.Synchronize( generation, names, index );
        }


        public void RemoveOld( int generation )
        {
            for( var i = Count - 1; i >= 0; i-- )
            {
                var name = this[ i ];
                if( name.Generation < generation )
                    RemoveAt( i );
                else
                    name.SubNames.RemoveOld( generation );
            }
        }
    }
}