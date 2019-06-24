# EnumType

A criteria enables you to specify your **Logical Intent** for your *Act* part of a test by specifying all possible enum values using equivalence classes.

More, the value you will use to test your case will be generated automatically, so that you do not test only one value, but any value of the specified enum values.

## Example

You use an EnumType like any criteria.

```C#
using NUnit.Framework;
using static SmartTests.SmartTest;

[TestFixture]
public class MathTest
{
    [Test]
    public void StringCompare_SameValue()
    {
        var data = "Smart Tests";

        var result = RunTest( Enum.Values( out var value,
                                           System.StringComparison.CurrentCulture,
                                           System.StringComparison.InvariantCulture,
                                           System.StringComparison.Ordinal),
                              () => string.Compare( data, data, value ) );

        Assert.AreEqual( 0, result );
    }


    [Test]
    public void StringCompare_DifferentCase()
    {
        var data = "Smart Tests";

        var result = RunTest( Enum.Values( out var value,
                                           System.StringComparison.CurrentCulture,
                                           System.StringComparison.InvariantCulture,
                                           System.StringComparison.Ordinal),
                              () => string.Compare( data, data.ToUpper(), value ) );

        Assert.AreEqual( 1, result );
    }
}
```

## What is an EnumType

An *EnumType* is a group of enum values of the same enum, to test for a logical intent.

For example,  `Enum.Values(out var value, System.String.StringComparison.CurrentCulture, System.String.StringComparison.InvariantCulture, System.String.StringComparison.Ordinal)` is used to represent equivalent enum values for your test, typically for property or parameter values.

The returned `value` should be used in your test as the value for the corresponding property or parameter (using [`Case`](../Cases/readme.md) if you have more than 1 parameter, as usual).

## Usage

All Enums can be created from a root property of `SmartTest` class (so implicitly usable when importing `using static SmartTests.SmartTest`): `Enum`.

## Enum methods

### Single enum value

You have `Value<T>(T value)` method to specify a single enum value.

For example: `Enum.Value( System.String.StringComparison.CurrentCulture )`.

You can use it directly as a criteria.

#### Multiple enum values in an equivalence class

You can create an equivalence class with multiple enum values of the same type with `Values<T>(out T value, T firstValue, params T[] values)`

For example: `Enum.Values(out var value, System.String.StringComparison.CurrentCulture, System.String.StringComparison.InvariantCulture, System.String.StringComparison.Ordinal)`

`Enum.Values` will choose any of the specified enum values and return it through  `value`, to use it in your test.

### Errors

`ErrorValue<T>(T value)` and `ErrorValues<T>(out T value, T firstValue, params T[] values)` are the exact same methods as `Value` and `Values`, but for errors.
