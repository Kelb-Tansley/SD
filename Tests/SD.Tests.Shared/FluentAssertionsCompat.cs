using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FluentAssertions;

public static class FluentAssertionsExtensions
{
    public static AssertionBuilder<T> Should<T>(this T value) => new(value);
}

public sealed class AssertionBuilder<T>
{
    private readonly T? _value;

    public AssertionBuilder(T? value)
    {
        _value = value;
    }

    public AssertionBuilder<T> And => this;

    public AssertionBuilder<T> NotBeNull()
    {
        Assert.IsNotNull(_value);
        return this;
    }

    public AssertionBuilder<T> BeEmpty()
    {
        if (_value is null)
        {
            Assert.Fail("Expected value to be empty, but it was null.");
            return this;
        }

        if (_value is string s)
        {
            Assert.AreEqual(0, s.Length);
            return this;
        }

        if (_value is IEnumerable enumerable)
        {
            var enumerator = enumerable.GetEnumerator();
            Assert.IsFalse(enumerator.MoveNext(), "Expected value to be empty.");
            return this;
        }

        Assert.Fail($"Expected value to be empty, but type '{_value.GetType().Name}' is not supported.");
        return this;
    }

    public AssertionBuilder<T> BeSameAs(object? expected, string? because = null)
    {
        Assert.AreSame(expected, _value, because ?? "Expected values to be the same instance.");
        return this;
    }

    public AssertionBuilder<T> NotBeSameAs(object? expected, string? because = null)
    {
        Assert.AreNotSame(expected, _value, because ?? "Expected values not to be the same instance.");
        return this;
    }

    public AssertionBuilder<T> HaveCount(int expected)
    {
        if (_value is ICollection collection)
        {
            Assert.AreEqual(expected, collection.Count);
            return this;
        }

        if (_value is IEnumerable enumerable)
        {
            var count = 0;
            foreach (var _ in enumerable)
            {
                count++;
            }

            Assert.AreEqual(expected, count);
            return this;
        }

        Assert.Fail($"HaveCount is not supported for type '{_value?.GetType().Name ?? "null"}'.");
        return this;
    }

    public AssertionBuilder<T> Contain(object? expected)
    {
        if (_value is IEnumerable enumerable)
        {
            if (expected is IEnumerable expectedEnumerable && expected is not string)
            {
                foreach (var expectedItem in expectedEnumerable)
                {
                    var found = false;
                    foreach (var actualItem in enumerable)
                    {
                        if (Equals(actualItem, expectedItem))
                        {
                            found = true;
                            break;
                        }
                    }

                    if (!found)
                    {
                        Assert.Fail("Expected collection to contain the specified items.");
                        return this;
                    }
                }

                return this;
            }

            foreach (var item in enumerable)
            {
                if (Equals(item, expected))
                {
                    return this;
                }
            }
        }

        Assert.Fail("Expected collection to contain the specified item.");
        return this;
    }

    public AssertionBuilder<T> ContainInOrder(params object?[] expectedItems)
    {
        if (_value is not IEnumerable enumerable)
        {
            Assert.Fail("ContainInOrder is only supported for IEnumerable values.");
            return this;
        }

        var index = 0;
        foreach (var item in enumerable)
        {
            if (index >= expectedItems.Length)
            {
                break;
            }

            if (!Equals(item, expectedItems[index]))
            {
                Assert.Fail($"Expected item at index {index} to be '{expectedItems[index]}', but found '{item}'.");
                return this;
            }

            index++;
        }

        Assert.AreEqual(expectedItems.Length, index, "Sequence did not contain the expected ordered items.");
        return this;
    }

    public AssertionBuilder<T> NotContain(object? expected)
    {
        if (_value is IEnumerable enumerable)
        {
            foreach (var item in enumerable)
            {
                if (Equals(item, expected))
                {
                    Assert.Fail("Expected collection not to contain the specified item.");
                    return this;
                }
            }

            return this;
        }

        Assert.Fail("NotContain is only supported for IEnumerable values.");
        return this;
    }

    public AssertionBuilder<T> Be<TExpected>(TExpected expected)
    {
        if (AreEquivalent(expected, _value))
        {
            return this;
        }

        if (TryNumericComparison(expected, _value, out var difference) && difference <= 1e-2)
        {
            return this;
        }

        Assert.Fail($"Expected value to be '{expected}', but it was '{_value}'.");
        return this;
    }

