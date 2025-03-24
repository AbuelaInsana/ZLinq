﻿namespace ZLinq.Tests.Linq;

public class GroupJoinTest
{
    [Fact]
    public void GroupJoin_BasicFunctionality()
    {
        // Arrange
        var outer = new[] { 1, 2, 3, 4 };
        var inner = new[] { 2, 3, 4, 5 };

        // Act - Standard LINQ as reference
        var expected = outer
            .GroupJoin(inner, o => o, i => i, (o, g) => $"{o}:{string.Join(",", g)}")
            .ToArray();

        // Act - ZLinq
        var actual = outer
            .AsValueEnumerable()
            .GroupJoin(inner, o => o, i => i, (o, g) => $"{o}:{string.Join(",", g)}")
            .ToArray();

        // Assert
        actual.ShouldBe(expected);
    }

    [Fact]
    public void GroupJoin_WithComplexObjects()
    {
        // Arrange
        var customers = new[]
        {
            new { Id = 1, Name = "Alice" },
            new { Id = 2, Name = "Bob" },
            new { Id = 3, Name = "Charlie" }
        };

        var orders = new[]
        {
            new { CustomerId = 1, OrderId = 101 },
            new { CustomerId = 1, OrderId = 102 },
            new { CustomerId = 2, OrderId = 201 },
            new { CustomerId = 4, OrderId = 401 }
        };

        // Act - Standard LINQ as reference
        var expected = customers
            .GroupJoin(orders, c => c.Id, o => o.CustomerId, (c, orders) => new { Customer = c.Name, OrderCount = orders.Count() })
            .ToArray();

        // Act - ZLinq
        var actual = customers
            .AsValueEnumerable()
            .GroupJoin(orders.AsValueEnumerable(), c => c.Id, o => o.CustomerId, (c, orders) => new { Customer = c.Name, OrderCount = orders.Count() })
            .ToArray();

        // Assert
        actual.Length.ShouldBe(expected.Length);
        for (int i = 0; i < actual.Length; i++)
        {
            actual[i].Customer.ShouldBe(expected[i].Customer);
            actual[i].OrderCount.ShouldBe(expected[i].OrderCount);
        }
    }

    [Fact]
    public void GroupJoin_WithCustomComparer()
    {
        // Arrange
        var outer = new[] { "A", "B", "C", "D" };
        var inner = new[] { "a", "b", "c", "e" };
        var comparer = StringComparer.OrdinalIgnoreCase;

        // Act - Standard LINQ as reference
        var expected = outer
            .GroupJoin(inner, o => o, i => i, (o, g) => $"{o}:[{string.Join(",", g)}]", comparer)
            .ToArray();

        // Act - ZLinq
        var actual = outer
            .AsValueEnumerable()
            .GroupJoin(inner.AsValueEnumerable(), o => o, i => i, (o, g) => $"{o}:[{string.Join(",", g)}]", comparer)
            .ToArray();

        // Assert
        actual.ShouldBe(expected);
    }

    [Fact]
    public void GroupJoin_WithEmptyOuter()
    {
        // Arrange
        var outer = Array.Empty<int>();
        var inner = new[] { 1, 2, 3, 4 };

        // Act - Standard LINQ as reference
        var expected = outer
            .GroupJoin(inner, o => o, i => i, (o, g) => $"{o}:{string.Join(",", g)}")
            .ToArray();

        // Act - ZLinq
        var actual = outer
            .AsValueEnumerable()
            .GroupJoin(inner.AsValueEnumerable(), o => o, i => i, (o, g) => $"{o}:{string.Join(",", g)}")
            .ToArray();

        // Assert
        actual.ShouldBeEmpty();
        actual.ShouldBe(expected);
    }

