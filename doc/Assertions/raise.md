# Raise Smart Assertions <!-- omit in toc -->

*Raise Smart Assertions* are assertions that ensure an event (of type `EventHandler<T>`) is raised or not, depending on your Test Logical Intent.

1. Before *Act*
  Registers itself on the event.
1. During *Act*
  Sets a flag that the event was raised
1. After *Act*
  Ensures that the flag is `true` or `false`, as expected, otherwise generates a `SmartTestException`.

> Note that if the instance does not have the specified event, a `BadTestException` is thrown.

We have different Raise Smart Assertions:

- [`Raised(string)`](#Raisedstring)
- [`Raised<T>(string, EventHandler<T>)`](#RaisedTstring-EventHandlerT)
- [`Raised(object, string)`](#Raisedobject-string)
- [`Raised<T>(object, string, EventHandler<T>)`](#RaisedTobject-string-EventHandlerT)
- [`NotRaised(string)`](#NotRaisedstring)
- [`NotRaised(object, string)`](#NotRaisedobject-string)

## `Raised(string)`

The simplest *Raise* Smart Assertion is `Raised(string)`.

It ensures that the instance involved in the *Act* raises the specified event.

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
                 SmartAssert.Raised("MyEvent") );
    }
}
```

In this example, the Smart Assertion ensures that the `MyEvent` event of the `mc` instance is raised when `mc.MyMethod()` is called.

> Note that if the name specified is not a public event name, a `BadTestException` is thrown.

## `Raised<T>(string, EventHandler<T>)`

This overload ensures that the instance involved in the *Act* raises the specified event and any other assertion when the event is raised.

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
                 SmartAssert.Raised("MyEvent",
                                    (sender, args) =>
                                    {
                                      // Check any thing you want when MyEvent is raised.
                                    }) );
    }
}
```

In this example, the Smart Assertion ensures that the `MyEvent` event of the `mc` instance is raised when `mc.MyMethod()` is called, and that any assertion you want pass when `MyEvent` is raised.

> Note that if the name specified is not a public event name, a `BadTestException` is thrown.

## `Raised(object, string)`

This overload ensures that the specified instance raises the specified event.

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
                 SmartAssert.Raised(mc.InnerObject, "MyEvent") );
    }
}
```

In this example, the Smart Assertion ensures that the `MyEvent` event of the `mc.InnerObject` instance is raised when `mc.MyMethod()` is called.

> Note that if the names specified is not public event name, a `BadTestException` is thrown.

## `Raised<T>(object, string, EventHandler<T>)`

This overload ensures that the specified instance raises the specified event and any other assertion when the event is raised.

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
                 SmartAssert.Raised(mc.InnerObject,
                                    "MyEvent",
                                    (sender, args) =>
                                    {
                                      // Check any thing you want when MyEvent is raised.
                                    }) );
    }
}
```

In this example, the Smart Assertion ensures that the `MyEvent` event of the `mc.InnerObject` instance is raised when `mc.MyMethod()` is called, and that any assertion you want pass when `MyEvent` is raised.

> Note that if the name specified is not a public event name, a `BadTestException` is thrown.

## `NotRaised(string)`

This method ensures that the instance involved int the *Act* do not raise the specified event in the *Act*.

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
                 SmartAssert.NotRaised("MyEvent") );
    }
}
```

In this example, the Smart Assertion ensures that the `MyEvent` event is not raised when calling `mc.MyMethod()` for the `mc` instance.

> Note that if the name specified is not a public event name, a `BadTestException` is thrown.

## `NotRaised(object, string)`

This overload ensures that the specified instance do not raise the specified event in the *Act*.

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
                 SmartAssert.NotRaised(mc.InnerObject, "MyEvent") );
    }
}
```

In this example, the Smart Assertion ensures that the `MyEvent` event is not raised when calling `mc.MyMethod()` for the `mc.InnerObject` instance.

> Note that if the name specified is not a public event name, a `BadTestException` is thrown.