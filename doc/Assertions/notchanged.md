# NotChanged Smart Assertions

*NotChanged Smart Assertions* are assertions that ensure an object did not change during the *Act* part of your test.

1. Before *Act*
  Creates a backup of properties and/or fields.
1. After *Act*
  Ensures that the properties and/or fields have the previous values.

## `NotChanged`

We have different `NotChanged` overloads:

* [`NotChanged()`](#NotChanged)
* [`NotChanged(NotChangedKind)`](#NotChangedNotChangedKind)
* [`NotChanged(object)`](#NotChangedObject)
* [`NotChanged(object, NotChangedKind)`](#NotChangedObject-NotChangedKind)

### NotChanged()

The simplest *NotChanged* Smart Assertion is `NotChanged()`.

It will ensure that the instance involved in the *Act* part is not changed; i.e. none of its public properties have changed.

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

        RunTest( AnyValue.Valid,
                 () => mc.MyMethod(),
                 SmartAssert.NotChanged() );
    }
}
```

In this example, the Smart Assertion ensures that no public property of `mc` has changed during the call to `mc.MyMethod()`.

> Note that it is not really true: if a property changes and comes back to its previous value during `mc.MyMethod()`, the assertion pass.

### `NotChanged(NotChangedKind)`

An overload of `NotChanged` take a parameter to specify what to backup and compare, using the enum `NotChangedKing`:

* `PublicProperties`
* `NonPublicProperties`
* `AllProperties`
* `PublicFields`
* `NonPublicFields`
* `AllFields`
* `All`

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

        RunTest( AnyValue.Valid,
                 () => mc.MyMethod(),
                 SmartAssert.NotChanged( NotChangedKind.All) );
    }
}
```

In this example, the Smart Assertions ensures that no property (public or not) and no field (public or not) of `mc` has changed during the call to `mc.MyMethod()`.

> Default parameter value is `NotChangedKind.PublicProperties`.

### `NotChanged(object)`

You can also specify that public properties of any object have not changed.

Is is especially useful if you have arguments that are instances that should not changed.

```C#
using NUnit.Framework;
using static SmartTests.SmartTest;

[TestFixture]
public class MyClassTest
{
    [Test]
    public void CopyFromTest()
    {
        var mc = new MyClass();
        var other = new MyClass();

        RunTest( ValidValue.IsValid,
                 () => mc.CopyFrom( other ),
                 SmartAssert.NotChanged(other) );
    }
}
```

In this example, the Smart Assertion ensures that no public property of `other` has changed during the call to `mc.CopyFrom(other)`.

### `NotChanged(object, NotChangedKind)`

You can do the same but for any public/non public properties/fields.

```C#
using NUnit.Framework;
using static SmartTests.SmartTest;

[TestFixture]
public class MyClassTest
{
    [Test]
    public void CopyFromTest()
    {
        var mc = new MyClass();
        var other = new MyClass();

        RunTest( ValidValue.IsValid,
                 () => mc.CopyFrom( other ),
                 SmartAssert.NotChanged( other, NotChangedKind.All ) );
    }
}
```

In this example, the Smart Assertion ensures that no property (public or not) and no field (public or not) of `other` has changed during the call to `mc.CopyFrom(other)`.

## `NotChangedExcept`

The `NotChangedExcept` Smart Assertion enables you to specify property/field names not to check.

Thus, if you test a property assignment, for example, the property itself should change, but not the other properties.

We have different `NotChangedExcept` overloads:

- [`NotChangedExceptAct()`](#NotChangedExceptAct)
- [`NotChangedExcept(params string[])`](#NotChangedExceptparams-string)
- [`NotChangedExcept(NotChangedKind, params string[])`](#NotChangedExceptNotChangedKind-params-string)
- [`NotChangedExcept(object, params string[])`](#NotChangedExceptobject-params-string)
- [`NotChangedExcept(object, NotChangedKind, params string[])`](#NotChangedExceptobject-NotChangedKind-params-string)
- [`NotChangedExcept(Expression<Func<T>>)`](#NotChangedExceptExpressionFuncT)
- [`NotChangedExcept(Expression<Func<T>>, NotChangedKind)`](#NotChangedExceptExpressionFuncT-NotChangedKind)

### `NotChangedExceptAct()`

Before v1.11, was `NotChangedExcept`.

This method enables you to deduce the public property name and instance of which property you do not want to be checked for changes.

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
                 SmartAssert.NotChangedExceptAct() );
    }
}
```

In this example, the Smart Assertion ensures that no public property of `mc` has changed during the call to `mc.MyProperty = 10` except `MyProperty`, that is not checked.

### `NotChangedExcept(params string[])`

This method enables you to specify the public property names you do not want to be checked for changes.

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
                 () => mc.MyMethod(),
                 SmartAssert.NotChangedExcept( "MyProperty", "OtherProperty" ) );
    }
}
```

In this example, the Smart Assertion ensures that no public property of `mc` has changed during the call to `mc.MyMethod()` except `MyProperty` or `OtherProperty`, that are not checked.

> Note that if the names specified are not public property names, a `BadTestException` occurs.

### `NotChangedExcept(NotChangedKind, params string[])`

This overload enables you to specify what to check, as seen before. Thus, you can avoid properties and/or fields checking.

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
                 () => mc.MyMethod(),
                 SmartAssert.NotChangedExcept( NotChangedKind.All, "MyProperty", "OtherProperty" ) );
    }
}
```

In this example, the Smart Assertion ensures that no property (public or not) nor field (public or not) of `mc` has changed during the call to `mc.MyMethod()` except `MyProperty` and `OtherProperty`, that are not checked.

> Note that if the names specified are not property nor field names, a `BadTestException` occurs.

### `NotChangedExcept(object, params string[])`

This overload enables you to specify for which instance you want the public properties to be checked or not.

```C#
using NUnit.Framework;
using static SmartTests.SmartTest;

[TestFixture]
public class MyClassTest
{
    [Test]
    public void CopyPropertiesFromTest()
    {
        var mc = new MyClass();
        var other = new MyClass();

        RunTest( ValidValue.IsValid,
                 () => mc.CopyPropertiesFrom(other),
                 SmartAssert.NotChangedExcept( other, "CopyCount" ) );
    }
}
```

In this example, the Smart Assertion ensures that no public property of `other` has changed during the call to `mc.CopyPropertiesFrom(other)` except `CopyCount`, that is not checked.

> Note that if the names specified are not public property names, a `BadTestException` occurs.

### `NotChangedExcept(object, NotChangedKind, params string[])`

This overload enables you to specify for which instance you want what to be checked or not.

```C#
using NUnit.Framework;
using static SmartTests.SmartTest;

[TestFixture]
public class MyClassTest
{
    [Test]
    public void CopyPropertiesFromTest()
    {
        var mc = new MyClass();
        var other = new MyClass();

        RunTest( ValidValue.IsValid,
                 () => mc.CopyPropertiesFrom(other),
                 SmartAssert.NotChangedExcept( other, NotChangedKind.All, "CopyCount" ) );
    }
}
```

In this example, the Smart Assertion ensures that no property (public or not) nor field (public or not) of `other` has changed during the call to `mc.CopyPropertiesFrom(other)` except `CopyCount`, that is not checked.

> Note that if the names specified are not property not field names, a `BadTestException` occurs.

### `NotChangedExcept(Expression<Func<T>>)`

This overload enables you to specify for which instance you want the public properties to be checked except for the specified property.

```C#
using NUnit.Framework;
using static SmartTests.SmartTest;

[TestFixture]
public class MyClassTest
{
    [Test]
    public void CopyPropertiesFromTest()
    {
        var mc = new MyClass();
        var other = new MyClass();

        RunTest( ValidValue.IsValid,
                 () => mc.CopyPropertiesFrom(other),
                 SmartAssert.NotChangedExcept( () => other.CopyCount ) );
    }
}
```

In this example, the Smart Assertion ensures that no public property of `other` has changed during the call to `mc.CopyPropertiesFrom(other)` except `CopyCount`, that is not checked.

### `NotChangedExcept(Expression<Func<T>>, NotChangedKind)`

This overload enables you to specify for which instance you want the specified kind of properties to be checked except for the specified property.

```C#
using NUnit.Framework;
using static SmartTests.SmartTest;

[TestFixture]
public class MyClassTest
{
    [Test]
    public void CopyPropertiesFromTest()
    {
        var mc = new MyClass();
        var other = new MyClass();

        RunTest( ValidValue.IsValid,
                 () => mc.CopyPropertiesFrom(other),
                 SmartAssert.NotChangedExcept( () => other.CopyCount, NotChangedKind.All ) );
    }
}
```

In this example, the Smart Assertion ensures that no property (public or not) of `other` has changed during the call to `mc.CopyPropertiesFrom(other)` except `CopyCount`, that is not checked.
