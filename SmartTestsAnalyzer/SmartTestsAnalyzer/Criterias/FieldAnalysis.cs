using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using Microsoft.CodeAnalysis;

using SmartTestsAnalyzer.Helpers;



namespace SmartTestsAnalyzer.Criterias
{
    class FieldAnalysis: CriteriaAnalysis
    {
        public FieldAnalysis( IFieldSymbol field )
        {
            Debug.Assert( field != null, "field != null" );
            Field = field;
        }


        public IFieldSymbol Field { get; }

        public virtual ITypeSymbol ContainingType => Field.ContainingType;


        public override void AddValues( Dictionary<string, CriteriaValues> values, INamedTypeSymbol errorType )
        {
            if( values.ContainsKey( ContainingType.Name ) )
                // Already added
                return;

            var value = new CriteriaValues();
            values[ ContainingType.Name ] = value;
            foreach( var criterion in ContainingType.GetMembers().Where( member => member is IFieldSymbol ).Cast<IFieldSymbol>() )
                value.Add( new FieldAnalysis( criterion ), criterion.HasAttribute( errorType ) );
        }


        public override string ToDisplayString( SymbolDisplayFormat displayFormat ) => Field.ToDisplayString( displayFormat );


        public override bool Equals( object obj ) => Equals( obj as FieldAnalysis );


        public bool Equals( FieldAnalysis other ) => Equals( Field, other?.Field );


        public override int GetHashCode() => Field != null ? Field.GetHashCode() : 0;
    }
}