using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;

using SmartTests;



namespace SmartTestsAnalyzer
{
    public class SymbolicConstant<T>: IEquatable<SymbolicConstant<T>>
        where T: IComparable<T>
    {
        // Represent an unknown value
        public SymbolicConstant()
        {
            Unknown = true;
        }


        public SymbolicConstant( T constant )
        {
            Constant = constant;
        }


        public SymbolicConstant( IFieldSymbol field )
        {
            Field = field;
        }


        public SymbolicConstant( IPropertySymbol property )
        {
            Property = property;
        }


        public bool Unknown { get; }
        public T Constant { get; }
        public IFieldSymbol Field { get; }
        public IPropertySymbol Property { get; }
        public ISymbol Symbol => (ISymbol)Field ?? Property;

        public bool Symbolic => Symbol != null || Unknown;


        public bool ConstantGreaterThan( SymbolicConstant<T> other ) => !Symbolic && Constant.CompareTo( other.Constant ) > 0;


        public bool Equals( SymbolicConstant<T> other ) =>
            other != null &&
            Unknown == other.Unknown &&
            Constant.Equals( other.Constant ) &&
            Equals( Field, other.Field ) &&
            Equals( Property, other.Property );


        public override bool Equals( object obj ) => Equals( obj as SymbolicConstant<T> );


        public override int GetHashCode() =>
            EqualityComparer<T>.Default.GetHashCode( Constant ) ^
            Unknown.GetHashCode() ^
            ( Field != null ? Field.GetHashCode() : 0 ) ^
            ( Property != null ? Property.GetHashCode() : 0 );


        public override string ToString() =>
            Unknown
                ? "?"
                : Symbol?.ToDisplayString( SymbolDisplayFormat.CSharpShortErrorMessageFormat )
                  ?? SmartTest.ToString( Constant );
    }
}