    [Fact]
    public void GroupJoin_WithEmptyInner()
    {
        // Arrange
        var outer = new[] { 1, 2, 3, 4 };
        var inner = Array.Empty<int>();

        // Act - Standard LINQ as reference
        var expected = outer
            .GroupJoin(inner, o => o, i => i, (o, g) => $"{o}:{string.Join(",", g)}")
            .ToArray();

        // Act - ZLinq
        var actual = outer
            .AsValueEnumerable()
            .GroupJoin(inner.AsValueEnumerable(), o => o, i => i, (o, g) => $"{o}:{string.Join(",", g)}")
            .ToArray();

        // Assert
        actual.Length.ShouldBe(expected.Length);
        actual.ShouldBe(expected);
    }

    [Fact]
    public void GroupJoin_WithNullKeys()
    {
        // Arrange
        var outer = new[]
        {
            new { Id = 1, Key = "A" },
            new { Id = 2, Key = (string)null! },
            new { Id = 3, Key = "C" },
        };

        var inner = new[]
        {
            new { Id = 101, Key = "A" },
            new { Id = 102, Key = (string)null! },
            new { Id = 103, Key = "C" },
        };

        // Act - Standard LINQ as reference
        var expected = outer
            .GroupJoin(inner, o => o.Key, i => i.Key, (o, g) => new { OuterId = o.Id, GroupCount = g.Count() })
            .ToArray();

        // Act - ZLinq
        var actual = outer
            .AsValueEnumerable()
            .GroupJoin(inner.AsValueEnumerable(), o => o.Key, i => i.Key, (o, g) => new { OuterId = o.Id, GroupCount = g.Count() })
            .ToArray();

        // Assert
        actual.Length.ShouldBe(expected.Length);
        for (int i = 0; i < actual.Length; i++)
        {
            actual[i].OuterId.ShouldBe(expected[i].OuterId);
            actual[i].GroupCount.ShouldBe(expected[i].GroupCount);
        }
    }

    [Fact]
    public void GroupJoin_WithMultipleMatches()
    {
        // Arrange
        var outer = new[] { 1, 2, 3 };
        var inner = new[] { 1, 2, 2, 3, 3, 3 };

        // Act - Standard LINQ as reference
        var expected = outer
            .GroupJoin(inner, o => o, i => i, (o, g) => new { Key = o, Count = g.Count() })
            .ToArray();

        // Act - ZLinq
        var actual = outer
            .AsValueEnumerable()
            .GroupJoin(inner.AsValueEnumerable(), o => o, i => i, (o, g) => new { Key = o, Count = g.Count() })
            .ToArray();

        // Assert
        actual.Length.ShouldBe(expected.Length);
        for (int i = 0; i < actual.Length; i++)
        {
            actual[i].Key.ShouldBe(expected[i].Key);
            actual[i].Count.ShouldBe(expected[i].Count);
        }
    }

    [Fact]
    public void GroupJoin_WithIEnumerableInner()
    {
        // Arrange
        var outer = new[] { 1, 2, 3, 4 };
        var inner = Enumerable.Range(1, 5); // 1, 2, 3, 4, 5

        // Act - Standard LINQ as reference
        var expected = outer
            .GroupJoin(inner, o => o, i => i, (o, g) => $"{o}:{string.Join(",", g)}")
            .ToArray();

        // Act - ZLinq
        var actual = outer
            .AsValueEnumerable()
            .GroupJoin(inner, o => o, i => i, (o, g) => $"{o}:{string.Join(",", g)}")
            .ToArray();

        // Assert
        actual.ShouldBe(expected);
    }

    [Fact]
    public void GroupJoin_WithIEnumerableInnerAndComparer()
    {
        // Arrange
        var outer = new[] { "A", "B", "C", "D" };
        var inner = new List<string> { "a", "b", "c", "e" };
        var comparer = StringComparer.OrdinalIgnoreCase;

        // Act - Standard LINQ as reference
        var expected = outer
            .GroupJoin(inner, o => o, i => i, (o, g) => $"{o}:{string.Join(",", g)}", comparer)
            .ToArray();

        // Act - ZLinq
        var actual = outer
            .AsValueEnumerable()
            .GroupJoin(inner, o => o, i => i, (o, g) => $"{o}:{string.Join(",", g)}", comparer)
            .ToArray();

        // Assert
        actual.ShouldBe(expected);
    }

