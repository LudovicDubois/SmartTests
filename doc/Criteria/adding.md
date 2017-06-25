# Adding a Criteria

Before adding your own Criteria, you should know what a Criteria is.

It this is not the case, [read this](readme.md).

First, you have to identify your *Logical Intent*.

Then, you have to identify the different Criterion for the Logical Intent.

Now, you are ready to create your own Criteria:

1. Create a class that is a subclass of `Criteria`.
1. Add one `public static readonly` field for each Criterion.
   Each field is of type the Criteria class itself.  
   Each field is directly initialized with a new instance of the Criteria class itself.

If a criterion represent an error, the according field should be decorated with the attribute `[Error]`.

## Example

Let say we want a Criteria for a value from two ranges.

For example, value can be in [-100, -10] or [10, 100].

1. We want to create a `In2Ranges` Criteria.
1. we have 9 criterions:
   1. Below -100 (an error)
   1. -100
   1. Between -100 and 10
   1. -10
   1. Between -10 and 10 (an error)
   1. 10
   1. Between 10 and 100
   1. 100
   1. Above 100 (an error)

Thus, we can define a new criteria with 9 fields, one for each criterion:

```C#
public class In2Ranges: Criteria
{
    [Error]
    public static readonly Criteria IsBelowMinRange1 = new In2Ranges();
    public static readonly Criteria IsMinRange1 = new In2Ranges();
    public static readonly Criteria IsInRange1 = new In2Ranges();
    public static readonly Criteria IsMaxRange1 = new In2Ranges();
    [Error]
    public static readonly Criteria IsBetweenRanges = new In2Ranges();
    public static readonly Criteria IsMinRange2 = new In2Ranges();
    public static readonly Criteria IsInRange2 = new In2Ranges();
    public static readonly Criteria IsMaxRange2 = new In2Ranges();
    [Error]
    public static readonly Criteria IsAboveMaxRange2 = new In2Ranges();
}
```

You can now use it as any other Criteria.
