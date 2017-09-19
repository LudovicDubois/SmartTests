using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

using SmartTests.Helpers;



namespace SmartTests.Acts
{
    public interface IAssignee
    {
        object AssigneeValue { get; }
        object AssignedValue { get; }
    }


    public class AssignAct<T>: Act<T>, IAssignee
    {
        public AssignAct( Expression<Func<T>> assignee, T value )
        {
            _Assignee = assignee;
            _Value = value;

            object instance;
            MemberInfo member;
            if( _Assignee.GetMemberContext( out instance, out member, out _Arguments ) )
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
                        if( property.GetMethod == Method )
                        {
                            Property = property;
                            Method = property.SetMethod;
                            break;
                        }
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


        public override T Invoke()
        {
            var memberGetExpression = _Assignee.Body as MemberExpression;
            if( memberGetExpression != null )
            {
                var closureExpression = memberGetExpression.Expression as MemberExpression;

                var member = Method != null
                                 ? Expression.Property( closureExpression, Method )
                                 : Expression.Field( closureExpression, Field );
                var lambda = Expression.Lambda( Expression.Assign( member, Expression.Constant( _Value ) ) ).Compile();

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

        public object AssignedValue
        {
            get { return _Value; }
        }
    }
}