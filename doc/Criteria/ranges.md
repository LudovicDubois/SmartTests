# Ranges

A criteria enables you to specify your **Logical Intent** for your *Act* part of a test by specifying all possible integer values using equivalence classes.

More, the value you will use to test your case will be generated automatically, so that you do not test only one value, but any value of the range.

## Example

You use a range like any criteria.

```C#
using NUnit.Framework;
using static SmartTests.SmartTest;

[TestFixture]
public class MathTest
{
    [Test]
    public void Sqrt_ValueGreaterOrEqualTo0()
    {
        var result = RunTest( Int.AboveOrEqual( 0, out var value ),
                              () => Math.Abs( value ) );

        Assert.AreEqual( value, result );
    }

    [Test]
    public void Sqrt_ValueLowerOrEqualTo0()
    {
        var result = RunTest( Int.BelowOrEqual( 0, out var value ),
                              () => Math.Abs( value ) );

        Assert.AreEqual( -value, result );
    }
}
```

## What is a Range

A *Range* is a sequence of integers, of any integer type, to test for a logical intent.

For exanple, `Int.AboveOrEqual( 0, out var value )` is used to represent and test equivalence class `[0, int.MaxValue]`, typically for propertie and parametes values.

The returned `value` should be used in your test as the value for the corresponding property or parameter (using [`Case`](../Cases/readme.md) if you have more than 1 parameter, as usual).

## Usage

All Ranges can be created from a root propery of `SmartTest` class (so implicitly usable when importing `using static SmartTests.SmartTest`).

### Roots

Roots property are the following:

1. `Byte`
2. `SByte`
3. `Short`
4. `UShort`
5. `Int`
6. `UInt`
7. `Long`
8. `ULong`

The created ranges are empty at creation time.

You have to call methods to actually add ranges as you need.

### Ranges methods

Then, you have `Range` methods to specify a range of integers.

For example: `Int.Range( 0, 10 )` represent `[0, 10]` range.

You can not use it directly as a criteria as:

1. It returns the created range.
2. You need to have a value to use as the testing value.

Once you have a filled Range, call its `GetValidValue( out var value )` to have the testing value. This method returns a Criteria.

#### Multiple ranges in an equivalence class

You can create an equivalence class with multiple integer sequences.

For example, for an equivalence class that is all numbers except 0, you can call multiple times the `Range` method by chaining them.

For example: `Int.Range( int.MinValue, -1 ).Range( 1, int.MaxValue )`

### Other Helpers Methods

Ranges have lots of helper methods to make it easier.

1. `Int.AboveOrEqual(min)` is `Int.Range(min, int.MaxValue)`
2. `Int.Above(min)` is `Int.Range(min + 1, int.MaxValue)`
3. `Int.BelowOrEqual(min)` is `Int.Below(int.MinValue, min)`
4. `Int.Below(min)` is `Int.Below(int.MinValue, min - 1)`

#### Overloads calling `GetValidValue`

For simple usage, `Range` method has an overload that calls `GetValidValue`.
Thus, `Int.Range( 0, 10, out var value )` is exactly the same as `Int.Range( 0, 10 ).GetValidValue( out var value )`.

In the same way, all heper methods have the same overload:

1. `Int.AboveOrEqual(min, out var value)`
2. `Int.Above(min, out var value)`
3. `Int.BelowOrEqual(min, out var value)`
4. `Int.Below(min, out var value)`

They have to be the last ones when you use multiple ranges as they return a Criteria.

> Tip: it's simpler to use these overloads instead of calling `GetValidValue`.

### GetErrorValue

`GetValidValue(out var value)` enables you to:

1. Use a *Range* where a Criteria is expected
2. Have a valid value, from the *Range*, to use in your test.

But, we have to be able to get invalid values for ranges too (as they are not combined with other criteria).

It is the goal of `GetErrorValue(out var value)`

Thus, to have an error value for:

1. A strictly negative value of integers

   `Int.Below(0).GetErrorValue(out var value)`

2. Range `[-10, 10]`

    `Int.Range(-10, -10).GetErrorValue(out var value)`

3. Range `[-short.MinValue,-10] U [10, short.MaxValue]`

    `Short.Below(-9).Above(9).GetErrorValue(out var value)` or
    `Short.BelowOrEqual(-10).AboveOrEqual(10).GetErrorValue(out var value)`
