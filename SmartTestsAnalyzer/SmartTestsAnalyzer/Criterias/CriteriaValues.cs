using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;

using SmartTestsAnalyzer.Ranges;



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
            // ReSharper disable once InconsistentNaming
            public bool IsError { get; }
        }


        public List<CriteriaValue> Values { get; } = new List<CriteriaValue>();


        public void Add( CriteriaAnalysis analysis, bool isError ) => Values.Add( new CriteriaValue( analysis, isError ) );


        public abstract void AddCurrentValues();


        // ReSharper disable once UnusedMember.Global
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
        where TRange: class, ISymbolicNumericType<T>, new()
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
            var errors = new TRange();
            missing.Range( missing.MinValue, missing.MaxValue );
            missing.RemoveRange( currentRange, errors );
            // LD: What to do with errors?
            if( missing.Chunks.Count > 0 )
                Values.Add( new CriteriaValue( new RangeAnalysis( missing, false ), false ) );
        }
    }


    public class EnumValues: CriteriaValues
    {
        public override void AddCurrentValues() => CompleteWithMissingValues( GetAllCurrentValues() );
        public override void AddMissingValues() => CompleteWithMissingValues( GetAllCurrentValues() );


        private HashSet<IFieldSymbol> GetAllCurrentValues()
        {
            var result = new HashSet<IFieldSymbol>();
            foreach( var criteriaValue in Values )
            {
                var rangeAnalysis = (RangeAnalysis)criteriaValue.Analysis;
                foreach( var value in ( (EnumTypeAnalyzer)rangeAnalysis.Type ).Values )
                    result.Add( value );
            }

            return result;
        }


        private void CompleteWithMissingValues( HashSet<IFieldSymbol> currentValues )
        {
            var first = currentValues.First();
            var type = first.ContainingType;
            var missing = type.GetMembers()
                              .Where( m => m.Kind == SymbolKind.Field )
                              .Cast<IFieldSymbol>()
                              .Where( value => !currentValues.Contains( value ) )
                              .ToArray();

            if( missing.Length > 0 )
                // ReSharper disable once CoVariantArrayConversion
                Values.Add( new CriteriaValue( new RangeAnalysis( new EnumTypeAnalyzer( null, missing ), false ), false ) );
        }
    }
}