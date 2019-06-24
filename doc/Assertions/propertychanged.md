# PropertyChanged Smart Assertions

*PropertyChanged Smart Assertions* are assertions that ensure `PropertyChanged` event is raised or not, depending on your Test Logical Intent.

1. Before *Act*
  Registers itself on the `PropertyChanged` event.
1. During *Act*
   1. Setting a flag that the event was raised
   1. Ensuring that the `PropertyName` of the event is expected.
1. After *Act*
  Ensures that the flag is `true` or `false`, as expected, otherwise generates a `SmartTestException`.

> Note that if the instance does not implements `INotifyPropertyChanged`, a `BadTestException` is thrown.

We have different PropertyChanged Smart Assertions:

- [PropertyChanged Smart Assertions](#PropertyChanged-Smart-Assertions)
  - [`Raised_PropertyChanged()`](#RaisedPropertyChanged)
  - [`Raised_PropertyChanged<T>(T,params string[])`](#RaisedPropertyChangedTTparams-string)
  - [`Raised_PropertyChanged<T>(T,string,object)`](#RaisedPropertyChangedTTstringobject)
  - [`Raised_PropertyChanged<T>(Expression<Func<T>>)`](#RaisedPropertyChangedTExpressionFuncT)
  - [`Raised_PropertyChanged<T>(Expression<Func<T>>, T)`](#RaisedPropertyChangedTExpressionFuncT-T)
  - [`NotRaised_PropertyChanged()`](#NotRaisedPropertyChanged)
  - [`NotRaised_PropertyChanged<T>(T)`](#NotRaisedPropertyChangedTT)
  - [`NotRaised_PropertyChanged<T>(T, params string[])`](#NotRaisedPropertyChangedTT-params-string)
  - [`NotRaised_PropertyChanged<T>(Expression<Func<T>>)`](#NotRaisedPropertyChangedTExpressionFuncT)

## `Raised_PropertyChanged()`

The simplest *PropertyChanged* Smart Assertion is `Raised_PropertyChanged()`.

It will ensure that the instance involved in the *Act* raises a `PropertyChanged` event for the property involved in the *Act*.

```C#
using NUnit.Framework;
using static SmartTests.SmartTest;

[TestFixture]
public class MyClassTest
{
    [Test]
    public void MyPropertyTest_Set()
    {
        var mc = new MyClass();

        RunTest( ValidValue.IsValid,
                 Assign( () => mc.MyProperty, 10 ),
                 SmartAssert.Raised_PropertyChanged() );
    }
}
```

In this example, the Smart Assertion ensures that the `PropertyChanged` event is raised when `mc.MyProperty` is assigned for the `mc` instance, `MyProperty` property.

> Note that if the names specified are not public property names, a `BadTestException` is thrown.

## `Raised_PropertyChanged<T>(T,params string[])`

This overload will ensure that the specified instance and properties raises a `PropertyChanged` event in the *Act*.

```C#
using NUnit.Framework;
using static SmartTests.SmartTest;

[TestFixture]
public class MyClassTest
{
    [Test]
    public void MyMethodTest()
    {
        var mc = new MyClass();

        RunTest( ValidValue.IsValid,
                 () => mc.MyMethod(),
                 SmartAssert.Raised_PropertyChanged(mc, "MyProperty") );
    }
}
```

In this example, the Smart Assertion ensures that the `PropertyChanged` event is raised when calling `mc.MyMethod()` for the `mc` instance, `MyProperty` property.

> Note that if the names specified are not public property names, a `BadTestException` is thrown.

Also:
> Note that if you do not specify property names to check, no property name check is done in the handler.

Finally:
> Note that if a property changed and is not listed here, a `SmartTestException` is thrown. If a property change twice, you have to specify its name twice.

## `Raised_PropertyChanged<T>(T,string,object)`

This overload will ensure that the specified instance and properties raises a `PropertyChanged` event in the *Act*, and the property value is the specified one.

```C#
using NUnit.Framework;
using static SmartTests.SmartTest;

[TestFixture]
public class MyClassTest
{
    [Test]
    public void MyMethodTest()
    {
        var mc = new MyClass();

        RunTest( ValidValue.IsValid,
                 () => mc.MyMethod(),
                 SmartAssert.Raised_PropertyChanged(mc, "MyProperty", 10) );
    }
}
```

In this example, the Smart Assertion ensures that the `PropertyChanged` event is raised when calling `mc.MyMethod()` for the `mc` instance, `MyProperty` property and that the new value of the property is `10`.

> Note that if the name specified is not a public property name, a `BadTestException` is thrown.

Finally:
> Note that if a property changed and is not the specified property, a `SmartTestException` is thrown. If a property change twice, you have to specify its name twice.

## `Raised_PropertyChanged<T>(Expression<Func<T>>)`

This overload will ensure that the specified instance and property raises a `PropertyChanged` event in the *Act*.

```C#
using NUnit.Framework;
using static SmartTests.SmartTest;

[TestFixture]
public class MyClassTest
{
    [Test]
    public void MyMethodTest()
    {
        var mc = new MyClass();

        RunTest( ValidValue.IsValid,
                 () => mc.MyMethod(),
                 SmartAssert.Raised_PropertyChanged( () => mc.MyProperty ) );
    }
}
```

In this example, the Smart Assertion ensures that the `PropertyChanged` event is raised when calling `mc.MyMethod()` for the `mc` instance, `MyProperty` property.

> Note that if the name specified is not a public property name, a `BadTestException` is thrown.

Finally:
> Note that if a property changed and is not the property specified , a `SmartTestException` is thrown. If a property change twice, you have to specify its name twice.

## `Raised_PropertyChanged<T>(Expression<Func<T>>, T)`

This overload will ensure that the specified instance and property raises a `PropertyChanged` event in the *Act* and the property value is the specified one.

```C#
using NUnit.Framework;
using static SmartTests.SmartTest;

[TestFixture]
public class MyClassTest
{
    [Test]
    public void MyMethodTest()
    {
        var mc = new MyClass();

        RunTest( ValidValue.IsValid,
                 () => mc.MyMethod(),
                 SmartAssert.Raised_PropertyChanged( () => mc.MyProperty, 10 ) );
    }
}
```

In this example, the Smart Assertion ensures that the `PropertyChanged` event is raised when calling `mc.MyMethod()` for the `mc` instance, `MyProperty` property and that the new value of the property is `10`.

> Note that if the name specified is not a public property name, a `BadTestException` is thrown.

Finally:
> Note that if a property changed and is not the property sepecified , a `SmartTestException` is thrown. If a property change twice, you have to specify its name twice.

## `NotRaised_PropertyChanged()`

This overload will ensure that the instance and property of the Assign *Act* do not raise a `PropertyChanged` event in the *Act*.

```C#
using NUnit.Framework;
using static SmartTests.SmartTest;

[TestFixture]
public class MyClassTest
{
    [Test]
    public void MyMethodTest()
    {
        var mc = new MyClass();

        RunTest( ValidValue.IsValid,
                 Assign( () => mc.MyProperty, 10 ),
                 SmartAssert.NotRaised_PropertyChanged() );
    }
}
```

In this example, the Smart Assertion ensures that the `PropertyChanged` event is not raised when calling `mc.MyProperty = 10` for the `mc` instance, `MyProperty` property.

## `NotRaised_PropertyChanged<T>(T)`

This overload will ensure that the specified instance do not raise a `PropertyChanged` event in the *Act*.

```C#
using NUnit.Framework;
using static SmartTests.SmartTest;

[TestFixture]
public class MyClassTest
{
    [Test]
    public void MyMethodTest()
    {
        var mc = new MyClass();

        RunTest( ValidValue.IsValid,
                 () => mc.MyMethod(),
                 SmartAssert.NotRaised_PropertyChanged(mc) );
    }
}
```

In this example, the Smart Assertion ensures that the `PropertyChanged` event is not raised when calling `mc.MyMethod()` for the `mc` instance.

## `NotRaised_PropertyChanged<T>(T, params string[])`

This overload will ensure that the specified instance do not raise a `PropertyChanged` event in the *Act* for the specified properties only.

```C#
using NUnit.Framework;
using static SmartTests.SmartTest;

[TestFixture]
public class MyClassTest
{
    [Test]
    public void MyMethodTest()
    {
        var mc = new MyClass();

        RunTest( ValidValue.IsValid,
                 () => mc.MyMethod(),
                 SmartAssert.NotRaised_PropertyChanged(mc, "MyProperty", "OtherProperty") );
    }
}
```

In this example, the Smart Assertion ensures that the `PropertyChanged` event is not raised for `MyProperty` nor `OtherProperty` when calling `mc.MyMethod()` for the `mc` instance.

## `NotRaised_PropertyChanged<T>(Expression<Func<T>>)`

This overload will ensure that the specified instance do not raise a `PropertyChanged` event in the *Act* for the specified property only.

```C#
using NUnit.Framework;
using static SmartTests.SmartTest;

[TestFixture]
public class MyClassTest
{
    [Test]
    public void MyMethodTest()
    {
        var mc = new MyClass();

        RunTest( ValidValue.IsValid,
                 () => mc.MyMethod(),
                 SmartAssert.NotRaised_PropertyChanged( () mc=> mc.MyProperty );
    }
}
```

In this example, the Smart Assertion ensures that the `PropertyChanged` event is not raised for `MyProperty` when calling `mc.MyMethod()` for the `mc` instance.
