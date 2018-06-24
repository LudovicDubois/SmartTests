using System.Collections.Generic;

using SmartTests.Ranges;



namespace SmartTestsAnalyzer.Criterias
{
    public abstract class CriteriaValues
    {
        public struct CriteriaValue
        {
            public CriteriaValue( CriteriaAnalysis analysis, bool isError )
            {
                Analysis = analysis;
                IsError = isError;
            }


            public CriteriaAnalysis Analysis { get; }
            public bool IsError { get; }
        }


        public List<CriteriaValue> Values { get; } = new List<CriteriaValue>();


        public void Add( CriteriaAnalysis analysis, bool isError ) => Values.Add( new CriteriaValue( analysis, isError ) );


        public abstract void CompleteValues();
    }


    public class FieldValues: CriteriaValues
    {
        public override void CompleteValues()
        { }
    }


    public class RangeValues: CriteriaValues
    {
        public override void CompleteValues() => CompleteWithMissingChunks( GetAllCurrentChunks() );


        private IntRange GetAllCurrentChunks()
        {
            var result = new IntRange();
            foreach( var criteriaValue in Values )
            {
                var rangeAnalysis = (RangeAnalysis)criteriaValue.Analysis;
                foreach( var chunk in rangeAnalysis.Range.Chunks )
                    result.Add( chunk.Min, chunk.Max );
            }
            return result;
        }


        private void CompleteWithMissingChunks( IntRange currentRange )
        {
            var missing = new IntRange();
            var value = int.MinValue;
            foreach( var chunk in currentRange.Chunks )
            {
                if( chunk.Min != value )
                    missing.Add( value, chunk.Min - 1 );
                value = chunk.Max + 1;
            }

            if( value < int.MaxValue )
                missing.Add( value, int.MaxValue );

            if( missing.Chunks.Count > 0 )
                Values.Add( new CriteriaValue( new RangeAnalysis( missing ), false ) );
        }
    }
}