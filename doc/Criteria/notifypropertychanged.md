# NotifyPropertyChanged Criteria

You use `NotifyPropertyChanged` criteria when you want to test the implementation of `INotifyPropertyChanged` in your class.

It is typically used to test a property or indexer assignment.

The different scenario to test are:

* If there is no subscriber, it should not generate any exception.  
   In case you didn't ensure that `PropertyChanged` event is not `null` before raising it.
* If there is a subscriber, we have two cases then:
  * We set the same value as the actual one, the `PropertyChanged` event should not be raised.
  * We set a different value as the actual one, the `PropertyChanged` event has to be raised.

Thus, the `NotifyPropertyChanged` criteria contains three criterions:

* `HasNoSubscriber`
* `HasSubscriberSameValue`
* `HasSubscriberOtherValue`

## Example

As this Criteria is for a value of a property or an indexer, you should combine it with another criteria for the range of the value (not mandatory for `AnyValue.IsValid`).

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

        RunTest( MinExcluded.IsAboveMin &
                 NotifyPropertyChanged.HasNoSubscriber,
                 Assign( () => mc.MyProperty, 10 ) );

        Assert.AreEqual( 10, mc.MyProperty );
    }
}
```

Learn more about [Combining Criteria](combining.md).

## Asserting a PropertyChanged event

You can automatically verify that the `PropertyChanged` event is raised with one line of code only!

See [PropertyChanged assertions](/doc/Assertions/propertychanged.md).