    [Fact]
    public void GroupJoin_EnsuresDisposal()
    {
        // Arrange
        var outerDisposed = false;
        var innerDisposed = false;

        IEnumerable<int> GetOuterSequence()
        {
            try
            {
                yield return 1;
                yield return 2;
                yield return 3;
            }
            finally
            {
                outerDisposed = true;
            }
        }

        IEnumerable<int> GetInnerSequence()
        {
            try
            {
                yield return 2;
                yield return 3;
                yield return 4;
            }
            finally
            {
                innerDisposed = true;
            }
        }

        // Act
        var result = GetOuterSequence()
            .AsValueEnumerable()
            .GroupJoin(GetInnerSequence().AsValueEnumerable(), o => o, i => i, (o, g) => $"{o}:{g.Count()}")
            .ToArray();

        // Assert
        outerDisposed.ShouldBeTrue();
        innerDisposed.ShouldBeTrue();
        result.Length.ShouldBe(3);
    }

    [Fact]
    public void GroupJoin_DisposesInnerEvenWithIncompleteOuterIteration()
    {
        // Arrange
        var innerDisposed = false;

        IEnumerable<int> GetInnerSequence()
        {
            try
            {
                yield return 2;
                yield return 3;
                yield return 4;
            }
            finally
            {
                innerDisposed = true;
            }
        }

        var outer = new[] { 1, 2, 3, 4 };

        // Act
        var joinQuery = outer
            .AsValueEnumerable()
            .GroupJoin(GetInnerSequence().AsValueEnumerable(), o => o, i => i, (o, g) => new { Key = o, Group = g });

        // Only take the first result and then dispose
        var firstResult = joinQuery.First();

        TestUtil.Dispose(joinQuery);

        // Assert
        innerDisposed.ShouldBeTrue();
    }

    [Fact]
    public void GroupJoin_TryGetNonEnumeratedCount_ReturnsFalse()
    {
        // Arrange
        var outer = new[] { 1, 2, 3 };
        var inner = new[] { 2, 3, 4 };

        var groupJoinQuery = outer
            .AsValueEnumerable()
            .GroupJoin(inner.AsValueEnumerable(), o => o, i => i, (o, g) => new { Key = o, Group = g });

        // Act
        var result = groupJoinQuery.TryGetNonEnumeratedCount(out var count);

        // Assert
        result.ShouldBeFalse();
    }

    [Fact]
    public void GroupJoin_TryGetSpan_ReturnsFalse()
    {
        // Arrange
        var outer = new[] { 1, 2, 3 };
        var inner = new[] { 2, 3, 4 };

        var groupJoinQuery = outer
            .AsValueEnumerable()
            .GroupJoin(inner.AsValueEnumerable(), o => o, i => i, (o, g) => new { Key = o, Group = g });

        // Act
        var result = groupJoinQuery.TryGetSpan(out var span);

        // Assert
        result.ShouldBeFalse();
    }

    [Fact]
    public void GroupJoin_TryCopyTo_ReturnsFalse()
    {
        // Arrange
        var outer = new[] { 1, 2, 3 };
        var inner = new[] { 2, 3, 4 };
        var result = new (int, IEnumerable<int>)[3];

        var groupJoinQuery = outer
            .AsValueEnumerable()
            .GroupJoin(inner.AsValueEnumerable(), o => o, i => i, (o, g) => (Key: o, Group: g));

        // Act
        var copyResult = groupJoinQuery.TryCopyTo(result.AsSpan(), 0);

        // Assert
        copyResult.ShouldBeFalse();
    }
}
