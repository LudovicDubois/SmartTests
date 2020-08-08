using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;

using SmartTests.Ranges;

using SmartTestsAnalyzer.Ranges;



namespace SmartTestsAnalyzer.Criterias
{
    class RangeAnalysis: CriteriaAnalysis
    {
        private static readonly Dictionary<Type, Func<CriteriaValues>> _CriteriaValuesGenerator = new Dictionary<Type, Func<CriteriaValues>>();


        static RangeAnalysis()
        {
            _CriteriaValuesGenerator[ typeof(SymbolicByteType) ] = () => new RangeValues<byte, SymbolicByteType>();
            _CriteriaValuesGenerator[ typeof(SymbolicSByteType) ] = () => new RangeValues<sbyte, SymbolicSByteType>();
            _CriteriaValuesGenerator[ typeof(SymbolicInt16Type) ] = () => new RangeValues<short, SymbolicInt16Type>();
            _CriteriaValuesGenerator[ typeof(SymbolicUInt16Type) ] = () => new RangeValues<ushort, SymbolicUInt16Type>();
            _CriteriaValuesGenerator[ typeof(SymbolicInt32Type) ] = () => new RangeValues<int, SymbolicInt32Type>();
            _CriteriaValuesGenerator[ typeof(SymbolicUInt32Type) ] = () => new RangeValues<uint, SymbolicUInt32Type>();
            _CriteriaValuesGenerator[ typeof(SymbolicInt64Type) ] = () => new RangeValues<long, SymbolicInt64Type>();
            _CriteriaValuesGenerator[ typeof(SymbolicUInt64Type) ] = () => new RangeValues<ulong, SymbolicUInt64Type>();
            _CriteriaValuesGenerator[ typeof(SymbolicSingleType) ] = () => new RangeValues<float, SymbolicSingleType>();
            _CriteriaValuesGenerator[ typeof(SymbolicDoubleType) ] = () => new RangeValues<double, SymbolicDoubleType>();
            _CriteriaValuesGenerator[ typeof(SymbolicDecimalType) ] = () => new RangeValues<decimal, SymbolicDecimalType>();
            _CriteriaValuesGenerator[ typeof(SymbolicDateTimeType) ] = () => new RangeValues<DateTime, SymbolicDateTimeType>();
            _CriteriaValuesGenerator[ typeof(EnumTypeAnalyzer) ] = () => new EnumValues();
        }


        public RangeAnalysis( IType type, bool isError )
        {
            Type = type;
            IsError = isError;
        }


        public IType Type { get; }
        // ReSharper disable once InconsistentNaming
        // ReSharper disable once MemberCanBePrivate.Global
        public bool IsError { get; }


        private static readonly TestedParameter _RangeTestedParameter = new TestedParameter( "Range" );


        public override void AddValues( Dictionary<TestedParameter, CriteriaValues> values, INamedTypeSymbol errorType )
        {
            if( !values.TryGetValue( _RangeTestedParameter, out var rangeValues ) )
            {
                rangeValues = _CriteriaValuesGenerator[ Type.GetType() ]();
                values[ _RangeTestedParameter ] = rangeValues;
            }

            rangeValues.Add( new RangeAnalysis( Type, IsError ), IsError );
        }


        public override bool Equals( object obj ) => Equals( obj as RangeAnalysis );


        protected bool Equals( RangeAnalysis other ) => other?.GetType() == typeof(RangeAnalysis) && Equals( Type, other.Type );


        public override int GetHashCode() => Type?.GetHashCode() ?? 0;


        public override string ToDisplayString( SymbolDisplayFormat displayFormat ) => Type.ToString();
    }
}