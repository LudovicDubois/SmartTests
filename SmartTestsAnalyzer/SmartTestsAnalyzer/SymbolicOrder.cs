using System;
using System.Collections.Generic;
using System.Linq;

using SmartTestsAnalyzer.Ranges;



namespace SmartTestsAnalyzer
{
    public class SymbolicOrder<T>
        where T: IComparable<T>
    {
        private class OrderNode
        {
            public OrderNode( SymbolicConstant<T> constant )
            {
                Constant = constant;
            }


            public SymbolicConstant<T> Constant { get; }
            public List<OrderNode> Previous { get; } = new List<OrderNode>();
            public List<OrderNode> Next { get; } = new List<OrderNode>();


            public bool HasPrevious( SymbolicConstant<T> value ) => Previous.Any( node => node.Constant.Equals( value ) || node.HasPrevious( value ) );
            public bool HasNext( SymbolicConstant<T> value ) => Next.Any( node => node.Constant.Equals( value ) || node.HasNext( value ) );
        }


        public SymbolicOrder( SymbolicConstant<T> min, SymbolicConstant<T> max )
        {
            _Min = min;
            _Max = max;
        }


        private readonly Dictionary<SymbolicConstant<T>, OrderNode> _Orders = new Dictionary<SymbolicConstant<T>, OrderNode>();
        private readonly SymbolicConstant<T> _Min;
        private readonly SymbolicConstant<T> _Max;


        public bool AddRange( List<SymbolicChunk<T>> chunks )
        {
            var result = true;
            foreach( var chunk in chunks )
                if( !Add( chunk ) )
                    result = false;
            return result;
        }


        public bool Add( SymbolicChunk<T> chunk ) => Add( chunk.Min, chunk.Max );


        private bool Add( SymbolicConstant<T> lower, SymbolicConstant<T> upper )
        {
            if( LessThan( upper, lower ) == true )
                // Cycle!
                return false;

            if( lower.Equals( upper ) )
                return true;

            if( !_Orders.TryGetValue( lower, out var lowerNode ) )
            {
                lowerNode = new OrderNode( lower );
                _Orders.Add( lower, lowerNode );
            }

            if( !_Orders.TryGetValue( upper, out var upperNode ) )
            {
                upperNode = new OrderNode( upper );
                _Orders.Add( upper, upperNode );
            }

            upperNode.Previous.Add( lowerNode );
            lowerNode.Next.Add( upperNode );
            return true;
        }


        public bool? LessThan( SymbolicConstant<T> a, SymbolicConstant<T> b )
        {
            if( !a.Symbolic && !b.Symbolic )
                return a.Constant.CompareTo( b.Constant ) < 0;
            if( a.Equals( _Min ) || b.Equals( _Max ))
                return true;
            if( a.Equals( _Max ) || b.Equals( _Min ) )
                return false;

            // Need to find a path from a to b
            if( !_Orders.TryGetValue( a, out var nodeA ) )
                // ?!? Unable to compare
                return null;
            var result = nodeA.HasNext( b );
            if( result )
                return true;

            result = nodeA.HasPrevious( b );
            if( result )
                return false;

            return null;
        }


        public bool? LessOrEqual( SymbolicConstant<T> a, SymbolicConstant<T> b ) => a.Equals( b ) ? true : LessThan( a, b );

        public bool? GreaterThan( SymbolicConstant<T> a, SymbolicConstant<T> b ) => !LessOrEqual( a, b );

        public bool? GreaterOrEqual( SymbolicConstant<T> a, SymbolicConstant<T> b ) => !LessThan( a, b );
    }
}