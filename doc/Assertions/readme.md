# Smart Assertions

*Smart Test* enables you to do **Smart Assertions** too.

*Smart Assertions* are assertions that do high level works with one line of code only!

For example, when you have a test for a property assignment, you can verify that `PropertyChanged` event is raised with 1 line of code only!

## Usage

To use a *Smart Assertion*, you have to pass `Assertion` instances as `params` parameters of any `RunTest` method.

To create an Assertion instance, you should use the `public static readonly` property `SmartAssert` from the class `SmartTest`.

```C#
using NUnit.Framework;
using static SmartTests.SmartTest;

[TestFixture]
public class MyClassTest
{
    [Test]
    public void MyPropertyTest_IsAboveMin()
    {
        var mc = new MyClass();

        RunTest( MinExcluded.IsAboveMin &
                 NotifyPropertyChanged.HasNoSubscriber,
                 Assign( () => mc.MyProperty, 10 ),
                 SmartAssert.Raised_PropertyChanged() );

        Assert.AreEqual( 10, mc.MyProperty );
    }
}
```

## What is a Smart Assertion

A *Smart Assertion* is an assertion that is able to run code *before* and *after* the *Act* part of your test.

For example, in the previous example, `Raised_PropertyChanged` creates an instance of an `Assertion` sub-class that:

1. Before *Act*: register itself on the `PropertyChanged` event of mc.
1. During *Act*: the event should be triggered and the previous method should be called:
   1. Setting a flag that the event was raised
   1. Verifying that the `PropertyName` of the event is `MyProperty`.
1. After *Act*: ensure that the flag is `true`, otherwise generates an exception.

## Existing Smart Assertions

Here is the existing categories of *Smart Assertions*:

* [`Change`](change.md): to test relative changes to properties
* [`ChangedTo`](changedto.md): to test an effective change to a property.
* [`NotChanged`](notchanged.md)L to test properties did not change.
* [`PropertyChanged`](propertychanged.md): to test `PropertyChanged` event.
* [`Raise`](raise.md): to test any classical event.
* [`Wait`](wait.md): to test parallel code.
* [`Within`](within.md): to test code is done within specific time.

## Adding Smart Assertions

If you want more *Smart Assertions*, you can [create your own](adding.md).