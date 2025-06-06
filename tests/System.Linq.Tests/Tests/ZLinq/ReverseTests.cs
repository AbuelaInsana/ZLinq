// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Generic;
using Xunit;

namespace ZLinq.Tests
{
    public class ReverseTests : EnumerableTests
    {
        [Fact]
        public void InvalidArguments()
        {
            AssertExtensions.Throws<ArgumentNullException>("source", () => Enumerable.Reverse<string>((IEnumerable<string>)null));
            AssertExtensions.Throws<ArgumentNullException>("source", () => Enumerable.Reverse<string>((string[])null));
        }

        [Theory]
        [MemberData(nameof(ReverseData))]
        public void Reverse<T>(IEnumerable<T> source)
        {
            T[] expected = source.ToArray();
            Array.Reverse(expected);

            var actual = source.Reverse();

            Assert.Equal(expected, actual);
            Assert.Equal(expected.Count(), actual.Count()); // Count may be optimized.
            Assert.Equal(expected, actual.ToArray());
            Assert.Equal(expected, actual.ToList());

            Assert.Equal(expected.FirstOrDefault(), actual.FirstOrDefault());
            Assert.Equal(expected.LastOrDefault(), actual.LastOrDefault());

            for (int i = 0; i < expected.Length; i++)
            {
                Assert.Equal(expected[i], actual.ElementAt(i));

                Assert.Equal(expected.Skip(i), actual.Skip(i));
                Assert.Equal(expected.Take(i), actual.Take(i));
            }

            Assert.Equal(default(T), actual.ElementAtOrDefault(-1));
            Assert.Equal(default(T), actual.ElementAtOrDefault(expected.Length));

            Assert.Equal(expected, actual.Select(_ => _));
            Assert.Equal(expected, actual.Where(_ => true));

            Assert.Equal(actual, actual); // Repeat the enumeration against itself.
        }

        [Theory]
        [MemberData(nameof(ReverseData))]
        public void ReverseArray<T>(IEnumerable<T> source)
        {
            T[] expected = source.ToArray();
            Array.Reverse(expected);

            var actual = source.ToArray().Reverse();

            Assert.Equal(expected, actual);
            Assert.Equal(expected.Count(), actual.Count()); // Count may be optimized.
            Assert.Equal(expected, actual.ToArray());
            Assert.Equal(expected, actual.ToList());

            Assert.Equal(expected.FirstOrDefault(), actual.FirstOrDefault());
            Assert.Equal(expected.LastOrDefault(), actual.LastOrDefault());

            for (int i = 0; i < expected.Length; i++)
            {
                Assert.Equal(expected[i], actual.ElementAt(i));

                Assert.Equal(expected.Skip(i), actual.Skip(i));
                Assert.Equal(expected.Take(i), actual.Take(i));
            }

            Assert.Equal(default(T), actual.ElementAtOrDefault(-1));
            Assert.Equal(default(T), actual.ElementAtOrDefault(expected.Length));

            Assert.Equal(expected, actual.Select(_ => _));
            Assert.Equal(expected, actual.Where(_ => true));

            Assert.Equal(actual, actual); // Repeat the enumeration against itself.
        }

        [Theory, MemberData(nameof(ReverseData))]
        public void RunOnce<T>(IEnumerable<T> source)
        {
            T[] expected = source.ToArray();
            Array.Reverse(expected);

            var actual = source.RunOnce().Reverse();

            Assert.Equal(expected, actual);
        }

        public static IEnumerable<object[]> ReverseData()
        {
            var integers = new[]
            {
                Array.Empty<int>(), // No elements.
                [1], // One element.
                [9999, 0, 888, -1, 66, -777, 1, 2, -12345], // Distinct elements.
                [-10, 0, 5, 0, 9, 100, 9], // Some repeating elements.
            };

            return integers
                .Select(collection => new object[] { collection })
                .Concat(integers.Select(c => new object[] { c.Select(i => i.ToString()).ToArray() }).ToArray())
                .ToArray();
        }

        [Fact(Skip = SkipReason.EnumeratorBehaviorDifference)]
        public void ForcedToEnumeratorDoesntEnumerate()
        {
            var valueEnumerable = NumberRangeGuaranteedNotCollectionType(0, 3).Reverse();
            // Don't insist on this behaviour, but check it's correct if it happens
            using var en = valueEnumerable.Enumerator;
            Assert.False(en.TryGetNext(out _));
        }
    }
}
