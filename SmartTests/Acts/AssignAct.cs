using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

using SmartTests.Helpers;



namespace SmartTests.Acts
{
    /// <summary>
    ///     This interface declares parts of an assignment
    /// </summary>
    public interface IAssignee
    {
        /// <summary>
        ///     The current value of the assignee, before assignment.
        /// </summary>
        object AssigneeValue { get; }
        /// <summary>
        ///     The value to assign.
        /// </summary>
        object AssignedValue { get; }
    }


    /// <summary>
    ///     This class represents the Act of assigning a property or indexer.
    /// </summary>
    /// <typeparam name="T">The type of the property or indexer.</typeparam>
    /// <remarks>
    ///     <para>DO NOT USE DIRECTLY.</para>
    ///     <para>Prefer using <see cref="O:SmartTests.SmartTest.RunTest" /> methods.</para>
    /// </remarks>
    /// <seealso cref="SmartTest" />
    public class AssignAct<T>: Act<T>, IAssignee
    {
        /// <summary>
        ///     Creates an instance of <see cref="AssignAct{T}" /> to represent an assignment of a property or indexer in the Act
        ///     part of your test.
        /// </summary>
        /// <param name="assignee">A lambda <see cref="Expression" /> of the assigned member.</param>
        /// <param name="value">The value to be assigned.</param>
        /// <remarks>
        ///     <para>DO NOT USE DIRECTLY.</para>
        ///     <para>Prefer using <see cref="SmartTest.Assign{T}" /> method.</para>
        /// </remarks>
        public AssignAct( Expression<Func<T>> assignee, T value )
        {
            _Assignee = assignee;
            _Value = value;

            if( _Assignee.GetMemberContext( out var instance, out var member, out _Arguments ) )
            {
                Instance = instance;
                Field = member as FieldInfo;
                if( Field != null )
                    return;

                Constructor = member as ConstructorInfo;
                if( Constructor != null )
                    throw new BadTestException( string.Format( Resource.BadTest_NotWritablePropertyNorIndexer, member.GetFullName() ) );

                Method = member as MethodInfo;
                if( Method != null )
                {
                    if( !Method.IsSpecialName )
                        throw new BadTestException( string.Format( Resource.BadTest_NotWritablePropertyNorIndexer, member.GetFullName() ) );
                    //An indexer?
                    foreach( var property in Method.DeclaringType.GetRuntimeProperties() )
                    {
                        if( !Equals( property.GetMethod, Method ) )
                            continue;
                        Property = property;
                        Method = property.SetMethod;
                        break;
                    }
                }
                else
                {
                    Property = (PropertyInfo)member;
                    Method = Property.SetMethod;
                }
            }

            if( Property == null &&
                Field == null &&
                Method == null )
                throw new BadTestException( string.Format( Resource.BadTest_NotWritablePropertyNorIndexer, member.GetFullName() ) );
        }


        private readonly Expression<Func<T>> _Assignee;
        private Func<T> _CompiledAssignee;
        private readonly T _Value;
        private readonly Expression[] _Arguments;


        /// <inheritdoc />
        public override T Invoke( ActContext context )
        {
            if( _Assignee.Body is MemberExpression memberGetExpression )
            {
                var closureExpression = memberGetExpression.Expression as MemberExpression;

                var member = Method != null
                                 ? Expression.Property( closureExpression, Method )
                                 : Expression.Field( closureExpression, Field );
                var lambda = Expression.Lambda( Expression.Assign( member, Expression.Constant( _Value, typeof(T) ) ) ).Compile();

                return (T)lambda.DynamicInvoke();
            }

            var methodCall = (MethodCallExpression)_Assignee.Body;
            var args = new List<Expression>( _Arguments )
                       {
                           Expression.Constant( _Value )
                       };
            var indexerCall = Expression.Call( methodCall.Object, Method, args.ToArray() );
            Expression.Lambda( indexerCall ).Compile().DynamicInvoke();

            return default(T);
        }


        object IAssignee.AssigneeValue
        {
            get
            {
                if( _CompiledAssignee == null )
                    _CompiledAssignee = _Assignee.Compile();
                return _CompiledAssignee();
            }
        }

        /// <inheritdoc />
        public object AssignedValue => _Value;
    }
}