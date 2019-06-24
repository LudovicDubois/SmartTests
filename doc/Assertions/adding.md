# Adding a Smart Assertion

Before adding your own Smart Assertion, you should know what a Smart Assertion is.

If this is not the case, [read this](/doc/Assertions/readme.md).

Smart Assertions are sub-classes of `Assertion` class.

However, to simplify usage, you should define method factories as extension methods for the `SmartAssertPlaceHolder` class.
This way:

1. They seem to be methods of `SmartAssert` property.
1. You do not need to specify template arguments a they are inferred.

## `Assertion` class

THe two main methods of abstract class `Assertion` are:

1. `BeforeAct(ActBase)`
1. `AfterAct(ActBase)`

As their name suggested, they are called before and after *Act*, respectively.

## `ActBase` class

In `ActBase` class, you will find all you need to know about *Act*:

1. `Instance`: the instance involved, if any.
1. `Constructor`: the constructor involved, if any.
1. `Field`: the field involved, if any.
1. `Method`: the method involved, if any.
1. `Property`: the property involved, if any.
1. `Assertions`: an array of all Smart Assertions.
1. `Exception`: the exception thrown in *Act* if any (`null` in `BeforeAct`).

Two sub-classes are available, depending on the *Act* expression: `Act` and `Act<T>`. `AssignAct<>` is a sub-class of `Act<T>` to represent assignment (cast the `ActBase` instance to `IAssignee` to access its members: `AssigneeValue` and `AssignedValue`).

## Example

Let us look at the ChangeTo Smart Assertion:

We want to check that the actual value is different from the assigned value before *Act* (otherwise the test is bad) and that they are equal after *Act*.

```C#
public static class ChangedToAssertions
{
    public static Assertion ChangedTo( this SmartAssertPlaceHolder @this ) => new ChangedToAssertion();

    private class ChangedToAssertion: Assertion
    {
        private IAssignee _Assignee;
        private object _Value;

        public override void BeforeAct( ActBase act )
        {
            _Assignee = act as IAssignee;
            if( _Assignee == null )
                throw new BadTestException( Resource.BadTest_NotAssignment );

            _Value = _Assignee.AssignedValue;
            if( Equals( _Assignee.AssigneeValue, _Value ) )
                throw new BadTestException( string.Format( Resource.BadTest_UnexpectedValue, _Value ) );
        }


        public override void AfterAct( ActBase act )
        {
            var actualValue = _Assignee.AssigneeValue;
            if( !Equals( actualValue, _Value ) )
                throw new SmartTestException( string.Format( Resource.ChangeWrongly, _Value, actualValue ) );
        }
    }
```
