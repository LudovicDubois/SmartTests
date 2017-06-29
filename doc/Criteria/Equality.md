# `Equality` Criteria

You use `Equality` criteria when you want to verify equality between two values (structures, not classes).

It contains three criterions:

* `AreDifferent`
* `AreEqual`
* `AreSame`

Use [`ReferenceEquality`](ReferenceEquality.md) if you want to compare classes instance for equality or [`Comparison`](Comparison.md) for a comparison not for equality.

## Example

```C#
using NUnit.Framework;
using static SmartTests.SmartTest;

[TestFixture]
public class MyClassTest
{
    [Test]
    public void EqualsTest_AreDifferent()
    {
        var mc = new MyClass();
        var mc2 = new MyClass( 10 );

        vr result = RunTest( Equality.AreDifferent,
                             () => mc.Equals( mc2 ) );

        Assert.IsFalse( result );
    }
}
```
