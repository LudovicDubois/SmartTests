# Writing a Smart Test

First of all, your *Smart Test* is a Test written with your favorite Testing Framework (if supported by SmartTests Framework. You can easily extend it to [support any Testing Framework](TestingFrameworks/readme.md) if needed).

Here is a C# example of a test using NUnit:

```C#
using NUnit.Framework;

[TestFixture]
public class MyTest
{
    [Test]
    public void MyFirstTest()
    {
        // Arrange

        // Act

        // Assert
    }
}
```

As you surely know, a test should be written with AAA good practice in mind:

1. **A**rrange
1. **A**ct
1. **A**ssert

The most important part is *Act* one: it calls what you want to test.

The *Arrange* part has to prepare everything needed to actually run your *Act* part.

The *Assert* part has to verify any assertion you need to verify after the *Act* part has done whatever is needed.

## `RunTest`

All you need is inside the `SmartTest` class, as static methods. Thus, it is a good idea to use it with a static using.

```C#
using NUnit.Framework;
using static SmartTests.SmartTest;
```

The first static method we will see is `RunTest`: it enables you to specify your *Act* part using a lambda expression.

Thus, the *Arrange* part is everything that is before the `RunTest` call, while the *Assert* part is everything that is after the `RunTest` call.
Thus, no need for comments to identify the three parts.

Here is a C# example of a Smart Test using NUnit:

```C#
using NUnit.Framework;
using static SmartTests.SmartTest;

[TestFixture]
public class MathTest
{
    [Test]
    public void Sqrt_ValueGreaterThanMin()
    {
        var result = RunTest( ..., () => Math.Sqrt(2) );

        Assert.AreEqual( 2, result );
    }
}
```

In this example, I did not specify the first argument yet; we will see it in the next part.

## Criteria

As you know, `Math.Sqrt` has a min value: `0` which is valid.
Any value greater than `0` is a valid argument, while any value lower than `0` is an invalid argument.

Thus, you should have a test for a value greater than `0` and a test for a value lower than `0`. But what about the limit value itself? Is it `0` or `1` or `-1` or any other value?
And this minimum value is correct or an error?

For better logical coverage, you have to have a test for the minimum value that answers these questions.

A criteria enables you to express exactly that.

For example, the criteria `MinIncluded` has three values: `IsBelowMin`, `IsMin` and `IsAboveMin`.

You use it to express your logical intent of the test as the first argument of `RunTest` call.

Here is a C# example of a complete Smart Test using NUnit:

```C#
using NUnit.Framework;
using static SmartTests.SmartTest;

[TestFixture]
public class MathTest
{
    [Test]
    public void Sqrt_ValueGreaterThanMin()
    {
        var result = RunTest( MinIncluded.IsAboveMin,
                              () => Math.Sqrt(2) );

        Assert.AreEqual( 2, result );
    }
}
```

As soon as you create this test, the Smart Test Analyzer generates a warning message specifying that `Math.Sqrt` test need two other tests: `MinIncluded.IsBelowMin` and `MinIncluded.IsMin`.

This way, your test is clearer and you do not forget to write tests.

This enables you to have a **Logical Coverage**, independently of the source code of your tested member.

You can [learn more about Criteria](doc/Criteria/readme.md)

### Ranges

You can even do better for simple types: use Ranges.

Ranges enables you to specify the range of the values your tests represent, and the Smart Tests Analyzer will ensure you have test for the full range.

Thus, the previous example should use Ranges this way:

```C#
using NUnit.Framework;
using static SmartTests.SmartTest;

[TestFixture]
public class MathTest
{
    [Test]
    public void Sqrt_ValueGreaterThanMin()
    {
        var result = RunTest( Double.AboveOrEqual(0, out var value),
                              () => Math.Sqrt(value) );

        Assert.AreEqual( value, result * 2 );
    }
}
```

What is interesting with this approach is that:

1. Your intent (equivalence class) is clear.
2. You use a random value within the specified range, thus you will perhaps find a bug that would not be found if the test had an hard-coded value.

You can [learn more about Ranges](doc/Criteria/ranges.md) and [lean more about Enums Criteria](doc/Criteria/enums.md)
