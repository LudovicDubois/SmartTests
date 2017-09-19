# Extending Smart Assertions

If you have to write several lines of code in a test to assert what ever you need, then you should create a *Smart Assertion*.

A Smart Assertion is a sub-class of the `Assertion` class.

## `SmartAssert` Property

It is not so easy to create instances, especially if they have type parameters.

Thus, we use method factories to create instances: the type parameters can be inferred from parameters of the method factory.

The `SmartAssert` static property of the main class `SmartTest` cannot have all this method factories as:
1. this class would be very huge
1. it will not be easy to extend it with new method factories (except for the developers of *SmartTests* library).

Thus, we have decided to use extensions method instead. It is very easy to add extension methods, without changing any existing code.

This way, it is very easy for the user of this library to extend it.

Thus, `SmartAssert` static property is a dummy property with a dummy type (`SmartAssertPlaceHolder`, an empty type) that is the object for added extensions methods.

## Creating your own Smart Assertion

### XAssertions class

Adding *Smart Assertions* only need to define a new static class that has extension methods for an object of type `SmartAssertPlaceHolder`.

Convention is to name it with the assertion name you want, with a suffix of `Assertions`.

For example, the `ChangedTo` Smart Assertion is defined in a static class `ChangedToAssertions`.

```CSharp
namespace SmartTests.Assertions
{
    public static class ChangedToAssertions
    {
        ...
    }
}
```

### Extensions methods

These extension methods should create your sub-class of `Assertion` class. Typically, a private nested class of your class.

Convention is to name it with the same assertion name you want, but with a suffix of `Assertion`

For example, the `ChangedTo` Smart Assertion is defined with methods `ChangedTo` and they instantiate a nested class whose name is `ChangedToAssertion`.

```CSharp
namespace SmartTests.Assertions
{
    public static class ChangedToAssertions
    {
        public static Assertion ChangedTo<T>( this SmartAssertPlaceHolder @this, Expression<Func<T>> after, T value ) 
            => new ChangedToAssertion<T>( after, value );

        private class ChangedToAssertion<T>: Assertion
        {
            public ChangedToAssertion( Expression<Func<T>> after, T value )
            {
                ...
            }

            ...
        }
    }
}
```

## `Assertion` Class

`Assertion` class is the base class of all *Smart Assertions*.

It is very simple: it has two methods only:

1. `BeforeAct`: called before the *Act* is run.
1. `AfterAct`: called after the *Act* is run.

```CSharp
namespace SmartTests
{
    public abstract class Assertion
    {
        public abstract void BeforeAct( ActBase act );
        public abstract void AfterAct( ActBase act );
    }
}
```

Thus, you only have to override these methods in you Smart Assertion class and do... what ever you want, **before** and **after** *Act*!

## `ActBase` Class

You probably need to know some information to implement your `BeforeAct` and `AfterAct` methods.

It is the role of the `ActBase` class.

It contains these read only properties:

1. `object Instance`: the instance implied in the *Act*, if any (`null` when testing static members).
1. `ConstructorInfo Constructor`: the constructor implied in the *Act*, if any.
1. `FieldInfo Field`: the field implied in the *Act*, if any.
1. `MethodInfo Method`: the field implied in the *Act*, if any.
1. `PropertyInfo Property`: the field implied in the *Act*, if any.
1. `Exception Exception`: the Exception that is raised within the `RunTest method` (can be in `BeforeAct` of any assertion, in the *Act* or in `AfterAct` of any assertion).

You also can know all the *Smart Assertions* involved:

1. `Assertion[] Assertions`: an array of all `Assertions` involved  in the test.
    Currently, no Smart Assertion depend on it... but, it used internally.

## `ChangedTo` Smart Assertions

Thus, we can see the source code for `ChangedTo` Assertion:

```CSharp
namespace SmartTests.Assertions
{
    public static class ChangedToAssertions
    {
        public static Assertion ChangedTo<T>( this SmartAssertPlaceHolder @this, Expression<Func<T>> after, T value ) 
            => new ChangedToAssertion<T>( after, value );

        private class ChangedToAssertion<T>: Assertion
        {
            public ChangedToAssertion( Expression<Func<T>> after, T value )
            {
                _After = after;
                _Value = value;
            }


            private readonly Expression<Func<T>> _After;
            private readonly T _Value;
            private Func<T> _Compiled;


            public override void BeforeAct( ActBase act )
            {
                _Compiled = _After.Compile();
                if( Equals( _Compiled(), _Value ) )
                    throw new BadTestException( string.Format( Resource.BadTest_UnexpectedValue, _Value ) );
            }


            public override void AfterAct( ActBase act )
            {
                var actualValue = _Compiled();
                if( !Equals( actualValue, _Value ) )
                    throw new SmartTestException( string.Format( Resource.ChangeWrongly, _Value, actualValue ) );
            }
        }
    }
}
```

In this simple *Smart Assertion*, we do not need any information from the *Act* itself. But, there is a more powerful `ChangedTo`: `ChangedTo()`!

This one infer is a little bit more complex. We will not see it here, but you can still access its source code, as Smart Tests is an [open source](https://github.com/LudovicDubois/SmartTests).