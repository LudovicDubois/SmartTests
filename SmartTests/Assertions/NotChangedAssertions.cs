// ReSharper disable UnusedParameter.Global

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

using SmartTests.Helpers;



namespace SmartTests.Assertions
{
    public static class NotChangedAssertions
    {
        public static Assertion NotChanged( this SmartAssertPlaceHolder @this, object instance ) => new NotChangedAssertion( instance );


        private class NotChangedAssertion: Assertion
        {
            public NotChangedAssertion( object instance )
            {
                _Instance = instance;
            }


            private readonly object _Instance;
            private PropertyInfo[] _Properties;
            private readonly Dictionary<PropertyInfo, object> _PropertyValues = new Dictionary<PropertyInfo, object>();


            public override void BeforeAct( ActBase act )
            {
                _Properties = _Instance.GetType().GetProperties();
                foreach( var property in _Properties )
                    _PropertyValues[ property ] = property.GetValue( _Instance );
            }


            public override void AfterAct( ActBase act )
            {
                var message = new StringBuilder();
                foreach( var pair in _PropertyValues )
                    if( !Equals( _PropertyValues[ pair.Key ], pair.Key.GetValue( _Instance ) ) )
                    {
                        message.AppendLine();
                        message.Append( string.Format( Resource.PropertyChanged, pair.Key.GetFullName() ) );
                    }

                if( message.Length > 0 )
                    throw new SmartTestException( message.ToString( Environment.NewLine.Length, message.Length - Environment.NewLine.Length ) );
            }
        }
    }
}