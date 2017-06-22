# Cases

Cases enables you to specify [Criteria](/doc/Criteria/readme.md) for each parameter, if any.

## Without Parameter Name

The simplest form of `Case` has only a [Criteria](/doc/Criteria/readme.md) expression.

It is a `public static` method of main class `SmartTest`:

```C#
using NUnit.Framework;
using static SmartTests.SmartTest;

[TestFixture]
public class MathTest
{
    [Test]
    public void Sqrt_ValueGreaterThanMin()
    {
        var result = RunTest( Case( MinIncluded.IsAboveMin ),
                              () => Math.Sqrt(2) );

        Assert.AreEqual( 2, result );
    }
}
```

This form is for properties or methods having one parameter only (thus, not ambiguity to what the criteria applies to).

> Note that an overload of `RunTest` enables you to specify the Criteria expression directly when you do not need the parameter name. Thus, this form is not really explicitly used.

## With Parameter Name

The most commonly used `Case` has two parameters:

1. Parameter Name
1. Criteria expression.

It is a `public static` method of main class `SmartTest`:

```C#
using NUnit.Framework;
using static SmartTests.SmartTest;

[TestFixture]
public class MathTest
{
    [Test]
    public void Sqrt_ValueGreaterThanMin()
    {
        var result = RunTest( Case( "d", MinIncluded.IsAboveMin ),
                              () => Math.Sqrt(2) );

        Assert.AreEqual( 2, result );
    }
}
```

This overload is for constructors and methods having parameters and indexers.

> Note that a compile-time error will occur if the parameter name specified is not a real parameter name of your tested member.

Also,
> Note that if your method has one parameter only, the Criteria expression can only be for this parameter; thus, the parameter name is optional: you can use the above overload or no `Case` at all (directly the Criteria).

## Combining Cases

If your member has several parameters you have to specify which parameter has which Criteria using `Case`s.

Combine all these Cases with `&` operator:

```C#
using NUnit.Framework;
using static SmartTests.SmartTest;

[TestFixture]
public class MathTest
{
    [Test]
    public void Max_ValuesGreaterThanMin()
    {
        var remainder;
        var result = RunTest( Case( "a", AnyValue.IsValid ) &
                              Case( "b", ValidValue.IsValid ),
                              () => Math.DivRem(5, 2, out remainder) );

        Assert.AreEqual( 2, result );
        Assert.AreEqual( 1, remainder)
    }
}
```

> Note that you need one Case for each parameter, except for out parameters (as we do not provide values).

If you need some other criteria expression for the whole tested member, you can combine your actual cases with a case without parameter.
