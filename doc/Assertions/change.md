# Change Smart Assertions

*Change Smart Assertions* are assertions that ensure relative changes to properties/indexers of an object.

For example, instead of asserting that the Count of a list is 3, you can assert that the count is incremented by 1 or 2... what ever that reflect your Logical Intent.

1. Before *Act*
  Evaluates an expression involving either one property or one indexer.
1. After *Act*
  Retrieves the value of the involved property or indexer (without the expression) and compare it with the previous value. Is they are not equal, a `SmartTestException` is thrown.

## Change

We only have one method.

### [`Change<T>(Expression<Func<T>>)`]

The expression must involve one property or one indexer only.

```C#
using NUnit.Framework;
using static SmartTests.SmartTest;

[TestFixture]
public class MyClassTest
{
    [Test]
    public void AddItemTest()
    {
        var mc = new MyClass();

        RunTest( AnyValue.Valid,
                 () => mc.AddItem(),
                 SmartAssert.Change( () => mc.MyList.Count + 1 ) );
    }
}
```

In this example, the Smart Assertion ensures that `mc.MyList.Count + 1` before *Act* equals `mc.MyList.Count` after *Act*.
