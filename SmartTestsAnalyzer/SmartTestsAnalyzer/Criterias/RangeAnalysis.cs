using System;
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
            throw new NotImplementedException();
        }


        public override string ToDisplayString( SymbolDisplayFormat displayFormat ) => Range.ToString();
    }
}