using System.Reflection;

using NUnit.Framework;

using SmartTests;



namespace SmartTestsAnalyzer.Runtime.Test
{
    public class ActValidator: Assertion
    {
        public ActValidator( ConstructorInfo constructor )
        {
            _Constructor = constructor;
        }


        public ActValidator( object instance, FieldInfo field )
        {
            _Instance = instance;
            _Field = field;
        }


        public ActValidator( object instance, MethodInfo method )
        {
            _Instance = instance;
            _Method = method;
        }


        public ActValidator( object instance, PropertyInfo property, MethodInfo method )
        {
            _Instance = instance;
            _Method = method;
            _Property = property;
        }


        private readonly object _Instance;
        private readonly ConstructorInfo _Constructor;
        private readonly FieldInfo _Field;
        private readonly MethodInfo _Method;
        private readonly PropertyInfo _Property;


        public override void BeforeAct( ActBase act )
        { }


        public override void AfterAct( ActBase act )
        {
            Assert.AreEqual( _Instance, act.Instance );
            Assert.AreEqual( _Constructor, act.Constructor );
            Assert.AreEqual( _Field, act.Field );
            Assert.AreEqual( _Method, act.Method );
            Assert.AreEqual( _Property, act.Property );
        }
    }
}