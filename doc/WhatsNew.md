# What's new

## version 1.10

It's now possible to [avoid generation of some values](Cases/readme.md#With-excluded-values) when generating random number from equivalence classes.
Especially useful for tests of properties: when you change the property value, you do not want the actual value to be randomly chosen.

## version 1.9

[`Criteria`](Criteria/readme.md) can now be [`Ranges`](Criteria/ranges.md) of `decimal` or `DateTime` values.
For `DateTime` values, you have to create the `DateTime` instance with constant in the `RunTest` statement.
Ensuring all possible values are tested.
To avoid name clash with `System` ones (`Double`, `Enum`, `Decimal` and `DateTime`), I now use the System name with `Range` suffix.
All previous notation is obsolete now.
Thus, `SmartTest.Double.Range...` is now `DoubleRange.Range...`.
For homogeneity, names with no conflicts use the same approach: `Int.Range` is not `Int32.Range`.

## version 1.8

[`Case`](Cases/readme.md#with-lambda-expression-and-equivalence-class) can now be associated with parameter properties and fields and the equivalence class.
Ensuring better representation of which properties/fields and equivalence classes are involved in a test.

## version 1.7

[`Case`](Cases/readme.md#with-lambda-expression) can now be associated with parameter properties and fields.
Ensuring better representation of which properties/fields are involved in a test.

## version 1.6

[`Criteria`](Criteria/readme.md) can now be [`Enums`](Criteria/enums.md).
Ensuring all possible values are tested.

## version 1.5

[`Criteria`](Criteria/readme.md) can now be [`Ranges`](Criteria/ranges.md) of real values.
Ensuring all possible values are tested.

## Version 1.4

[`Criteria`](Criteria/readme.md) can now be [`Ranges`](Criteria/ranges.md) of integer values.
Ensuring all possible values are tested.

## Version 1.3

SmartTests Visual Studio addin added.

## Version 1.2

None

## Version 1.1

### In the library

Two new Smart Assertions:

1. [`Wait`](Assertions/wait.md): to test parallel code.
2. [`Within`](Assertions/within.md): to test code is done within specific time.

### In the Analyzer

Support for xUnit.

## Version 1

First version
