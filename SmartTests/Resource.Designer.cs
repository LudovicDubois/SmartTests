﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SmartTests {
    using System;
    using System.Reflection;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resource {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resource() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("SmartTests.Resource", typeof(Resource).GetTypeInfo().Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to BAD TEST: ActContext.SetHandle expected.
        /// </summary>
        internal static string BadTest_ExpectedContextSetHandle {
            get {
                return ResourceManager.GetString("BadTest_ExpectedContextSetHandle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to BAD TEST: Time should be strictly positive, but was {0}ms.
        /// </summary>
        internal static string BadTest_NegativeTimeSpan {
            get {
                return ResourceManager.GetString("BadTest_NegativeTimeSpan", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to BAD TEST: Act is not an assignment.
        /// </summary>
        internal static string BadTest_NotAssignment {
            get {
                return ResourceManager.GetString("BadTest_NotAssignment", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to BAD TEST: &apos;{0}&apos; is not an event of type &apos;{1}&apos;.
        /// </summary>
        internal static string BadTest_NotEvent {
            get {
                return ResourceManager.GetString("BadTest_NotEvent", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to BAD TEST: class &apos;{0}&apos; does not implement INotifyPropertyChanged.
        /// </summary>
        internal static string BadTest_NotINotifyPropertyChanged {
            get {
                return ResourceManager.GetString("BadTest_NotINotifyPropertyChanged", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to BAD TEST: &apos;{0}&apos; is not a property.
        /// </summary>
        internal static string BadTest_NotProperty {
            get {
                return ResourceManager.GetString("BadTest_NotProperty", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to BAD TEST: &apos;{0}&apos; is not a property nor a field of type &apos;{1}&apos;.
        /// </summary>
        internal static string BadTest_NotPropertyNorField {
            get {
                return ResourceManager.GetString("BadTest_NotPropertyNorField", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to BAD TEST: &apos;{0}&apos; is not a writable property nor indexer.
        /// </summary>
        internal static string BadTest_NotPropertyNorIndexer {
            get {
                return ResourceManager.GetString("BadTest_NotPropertyNorIndexer", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to BAD TEST: &apos;{0}&apos; is not a writable property nor indexer.
        /// </summary>
        internal static string BadTest_NotWritablePropertyNorIndexer {
            get {
                return ResourceManager.GetString("BadTest_NotWritablePropertyNorIndexer", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to BAD TEST: ActContext.SetHandle called, but specified handle expected.
        /// </summary>
        internal static string BadTest_UnexpectedContextSetHandle {
            get {
                return ResourceManager.GetString("BadTest_UnexpectedContextSetHandle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to BAD TEST: unexpected value {0}.
        /// </summary>
        internal static string BadTest_UnexpectedValue {
            get {
                return ResourceManager.GetString("BadTest_UnexpectedValue", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Change is wrong. Expected {0}, but was {1}.
        /// </summary>
        internal static string ChangeWrongly {
            get {
                return ResourceManager.GetString("ChangeWrongly", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Event &apos;{0}&apos; was unexpected.
        /// </summary>
        internal static string ExpectedNotRaisedEvent {
            get {
                return ResourceManager.GetString("ExpectedNotRaisedEvent", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Event &apos;{0}&apos; was expected.
        /// </summary>
        internal static string ExpectedRaisedEvent {
            get {
                return ResourceManager.GetString("ExpectedRaisedEvent", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Field &apos;{0}&apos; has changed.
        /// </summary>
        internal static string FieldChanged {
            get {
                return ResourceManager.GetString("FieldChanged", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Property &apos;{0}&apos; has changed.
        /// </summary>
        internal static string PropertyChanged {
            get {
                return ResourceManager.GetString("PropertyChanged", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Exception&apos;s Message should be &apos;{0}&apos;, but was &apos;{1}&apos;.
        /// </summary>
        internal static string ThrowBadMessage {
            get {
                return ResourceManager.GetString("ThrowBadMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Exception&apos;s ParamName should be &apos;{0}&apos;, but was &apos;{1}&apos;.
        /// </summary>
        internal static string ThrowBadParameterName {
            get {
                return ResourceManager.GetString("ThrowBadParameterName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Exception &apos;{0}&apos; was expected.
        /// </summary>
        internal static string ThrowNoException {
            get {
                return ResourceManager.GetString("ThrowNoException", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Exception &apos;{0}&apos; was expected, but was &apos;{1}&apos;.
        /// </summary>
        internal static string ThrowWrongException {
            get {
                return ResourceManager.GetString("ThrowWrongException", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Timeout reached ({0}ms).
        /// </summary>
        internal static string TimeoutReached {
            get {
                return ResourceManager.GetString("TimeoutReached", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Should be less than {0}ms, but was {1}ms.
        /// </summary>
        internal static string TimespanExceeded {
            get {
                return ResourceManager.GetString("TimespanExceeded", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unexpected property name &apos;{0}&apos; when PropertyChanged event was raised.
        /// </summary>
        internal static string UnexpectedPropertyNameWhenPropertyChangedRaised {
            get {
                return ResourceManager.GetString("UnexpectedPropertyNameWhenPropertyChangedRaised", resourceCulture);
            }
        }
    }
}
