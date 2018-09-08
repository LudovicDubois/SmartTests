using System;
using System.Collections.Generic;
using System.Linq;

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


        public abstract void AddCurrentValues();
        public abstract void AddMissingValues();
    }


    public class FieldValues: CriteriaValues
    {
        public override void AddCurrentValues()
        { }


        public override void AddMissingValues()
        { }
    }


    public class RangeValues<T, TRange>: CriteriaValues
        where T: struct, IComparable<T>
        where TRange: class, INumericType<T>, new()
    {
        public override void AddCurrentValues() => CompleteWithMissingChunks( GetAllCurrentChunks() );
        public override void AddMissingValues() => CompleteWithMissingChunks( GetAllCurrentChunks() );


        private TRange GetAllCurrentChunks()
        {
            var result = new TRange();
            foreach( var criteriaValue in Values )
            {
                var rangeAnalysis = (RangeAnalysis)criteriaValue.Analysis;
                foreach( var chunk in ( (TRange)rangeAnalysis.Type ).Chunks )
                    result.Range( chunk.Min, chunk.MinIncluded, chunk.Max, chunk.MaxIncluded );
            }

            return result;
        }


        private void CompleteWithMissingChunks( TRange currentRange )
        {
            var missing = new TRange();
            if( currentRange.Chunks.Count == 0 )
            {
                missing.Range( missing.MinValue, missing.MaxValue );
                Values.Add( new CriteriaValue( new RangeAnalysis( missing, false ), false ) );
                return;
            }

            var value = missing.MinValue;
            var valueIncluded = true;
            foreach( var chunk in currentRange.Chunks )
            {
                if( chunk.IncludedMin.CompareTo( value ) != 0 )
                    missing.Range( value, valueIncluded, chunk.Min, !chunk.MinIncluded );
                value = chunk.Max;
                valueIncluded = !chunk.MaxIncluded;
            }

            if( currentRange.Chunks.Last().IncludedMax.CompareTo( missing.MaxValue ) < 0 )
                missing.Range( value, valueIncluded, missing.MaxValue, true );

            if( missing.Chunks.Count > 0 )
                Values.Add( new CriteriaValue( new RangeAnalysis( missing, false ), false ) );
        }
    }
}