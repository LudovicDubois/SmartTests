using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;

using SmartTests.Ranges;



namespace SmartTestsAnalyzer.Criterias
{
    class RangeAnalysis: CriteriaAnalysis
    {
        private static readonly Dictionary<Type, Func<CriteriaValues>> _CriteriaValuesGenerator = new Dictionary<Type, Func<CriteriaValues>>();


        static RangeAnalysis()
        {
            _CriteriaValuesGenerator[ typeof(ByteType) ] = () => new RangeValues<byte, ByteType>();
            _CriteriaValuesGenerator[ typeof(SByteType) ] = () => new RangeValues<sbyte, SByteType>();
            _CriteriaValuesGenerator[ typeof(ShortType) ] = () => new RangeValues<short, ShortType>();
            _CriteriaValuesGenerator[ typeof(UShortType) ] = () => new RangeValues<ushort, UShortType>();
            _CriteriaValuesGenerator[ typeof(IntType) ] = () => new RangeValues<int, IntType>();
            _CriteriaValuesGenerator[ typeof(UIntType) ] = () => new RangeValues<uint, UIntType>();
            _CriteriaValuesGenerator[ typeof(LongType) ] = () => new RangeValues<long, LongType>();
            _CriteriaValuesGenerator[ typeof(ULongType) ] = () => new RangeValues<ulong, ULongType>();
        }


        public RangeAnalysis( IType type )
        {
            Type = type;
        }


        public IType Type { get; }


        public override void AddValues( Dictionary<string, CriteriaValues> values, INamedTypeSymbol errorType )
        {
            if( !values.TryGetValue( "Range", out var rangeValues ) )
            {
                rangeValues = _CriteriaValuesGenerator[ Type.GetType() ]();
                values[ "Range" ] = rangeValues;
            }

            rangeValues.Add( new RangeAnalysis( Type ), false );
        }


        public override bool Equals( object obj ) => Equals( obj as RangeAnalysis );


        protected bool Equals( RangeAnalysis other ) => other?.GetType() == typeof(RangeAnalysis) && Equals( Type, other.Type );


        public override int GetHashCode() => Type?.GetHashCode() ?? 0;


        public override string ToDisplayString( SymbolDisplayFormat displayFormat ) => Type.ToString();
    }
}