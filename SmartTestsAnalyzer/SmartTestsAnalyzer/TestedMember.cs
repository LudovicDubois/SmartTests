using System;

using Microsoft.CodeAnalysis;



namespace SmartTestsAnalyzer
{
    public class TestedMember
    {
        public TestedMember( ISymbol symbol, TestedMemberKind kind )
        {
            Symbol = symbol;
            Kind = kind;
        }


        public ISymbol Symbol { get; }
        public TestedMemberKind Kind { get; }


        public override bool Equals( object other ) => other is TestedMember && Equals( (TestedMember)other );


        public bool Equals( TestedMember other ) => Equals( Symbol, other.Symbol ) && Kind == other.Kind;


        public override int GetHashCode()
        {
            unchecked
            {
                return ( ( Symbol != null ? Symbol.GetHashCode() : 0 ) * 397 ) ^ (int)Kind;
            }
        }


        public override string ToString()
        {
            var typeAndMemberName = Symbol.ToDisplayString( SymbolDisplayFormat.CSharpErrorMessageFormat );

            switch( Kind )
            {
                case TestedMemberKind.Method:
                case TestedMemberKind.Field:
                    return typeAndMemberName;                    

                case TestedMemberKind.PropertyGet:
                    return typeAndMemberName + " [get]";
                case TestedMemberKind.PropertySet:
                    return typeAndMemberName + " [set]";

                case TestedMemberKind.IndexerGet:
                    return typeAndMemberName + " [get]";
                case TestedMemberKind.IndexerSet:
                    return typeAndMemberName + " [set]";

                default:
                    throw new NotImplementedException();
            }
        }
    }
}