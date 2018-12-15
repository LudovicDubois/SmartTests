# Criteria

A criteria enables you to specify your **Logical Intent** for your *Act* part of a test.

## Usage

You use a criteria in a [Case](/doc/Cases/readme.md) as the first argument of [RunTest](/doc/WriteSmartTest.md) method call.

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

Fortunately, if you only need one `Case`, you can omit it by calling another `RunTest` overload.

```C#
using NUnit.Framework;
using static SmartTests.SmartTest;

[TestFixture]
public class MathTest
{
    [Test]
    public void Sqrt_ValueGreaterThanMin()
    {
        var result = RunTest( MinIncluded.IsAboveMin,
                              () => Math.Sqrt(2) );

        Assert.AreEqual( 2, result );
    }
}
```

## What is a Criteria

A *Criteria* has several criterion, each for a specific case of a logical intent.

For example, the `MinIncluded` criteria is used when you have a minimum value (for your property, parameter...).

* All values below this minimum value is an error.
* Minimum value is correct.
* All values above this minimum value is correct.

Technically, a criteria is a subclass of `Criteria` class, that defines public static read-only fields only of its own type, one for each Logical Criterion.

Thus, for `MinIncluded`, we have this class:

```C#
public class MinIncluded : Criteria
{
    [Error]
    public static readonly Criteria IsBelowMin = new MinIncluded();
    public static readonly Criteria IsMin = new MinIncluded();
    public static readonly Criteria IsAboveMin = new MinIncluded();
}
```

As you can see in the previous example, the `[Error]` attribute specifies which criterion are errors.

## Smart Test Analyzer

When you create a test with a criteria, the *Smart Test Analyzer* generates a warning message specifying that your test need two other tests: `MinIncluded.IsBelowMin` and `MinIncluded.IsMin`.

This way, your test is clearer and you do not forget to write tests.

This enables you to have a **Logical Coverage**, independently of the source code of your tested member.

## Existing Criteria

Currently, predefined Criteria are the following:

### Value with 1 limit

* [`MaxExcluded`](MaxExcluded.md)
* [`MaxIncluded`](MaxIncluded.md)
* [`MinExcluded`](MinExcluded,md)
* [`MinIncluded`](MinIncluded.md)

### Value with 2 limits

* [`MinIncMaxInc`](MinIncMaxInc.md)
* [`MinIncMaxExc`](MinIncMaxExc.md)
* [`MinExcMaxInc`](MinExcMaxInc.md)
* [`MinExcMaxExc`](MinExcMaxExc.md)

### Comparisons

* [`Comparison`](Comparison.md)
* [`Equality`](Equality.md)
* [`ReferenceEquality`](ReferenceEquality.md)

### Collections

* [`Collection`](Collection.md)
* [`CollectionItem`](CollectionItem.md)

### Valid Values

* [`AnyValue`](AnyValue.md)
* [`FormattedString`](FormattedString.md)
* [`Path`](Path.md)
* [`ValidString`](ValidString.md)
* [`ValidValue`](ValidValue.md)

### Others

* [`NotifyPropertyChanged`](NotifyPropertyChanged.md)

### Ranges

Since v1.4, we can define criteria as ranges of integer values.
It enables you to show logical equivalence groups right in your tests.

For more information, see [`Ranges`](ranges.md).

Since v1.5, we can define criteria as ranges of floating point numbers.

For more information, see [`Ranges`](ranges.md).

### Enums

Since v1.6, we can define criteria as enum values.
For more information, see [`Enums`](enums.md)

### Next

We will add other Criteria.

## Combining Criteria

Sometimes, you have several intents that are orthogonal for your test.

For example, your property as a minimum value (see [`MinIncluded`](MinIncluded.md)) and should raised an event when changed (see [`NotifyPropertyChanged`](NotifyPropertyChanged.md)).

You only have to [combine Criteria](combine.md) to achieve your goal.

## Adding Criteria

If you cannot express your intent with this predefined criteria, you can [create your own](adding.md).