# `CollectionItem` Criteria

You use `CollectionItem` criteria when you want to test collection management, such as insertion or deletion of items.

It contains three criterions:

* `IsNull` (an error)
* `IsNotInCollection`
* `IsInCollection`

Combine it with [`Collection`](Collection.md) for a more complete test.

## Example

```C#
using NUnit.Framework;
using static SmartTests.SmartTest;

[TestFixture]
public class MyClassTest
{
    [Test]
    public void AddTest_IsEmpty()
    {
        var mc = new MyClass();

        RunTest( Collection.IsEmpty &
                 CollectionItem.IsNotInCollection,
                 () => mc.Add( 10 ) );

        Assert.AreEqual( 1, mc.Count );
        Assert.AreEqual( 10, mc[0] );
    }
}
```