    public AssertionBuilder<T> BeApproximately(double expected, double tolerance)
    {
        var actual = ConvertToDouble(_value);
        Assert.AreEqual(expected, actual, tolerance);
        return this;
    }

    public AssertionBuilder<T> BeLessThanOrEqualTo(double expected, string? because = null)
    {
        var actual = ConvertToDouble(_value);
        Assert.IsTrue(actual <= expected, because ?? $"Expected {actual} to be less than or equal to {expected}.");
        return this;
    }

    public AssertionBuilder<T> BeEquivalentTo(object? expected)
    {
        if (_value is not IEnumerable actualEnumerable)
        {
            Assert.Fail("BeEquivalentTo is only supported for IEnumerable values.");
            return this;
        }

        if (expected is not IEnumerable expectedEnumerable)
        {
            Assert.Fail("BeEquivalentTo requires an IEnumerable value.");
            return this;
        }

        var actualItems = actualEnumerable.Cast<object?>().ToList();
        var expectedItems = expectedEnumerable.Cast<object?>().ToList();

        Assert.AreEqual(expectedItems.Count, actualItems.Count, "Expected collections to have the same number of items.");

        for (var index = 0; index < expectedItems.Count; index++)
        {
            Assert.AreEqual(expectedItems[index], actualItems[index], $"Expected item at index {index} to match.");
        }

        return this;
    }

    public AssertionBuilder<T> NotThrow()
    {
        if (_value is not Action action)
        {
            Assert.Fail("NotThrow is only supported for Action values.");
            return this;
        }

        try
        {
            action();
        }
        catch (Exception ex)
        {
            Assert.Fail($"Expected action not to throw, but it threw {ex.GetType().Name}: {ex.Message}");
        }

        return this;
    }

    public SingleItemAssertion<object?> ContainSingle()
    {
        if (_value is not IEnumerable enumerable)
        {
            Assert.Fail("ContainSingle is only supported for IEnumerable values.");
            return new SingleItemAssertion<object?>(null);
        }

        object? singleItem = null;
        var count = 0;

        foreach (var item in enumerable)
        {
            count++;
            singleItem = item;
        }

        Assert.AreEqual(1, count, $"Expected collection to contain exactly one item, but found {count}.");
        return new SingleItemAssertion<object?>(singleItem);
    }

    private static double ConvertToDouble(object? value)
    {
        if (value is null)
        {
            throw new AssertFailedException("Expected a non-null value for BeApproximately.");
        }

        return value switch
        {
            double d => d,
            float f => f,
            int i => i,
            long l => l,
            decimal m => (double)m,
            _ => Convert.ToDouble(value)
        };
    }

    private static bool AreEquivalent(object? expected, object? actual)
    {
        if (ReferenceEquals(expected, actual))
        {
            return true;
        }

        if (expected is null || actual is null)
        {
            return false;
        }

        if (IsNumeric(expected) && IsNumeric(actual))
        {
            return TryNumericComparison(expected, actual, out var difference) && difference <= 1e-10;
        }

        return Equals(expected, actual);
    }

    private static bool TryNumericComparison(object? expected, object? actual, out double difference)
    {
        difference = double.MaxValue;

        if (!IsNumeric(expected) || !IsNumeric(actual))
        {
            return false;
        }

        var expectedValue = ConvertToDouble(expected);
        var actualValue = ConvertToDouble(actual);
        difference = Math.Abs(expectedValue - actualValue);
        return true;
    }

    private static bool IsNumeric(object? value) => value is byte or sbyte or short or ushort or int or uint or long or ulong or float or double or decimal;
}

public sealed class SingleItemAssertion<T>
{
    private readonly T? _value;

    public SingleItemAssertion(T? value)
    {
        _value = value;
    }

    public SingleItemValue<T> Which => new(_value);
}

public sealed class SingleItemValue<T>
{
    private readonly T? _value;

    public SingleItemValue(T? value)
    {
        _value = value;
    }

    public AssertionBuilder<T> Should() => new(_value);

    public object? Number => GetValue(nameof(Number));

    private object? GetValue(string propertyName)
    {
        if (_value is null)
        {
            return null;
        }

        var property = _value.GetType().GetProperty(propertyName);
        return property?.GetValue(_value);
    }
}
