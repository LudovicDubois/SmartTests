using System.Collections.Generic;

using Microsoft.CodeAnalysis;

using SmartTests.Ranges;



namespace SmartTestsAnalyzer.Criterias
{
    class RangeAnalysis: CriteriaAnalysis
    {
        public RangeAnalysis( IntRange range )
        {
            Range = range;
        }


        public IntRange Range { get; }


        public override void AddValues( Dictionary<string, CriteriaValues> values, INamedTypeSymbol errorType )
        {
            if( !values.TryGetValue( "IntRange", out var intRangeValues ) )
            {
                intRangeValues = new RangeValues();
                values[ "IntRange" ] = intRangeValues;
            }
            intRangeValues.Add( new RangeAnalysis( Range ), false );
        }


        public override bool Equals( object obj ) => Equals( obj as RangeAnalysis );


        protected bool Equals( RangeAnalysis other ) => other?.GetType() == typeof(RangeAnalysis) && Equals( Range, other.Range );


        public override int GetHashCode() => Range?.GetHashCode() ?? 0;


        public override string ToDisplayString( SymbolDisplayFormat displayFormat ) => Range.ToString();
    }
}