# MinExcMaxExc Criteria

You use `MinExcMaxExc` criteria when you want to explicitly specify that a value is in a range, with invalid limits.

Thus, it contains five criterions:

* `IsBelowMin` (an error)
* `IsMin` (an error)
* `IsBetweenMinMax`
* `IsMax` (an error)
* `IsAboveMax` (an error)

Use [MinIncMaxInc](minincmaxinc.md), [MinIncMaxExc](minincmaxexc.md) or [MinExcMaxInc](minexmaxinc.md) to include/exclude min and/or max.

## Example

```C#
using NUnit.Framework;
using static SmartTests.SmartTest;

[TestFixture]
public class MyClassTest
{
    [Test]
    public void MyPropertyTest_IsBelowMin()
    {
        var mc = new MyClass();

        RunTest( MinExcMaxExc.IsBetweenMinMax,
                 Assign( () => mc.MyProperty, 10 ) );

        Assert.AreEqual( 10, mc.MyProperty );
    }
}
```
