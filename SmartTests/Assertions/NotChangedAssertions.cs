// ReSharper disable UnusedParameter.Global

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

using SmartTests.Helpers;



namespace SmartTests.Assertions
{
    [Flags]
    public enum NoChangedKind
    {
        PublicProperties = 0b0000_00001,
        NonPublicProperties = 0b0000_0010,
        AllProperties = PublicProperties | NonPublicProperties,
    }


    public static class NotChangedAssertions
    {
        public static Assertion NotChanged( this SmartAssertPlaceHolder @this, object instance, NoChangedKind kind = NoChangedKind.PublicProperties ) => new NotChangedAssertion( instance, kind );


        private class NotChangedAssertion: Assertion
        {
            public NotChangedAssertion( object instance, NoChangedKind kind )
            {
                _Instance = instance;
                _Kind = kind;
            }


            private readonly object _Instance;
            private readonly NoChangedKind _Kind;
            private PropertyInfo[] _Properties;
            private readonly Dictionary<PropertyInfo, object> _PropertyValues = new Dictionary<PropertyInfo, object>();


            private static readonly Dictionary<NoChangedKind, BindingFlags> _Flags = new Dictionary<NoChangedKind, BindingFlags>
                                                                                     {
                                                                                         [ NoChangedKind.NonPublicProperties ] = BindingFlags.Instance | BindingFlags.NonPublic,
                                                                                         [ NoChangedKind.PublicProperties ] = BindingFlags.Instance | BindingFlags.Public,
                                                                                         [ NoChangedKind.AllProperties ] = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                                                                                     };


            public override void BeforeAct( ActBase act )
            {
                _Properties = _Instance.GetType().GetProperties( _Flags[ _Kind ] );
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