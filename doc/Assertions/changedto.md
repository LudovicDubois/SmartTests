# ChangedTo Smart Assertions

*ChangedTo Smart Assertions* are assertions that ensure an object property changed during the *Act* part of your test.
i.e.

1. Before *Act*
  Ensures that the property value is not the one to be assigned in the *Act*.
1. After *Act*
  Ensures that the property value is the one to be assigned in the *Act*.

## `ChangedTo`

We have different `ChangedTo` overloads:

- [ChangedTo Smart Assertions](#changedto-smart-assertions)
  - [`ChangedTo`](#changedto)
  - [`ChangedTo()`](#changedto)
  - [`ChangedTo<T>(Expression<Func<T>>, T)`](#changedtotexpressionfunct-t)

## `ChangedTo()`

The simplest *ChangedTo* Smart Assertion is `ChangedTo()`.

The instance and property involved are the one you want to assign in the *Act*.

```C#
using NUnit.Framework;
using SmartTests.Assertions;
using static SmartTests.SmartTest;

[TestFixture]
public class MyClassTest
{
    [Test]
    public void MyPropertyTest_Set()
    {
        var mc = new MyClass();

        RunTest( AnyValue.Valid,
                 Assign( () => mc.MyProperty, 10),
                 SmartAssert.ChangedTo() );
    }
}
```

In this example, the Smart Assertion verifies that `!Equals(mc.MyProperty,10)` before *Act* (otherwise a `BadTestException` is raised) and that `Equals(mc.MyProperty,10)` after *Act* (otherwise a `SmartTestException` is raised).

## `ChangedTo<T>(Expression<Func<T>>, T)`

The instance and property involved are provided as arguments.

```C#
using NUnit.Framework;
using SmartTests.Assertions;
using static SmartTests.SmartTest;

[TestFixture]
public class MyClassTest
{
    [Test]
    public void MyMethodTest()
    {
        var mc = new MyClass();

        RunTest( AnyValue.Valid,
                 () => mc.MyMethod(),
                 SmartAssert.ChangedTo(() => mc.MyProperty, 10) );
    }
}
```

In this example, the Smart Assertion verifies that `!Equals(mc.MyProperty,10)` before *Act* (otherwise a `BadTestException` is raised) and that `Equals(mc.MyProperty,10)` after *Act* (otherwise a `SmartTestException` is raised).
