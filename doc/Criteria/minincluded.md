# MinIncluded Criteria

You use `MinExcluded` criteria when you want to explicitly specify that a value has a minimum, that is valid.

Thus, it contains three criterions:

* `IsBelowMin` (an error)
* `IsMin`
* `IsAboveMin`

Use [MinExcluded](minexcluded.md) if you want the minimum be excluded.

Use [MaxIncluded](maxincluded.md) or [MaxExcluded](maxexcluded.md) if you want a maximum instead of a minimum.

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

        RunTest( MinIncluded.IsAboveMin,
                 Assign( () => mc.MyProperty, 10 ) );

        Assert.AreEqual( 10, mc.MyProperty );
    }
}
```
