# `Comparison` Criteria

You use `Comparison` criteria when you want to compare a value with another value.

It contains three criterions:

* `IsBelow`
* `IsEqual`
* `IsAbove`

Use [`Equality`](Equality.md) or [`ReferenceEquality`](ReferenceEquality.md) if you want to compare for equality (of a structure or a class respectively).

## Example

```C#
using NUnit.Framework;
using static SmartTests.SmartTest;

[TestFixture]
public class MyClassTest
{
    [Test]
    public void CompareToTest_IsBelow()
    {
        var mc = new MyClass();

        var result = RunTest( Comparison.IsBelow,
                 () => mc.CompareTo(0) );

        Assert.IsTrue( result < 0 );
    }
}
```
