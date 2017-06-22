# Combining Criteria

Sometimes, you have several intents that are orthogonal for your test.

For example, your property as a minimum value (see [MinIncluded](minincluded.md)) and should raised an event when changed (see [NotifyPropertyChanged](notifypropertychanged.md)).

Thus, we need to combine Criteria.

## & operator

`&` operator enables you to combine two orthogonal Criteria for one [Case](/doc/Cases/readme.md).

Thus, for our previous example, we combine [MinIncluded](minincluded.md) with [NotifyPropertyChanged](notifypropertychanged.md):

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

### How many tests to write?

As usual, the *Smart Tests* Analyzer will notify you of missing tests.

In case you combine criteria, how many tests do we need?

* `MinIncuded` has 3 criterions (1 is an error)
* `NotifyPropertyChanged` has 3 criterions (no error)

Do we have to write 9 tests?

No.

The rule is:
> When you combine criteria, you should have one test per error criterion (independently), and one test for all combinations of valid criterions.

Thus, for our example, we should have 7 tests, not 9:

1. `MinIncluded.IsBelowMin`
1. `MinIncluded.IsMin & NotifyPropertyChanged.HasNoSubscriber`
1. `MinIncluded.IsMin & NotifyPropertyChanged.HasSubscriberSameValue`
1. `MinIncluded.IsMin & NotifyPropertyChanged.HasSubscriberOtherValue`
1. `MinIncluded.IsAboveMin & NotifyPropertyChanged.HasNoSubscriber`
1. `MinIncluded.IsAboveMin & NotifyPropertyChanged.HasSubscriberSameValue`
1. `MinIncluded.IsAboveMin & NotifyPropertyChanged.HasSubscriberOtherValue`

If our property has a minimum value that is an error instead, we would have 4 tests only:

1. `MinExcluded.IsBelowMin`
1. `MinExcluded.IsMin`
1. `MinExcluded.IsAboveMin & NotifyPropertyChanged.HasNoSubscriber`
1. `MinExcluded.IsAboveMin & NotifyPropertyChanged.HasSubscriberSameValue`
1. `MinExcluded.IsAboveMin & NotifyPropertyChanged.HasSubscriberOtherValue`

## | operator

In our previous example with `MinIncluded` and `NotifyPropertyChanged`, we have 9 cases.

For any reason, you can consider that testing `NotifyPropertyChanged` should be independent of `MinIncluded`.
Thus, we do not want to tests the 3 cases twice (one for `MinIncluded.IsMin` and one for `MinIncluded.IsAboveMin`).

In this case, you can combine criterion of the same criteria with the `|` operator, thus having 5 cases only:

1. `MinIncluded.IsBelowMin`
1. `MinIncluded.IsMin & (NotifyPropertyChanged.HasNoSubscriber | NotifyPropertyChanged.HasSubscriberSameValue | NotifyPropertyChanged.HasSubscriberOtherValue)`
1. `MinIncluded.IsAboveMin & NotifyPropertyChanged.HasNoSubscriber`
1. `MinIncluded.IsAboveMin & NotifyPropertyChanged.HasSubscriberSameValue`
1. `MinIncluded.IsAboveMin & NotifyPropertyChanged.HasSubscriberOtherValue`

It is not the best way to have a good logical coverage, but it can be sufficient for a good code coverage.

As you can see, you can also use parenthesis in you criteria expression.
