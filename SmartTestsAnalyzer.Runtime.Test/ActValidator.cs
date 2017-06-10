using System.Reflection;

using NUnit.Framework;

using SmartTests;



namespace SmartTestsAnalyzer.Runtime.Test
{
    public class ActValidator: Assertion
    {
        public ActValidator( object instance, FieldInfo field )
        {
            _Instance = instance;
            _Field = field;
        }


        public ActValidator( object instance, MethodBase method )
        {
            _Instance = instance;
            _Method = method;
        }


        public ActValidator( object instance, PropertyInfo property, MethodBase method )
        {
            _Instance = instance;
            _Method = method;
            _Property = property;
        }


        private readonly object _Instance;
        private readonly FieldInfo _Field;
        private readonly MethodBase _Method;
        private readonly PropertyInfo _Property;


        public override void BeforeAct( ActBase act )
        { }


        public override void AfterAct( ActBase act )
        {
            Assert.AreEqual( _Instance, act.Instance );
            Assert.AreEqual( _Field, act.Field );
            Assert.AreEqual( _Method, act.Method );
            Assert.AreEqual( _Property, act.Property );
        }
    }
}