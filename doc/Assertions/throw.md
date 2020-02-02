# Throw Smart Assertions <!-- omit in toc -->

*Throw Smart Assertions* are assertions that catch exceptions from within the Act part of your test.

1. Before *Act*
   does nothing.
2. After *Act*
   Ensure it is the excepted exception, otherwise throws an error.

We have three *Throw* assertions, for which the last parameter `Action<T>` is optional.

- [`Throw<T>([Action<T>])`](#throwtactiont)
- [`Throw<T>(string[, Action<T>])`](#throwtstring-actiont)
- [`Throw<T>(string, string[, Action<T>])`](#throwtstring-string-actiont)

## `Throw<T>([Action<T>])`

```C#
using NUnit.Framework;
using static SmartTests.SmartTest;

[TestFixture]
public class MyClassTest
{
    [Test]
    public void DivideTest()
    {
        var mc = new MyClass();

        RunTest( Case("denominator", ValidValue.IsValid) &
                 Case((double divisor) => divisor.Value(0)),
                 () => mc.Divide(1, 0),
                 SmartAssert.Throw<DivideByZeroException>() );
    }
}
```

This example is the simplest usage. The Smart Assertion ensures that the call to `Divide` method generates a `DivideByZeroException`.

You can also verify any property of the exception by providing a method to the `Throw`:

```C#
using NUnit.Framework;
using static SmartTests.SmartTest;

[TestFixture]
public class MyClassTest
{
    [Test]
    public void DivideTest()
    {
        var mc = new MyClass();

        RunTest( Case("denominator", ValidValue.IsValid) &
                 Case((double divisor) => divisor.Value(0)),
                 () => mc.Divide(1, 0),
                 SmartAssert.Throw<DivideByZeroException>(e => e.Message == "...") );
    }
}
```

## `Throw<T>(string[, Action<T>])`

```C#
using NUnit.Framework;
using static SmartTests.SmartTest;

[TestFixture]
public class MyClassTest
{
    [Test]
    public void DivideTest()
    {
        var mc = new MyClass();

        RunTest( Case("denominator", ValidValue.IsValid) &
                 Case((double divisor) => divisor.Value(0)),
                 () => mc.Divide(1, 0),
                 SmartAssert.Throw<ArgumentOutOfRangeException>("divisor") );
    }
}
```

The Smart Assertion ensures that the call to `Divide` method generates a `ArgumentOutOfRangeException` for this particular parameter name: `divisor`.
Any test for `ArgumentException` (and sub-classes) can use this version.

You can provide the optional action to ensure anything else about the exception.

## `Throw<T>(string, string[, Action<T>])`

```C#
using NUnit.Framework;
using static SmartTests.SmartTest;

[TestFixture]
public class MyClassTest
{
    [Test]
    public void DivideTest()
    {
        var mc = new MyClass();

        RunTest( Case("denominator", ValidValue.IsValid) &
                 Case((double divisor) => divisor.Value(0)),
                 () => mc.Divide(1, 0),
                 SmartAssert.Throw<ArgumentOutOfRangeException>("divisor", "divisor cannot be 0") );
    }
}
```

Same as previous one, except that we test for the error message too.

You can provide the optional action to ensure anything else about the exception.
