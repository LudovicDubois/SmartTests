# `ReferenceEquality` Criteria

You use `ReferenceEquality` criteria when you want to test for equality between two values (classes, not structures). Especially when you redefine `==` operator.

It contains five criterions:

* `HasNullLeftOperand`
* `HasNullRightOperand`
* `HasBothNullOperands`
* `HasEqualNotNullOperands`
* `HasSameNotNullOperands`

Use [`equality`](Equality.md) if you want to compare structures instance for equality or [`Comparison`](Comparison.md) for a comparison not for equality.

## Example

```C#
using NUnit.Framework;
using static SmartTests.SmartTest;

[TestFixture]
public class MyClassTest
{
    [Test]
    public void EqualityTest_AreSame()
    {
        var mc = new MyClass();

        var result = RunTest( ReferenceEquality.HasSameNotNullOperands,
                              () => mc == mc );

        Assert.IsTrue( result );
    }
}
```
