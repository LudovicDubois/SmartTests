using System.Collections.Generic;

using Microsoft.CodeAnalysis;

using SmartTests.Ranges;



namespace SmartTestsAnalyzer.Criterias
{
    class RangeAnalysis: CriteriaAnalysis
    {
        public RangeAnalysis( IType type )
        {
            Type = type;
        }


        public IType Type { get; }


        public override void AddValues( Dictionary<string, CriteriaValues> values, INamedTypeSymbol errorType )
        {
            if( !values.TryGetValue( "IntRange", out var intRangeValues ) )
            {
                intRangeValues = new RangeValues<int, IntType>();
                values[ "IntRange" ] = intRangeValues;
            }
            intRangeValues.Add( new RangeAnalysis( Type ), false );
        }


        public override bool Equals( object obj ) => Equals( obj as RangeAnalysis );


        protected bool Equals( RangeAnalysis other ) => other?.GetType() == typeof(RangeAnalysis) && Equals( Type, other.Type );


        public override int GetHashCode() => Type?.GetHashCode() ?? 0;


        public override string ToDisplayString( SymbolDisplayFormat displayFormat ) => Type.ToString();
    }
}