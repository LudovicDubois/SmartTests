# MaxIncluded Criteria

You use `MaxIncluded` criteria when you want to explicitly specify that a value has a maximum, that is valid.

Thus, it contains three criterions:

* `IsBelowMax`
* `IsMax`
* `IsAboveMax` (an error)

Use [MaxExcluded](maxexcluded.md) if you want the maximum be excluded.

Use [MinIncluded](minincluded.md) or [MinExcluded](minexcluded.md) if you want a minimum instead of a maximum.

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

        RunTest( MaxIncluded.IsBelowMax,
                 Assign( () => mc.MyProperty, -10 ) );

        Assert.AreEqual( -10, mc.MyProperty );
    }
}
```
