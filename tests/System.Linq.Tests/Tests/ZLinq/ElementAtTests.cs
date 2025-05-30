// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Collections.Immutable;
using Xunit;

namespace ZLinq.Tests
{
    public class ElementAtTests : EnumerableTests
    {
        [Fact]
        public void SameResultsRepeatCallsIntQuery()
        {
            var q0 = from x in new[] { 9999, 0, 888, -1, 66, -777, 1, 2, -12345 } where x > int.MinValue select x;
            var q1 = from x in new[] { 9999, 0, 888, -1, 66, -777, 1, 2, -12345 } where x > int.MinValue select x;
            var q2 = from x in new[] { 9999, 0, 888, -1, 66, -777, 1, 2, -12345 } where x > int.MinValue select x;
            Assert.Equal(q0.ElementAt(3), q0.ElementAt(3));
            Assert.Equal(q1.ElementAt(new Index(3)), q1.ElementAt(new Index(3)));
            Assert.Equal(q2.ElementAt(^6), q2.ElementAt(^6));
        }

        [Fact]
        public void SameResultsRepeatCallsStringQuery()
        {
            var q0 = from x in new[] { "!@#$%^", "C", "AAA", "", "Calling Twice", "SoS", string.Empty } where !string.IsNullOrEmpty(x) select x;
            var q1 = from x in new[] { "!@#$%^", "C", "AAA", "", "Calling Twice", "SoS", string.Empty } where !string.IsNullOrEmpty(x) select x;
            var q2 = from x in new[] { "!@#$%^", "C", "AAA", "", "Calling Twice", "SoS", string.Empty } where !string.IsNullOrEmpty(x) select x;
            Assert.Equal(q0.ElementAt(4), q0.ElementAt(4));
            Assert.Equal(q1.ElementAt(new Index(4)), q1.ElementAt(new Index(4)));
            Assert.Equal(q2.ElementAt(^2), q2.ElementAt(^2));
        }

        public static IEnumerable<object[]> TestData()
        {
            yield return [NumberRangeGuaranteedNotCollectionType(9, 1), 0, 1, 9];
            yield return [NumberRangeGuaranteedNotCollectionType(9, 10), 9, 1, 18];
            yield return [NumberRangeGuaranteedNotCollectionType(-4, 10), 3, 7, -1];

            yield return [new int[] { -4 }, 0, 1, -4];
            yield return [new int[] { 9, 8, 0, -5, 10 }, 4, 1, 10];
        }

        [Theory]
        [MemberData(nameof(TestData))]
        public void ElementAt(IEnumerable<int> source, int index, int indexFromEnd, int expected)
        {
            Assert.Equal(expected, source.ElementAt(index));
            Assert.Equal(expected, source.ElementAt(new Index(index)));
            Assert.Equal(expected, source.ElementAt(^indexFromEnd));
        }

        [Theory]
        [MemberData(nameof(TestData))]
        public void ElementAtRunOnce(IEnumerable<int> source, int index, int indexFromEnd, int expected)
        {
            Assert.Equal(expected, source.RunOnce().ElementAt(index));
            Assert.Equal(expected, source.RunOnce().ElementAt(new Index(index)));
            Assert.Equal(expected, source.RunOnce().ElementAt(^indexFromEnd));
        }

        [Fact]
        public void InvalidIndex_ThrowsArgumentOutOfRangeException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new int?[] { 9, 8 }.ElementAt(-1));
            Assert.Throws<ArgumentOutOfRangeException>(() => new int?[] { 9, 8 }.ElementAt(^3));
            Assert.Throws<ArgumentOutOfRangeException>(() => new int?[] { 9, 8 }.ElementAt(int.MaxValue));
            Assert.Throws<ArgumentOutOfRangeException>(() => new int?[] { 9, 8 }.ElementAt(int.MinValue));
            Assert.Throws<ArgumentOutOfRangeException>(() => new int?[] { 9, 8 }.ElementAt(new Index(int.MaxValue)));
            Assert.Throws<ArgumentOutOfRangeException>(() => new int?[] { 9, 8 }.ElementAt(^int.MaxValue));

            Assert.Throws<ArgumentOutOfRangeException>(() => new int[] { 1, 2, 3, 4 }.ElementAt(4));
            Assert.Throws<ArgumentOutOfRangeException>(() => new int[] { 1, 2, 3, 4 }.ElementAt(new Index(4)));
            Assert.Throws<ArgumentOutOfRangeException>(() => new int[] { 1, 2, 3, 4 }.ElementAt(^0));
            Assert.Throws<ArgumentOutOfRangeException>(() => new int[] { 1, 2, 3, 4 }.ElementAt(^5));

            Assert.Throws<ArgumentOutOfRangeException>(() => new int[0].ElementAt(0));
            Assert.Throws<ArgumentOutOfRangeException>(() => new int[0].ElementAt(new Index(0)));
            Assert.Throws<ArgumentOutOfRangeException>(() => new int[0].ElementAt(^0));
            Assert.Throws<ArgumentOutOfRangeException>(() => new int[0].ElementAt(^1));

            Assert.Throws<ArgumentOutOfRangeException>(() => NumberRangeGuaranteedNotCollectionType(-4, 5).ElementAt(-1));
            Assert.Throws<ArgumentOutOfRangeException>(() => NumberRangeGuaranteedNotCollectionType(-4, 5).ElementAt(int.MinValue));
            Assert.Throws<ArgumentOutOfRangeException>(() => NumberRangeGuaranteedNotCollectionType(-4, 5).ElementAt(int.MaxValue));
            Assert.Throws<ArgumentOutOfRangeException>(() => NumberRangeGuaranteedNotCollectionType(-4, 5).ElementAt(^6));
            Assert.Throws<ArgumentOutOfRangeException>(() => NumberRangeGuaranteedNotCollectionType(-4, 5).ElementAt(new Index(int.MaxValue)));
            Assert.Throws<ArgumentOutOfRangeException>(() => NumberRangeGuaranteedNotCollectionType(-4, 5).ElementAt(^int.MaxValue));

            Assert.Throws<ArgumentOutOfRangeException>(() => NumberRangeGuaranteedNotCollectionType(5, 5).ElementAt(5));
            Assert.Throws<ArgumentOutOfRangeException>(() => NumberRangeGuaranteedNotCollectionType(5, 5).ElementAt(new Index(5)));
            Assert.Throws<ArgumentOutOfRangeException>(() => NumberRangeGuaranteedNotCollectionType(5, 5).ElementAt(^0));

            Assert.Throws<ArgumentOutOfRangeException>(() => NumberRangeGuaranteedNotCollectionType(0, 0).ElementAt(0));
            Assert.Throws<ArgumentOutOfRangeException>(() => NumberRangeGuaranteedNotCollectionType(0, 0).ElementAt(new Index(0)));
            Assert.Throws<ArgumentOutOfRangeException>(() => NumberRangeGuaranteedNotCollectionType(0, 0).ElementAt(^0));
            Assert.Throws<ArgumentOutOfRangeException>(() => NumberRangeGuaranteedNotCollectionType(0, 0).ElementAt(^1));
        }

        [Fact]
        public void NullableArray_ValidIndex_ReturnsCorrectObject()
        {
            int?[] source = [9, 8, null, -5, 10];

            Assert.Null(source.ElementAt(2));
            Assert.Equal(-5, source.ElementAt(3));

            Assert.Null(source.ElementAt(new Index(2)));
            Assert.Equal(-5, source.ElementAt(new Index(3)));

            Assert.Null(source.ElementAt(^3));
            Assert.Equal(-5, source.ElementAt(^2));
        }

        [Fact]
        public void NullSource_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>("source", () => ((IEnumerable<int>)null).ElementAt(2));
            Assert.Throws<ArgumentNullException>("source", () => ((IEnumerable<int>)null).ElementAt(new Index(2)));
            Assert.Throws<ArgumentNullException>("source", () => ((IEnumerable<int>)null).ElementAt(^2));
        }

        [Fact]
        public void MutableSource()
        {
            var source = new List<int>() { 0, 1, 2, 3, 4 };

            {
                Assert.Equal(2, source.ElementAt(2));
                Assert.Equal(2, source.ElementAt(new Index(2)));
                Assert.Equal(2, source.ElementAt(^3));
            }

            {
                source.InsertRange(3, [-1, -2]);
                source.RemoveAt(0);
                Assert.Equal(-1, source.ElementAt(2));
                Assert.Equal(-1, source.ElementAt(new Index(2)));
                Assert.Equal(-1, source.ElementAt(^4));
            }
        }

        [Fact]
        public void MutableSourceNotList()
        {
            var source = new List<int>() { 0, 1, 2, 3, 4 };

            {
                var query1 = ForceNotCollection(source).Select(i => i);
                var query2 = ForceNotCollection(source).Select(i => i);
                var query3 = ForceNotCollection(source).Select(i => i);
                Assert.Equal(2, query1.ElementAt(2));
                Assert.Equal(2, query2.ElementAt(new Index(2)));
                Assert.Equal(2, query3.ElementAt(^3));
            }

            {
                var query1 = ForceNotCollection(source).Select(i => i);
                var query2 = ForceNotCollection(source).Select(i => i);
                var query3 = ForceNotCollection(source).Select(i => i);
                source.InsertRange(3, [-1, -2]);
                source.RemoveAt(0);
                Assert.Equal(-1, query1.ElementAt(2));
                Assert.Equal(-1, query2.ElementAt(new Index(2)));
                Assert.Equal(-1, query3.ElementAt(^4));
            }
        }

        [Fact]
        public void EnumerateElements()
        {
            const int ElementCount = 10;
            int state = -1;
            int moveNextCallCount = 0;
            Func<DelegateIterator<int>> source = () =>
            {
                state = -1;
                moveNextCallCount = 0;
                return new DelegateIterator<int>(
                    moveNext: () => { moveNextCallCount++; return ++state < ElementCount; },
                    current: () => state,
                    dispose: () => state = -1);
            };

            Assert.Equal(0, source().ElementAt(0));
            Assert.Equal(1, moveNextCallCount);
            Assert.Equal(0, source().ElementAt(new Index(0)));
            Assert.Equal(1, moveNextCallCount);

            Assert.Equal(5, source().ElementAt(5));
            Assert.Equal(6, moveNextCallCount);
            Assert.Equal(5, source().ElementAt(new Index(5)));
            Assert.Equal(6, moveNextCallCount);

            Assert.Equal(0, source().ElementAt(^ElementCount));
            Assert.Equal(ElementCount + 1, moveNextCallCount);
            Assert.Equal(5, source().ElementAt(^5));
            Assert.Equal(ElementCount + 1, moveNextCallCount);

            Assert.Throws<ArgumentOutOfRangeException>(() => source().ElementAt(ElementCount));
            Assert.Equal(ElementCount + 1, moveNextCallCount);
            Assert.Throws<ArgumentOutOfRangeException>(() => source().ElementAt(new Index(ElementCount)));
            Assert.Equal(ElementCount + 1, moveNextCallCount);
            Assert.Throws<ArgumentOutOfRangeException>(() => source().ElementAt(^0));
            Assert.Equal(0, moveNextCallCount);
        }

        [Fact]
        public void NonEmptySource_Consistency()
        {
            int[] source = [0, 1, 2, 3, 4, 5, 6, 7, 8, 9];

            Assert.Equal(5, source.ElementAt(5));
            Assert.Equal(5, source.ElementAt(new Index(5)));
            Assert.Equal(5, source.ElementAt(^5));

            Assert.Equal(0, source.ElementAt(0));
            Assert.Equal(0, source.ElementAt(new Index(0)));
            Assert.Equal(0, source.ElementAt(^10));

            Assert.Equal(9, source.ElementAt(9));
            Assert.Equal(9, source.ElementAt(new Index(9)));
            Assert.Equal(9, source.ElementAt(^1));

            Assert.Throws<ArgumentOutOfRangeException>(() => source.ElementAt(-1));
            Assert.Throws<ArgumentOutOfRangeException>(() => source.ElementAt(^11));

            Assert.Throws<ArgumentOutOfRangeException>(() => source.ElementAt(10));
            Assert.Throws<ArgumentOutOfRangeException>(() => source.ElementAt(new Index(10)));
            Assert.Throws<ArgumentOutOfRangeException>(() => source.ElementAt(^0));

            Assert.Throws<ArgumentOutOfRangeException>(() => source.ElementAt(int.MinValue));
            Assert.Throws<ArgumentOutOfRangeException>(() => source.ElementAt(^int.MaxValue));

            Assert.Throws<ArgumentOutOfRangeException>(() => source.ElementAt(int.MaxValue));
            Assert.Throws<ArgumentOutOfRangeException>(() => source.ElementAt(new Index(int.MaxValue)));
        }

        [Fact]
        public void NonEmptySource_Consistency_ThrowsIListIndexerException()
        {
            int[] source = [0, 1, 2, 3, 4, 5, 6, 7, 8, 9];
            Assert.Throws<ArgumentOutOfRangeException>(() => source.ElementAt(-1));
            Assert.Throws<ArgumentOutOfRangeException>(() => source.ElementAt(^11));
            // ImmutableArray<T> implements IList<T>. ElementAt calls ImmutableArray<T>'s indexer, which throws IndexOutOfRangeException instead of ArgumentOutOfRangeException.
            Assert.Throws<ArgumentOutOfRangeException>(() => ImmutableArray.Create(source).ElementAt(-1));
            Assert.Throws<ArgumentOutOfRangeException>(() => ImmutableArray.Create(source).ElementAt(^11));

            Assert.Throws<ArgumentOutOfRangeException>(() => source.ElementAt(10));
            Assert.Throws<ArgumentOutOfRangeException>(() => source.ElementAt(new Index(10)));
            Assert.Throws<ArgumentOutOfRangeException>(() => source.ElementAt(^0));
            Assert.Throws<ArgumentOutOfRangeException>(() => ImmutableArray.Create(source).ElementAt(10));
            Assert.Throws<ArgumentOutOfRangeException>(() => ImmutableArray.Create(source).ElementAt(new Index(10)));
            Assert.Throws<ArgumentOutOfRangeException>(() => ImmutableArray.Create(source).ElementAt(^0));
        }

        [Fact]
        public void NonEmptySource_Consistency_NotList()
        {
            int[] source = [0, 1, 2, 3, 4, 5, 6, 7, 8, 9];

            Assert.Equal(5, ForceNotCollection(source).ElementAt(5));
            Assert.Equal(5, ForceNotCollection(source).ElementAt(new Index(5)));
            Assert.Equal(5, ForceNotCollection(source).ElementAt(^5));

            Assert.Equal(0, ForceNotCollection(source).ElementAt(0));
            Assert.Equal(0, ForceNotCollection(source).ElementAt(new Index(0)));
            Assert.Equal(0, ForceNotCollection(source).ElementAt(^10));

            Assert.Equal(9, ForceNotCollection(source).ElementAt(9));
            Assert.Equal(9, ForceNotCollection(source).ElementAt(new Index(9)));
            Assert.Equal(9, ForceNotCollection(source).ElementAt(^1));

            Assert.Throws<ArgumentOutOfRangeException>(() => ForceNotCollection(source).ElementAt(-1));
            Assert.Throws<ArgumentOutOfRangeException>(() => ForceNotCollection(source).ElementAt(^11));

            Assert.Throws<ArgumentOutOfRangeException>(() => ForceNotCollection(source).ElementAt(10));
            Assert.Throws<ArgumentOutOfRangeException>(() => ForceNotCollection(source).ElementAt(new Index(10)));
            Assert.Throws<ArgumentOutOfRangeException>(() => ForceNotCollection(source).ElementAt(^0));



            Assert.Throws<ArgumentOutOfRangeException>(() => ForceNotCollection(source).ElementAt(int.MinValue));
            Assert.Throws<ArgumentOutOfRangeException>(() => ForceNotCollection(source).ElementAt(^int.MaxValue));

            Assert.Throws<ArgumentOutOfRangeException>(() => ForceNotCollection(source).ElementAt(int.MaxValue));
            Assert.Throws<ArgumentOutOfRangeException>(() => ForceNotCollection(source).ElementAt(new Index(int.MaxValue)));
        }

        [Fact]
        public void NonEmptySource_Consistency_ListPartition()
        {
            int[] source = [0, 1, 2, 3, 4, 5, 6, 7, 8, 9];

            Assert.Equal(5, ListPartitionOrEmpty(source).ElementAt(5));
            Assert.Equal(5, ListPartitionOrEmpty(source).ElementAt(new Index(5)));
            Assert.Equal(5, ListPartitionOrEmpty(source).ElementAt(^5));

            Assert.Equal(0, ListPartitionOrEmpty(source).ElementAt(0));
            Assert.Equal(0, ListPartitionOrEmpty(source).ElementAt(new Index(0)));
            Assert.Equal(0, ListPartitionOrEmpty(source).ElementAt(^10));

            Assert.Equal(9, ListPartitionOrEmpty(source).ElementAt(9));
            Assert.Equal(9, ListPartitionOrEmpty(source).ElementAt(new Index(9)));
            Assert.Equal(9, ListPartitionOrEmpty(source).ElementAt(^1));

            Assert.Throws<ArgumentOutOfRangeException>(() => ListPartitionOrEmpty(source).ElementAt(-1));
            Assert.Throws<ArgumentOutOfRangeException>(() => ListPartitionOrEmpty(source).ElementAt(^11));

            Assert.Throws<ArgumentOutOfRangeException>(() => ListPartitionOrEmpty(source).ElementAt(10));
            Assert.Throws<ArgumentOutOfRangeException>(() => ListPartitionOrEmpty(source).ElementAt(new Index(10)));
            Assert.Throws<ArgumentOutOfRangeException>(() => ListPartitionOrEmpty(source).ElementAt(^0));

            Assert.Throws<ArgumentOutOfRangeException>(() => ListPartitionOrEmpty(source).ElementAt(int.MinValue));
            Assert.Throws<ArgumentOutOfRangeException>(() => ListPartitionOrEmpty(source).ElementAt(^int.MaxValue));

            Assert.Throws<ArgumentOutOfRangeException>(() => ListPartitionOrEmpty(source).ElementAt(int.MaxValue));
            Assert.Throws<ArgumentOutOfRangeException>(() => ListPartitionOrEmpty(source).ElementAt(new Index(int.MaxValue)));
        }

        [Fact]
        public void NonEmptySource_Consistency_EnumerablePartition()
        {
            int[] source = [0, 1, 2, 3, 4, 5, 6, 7, 8, 9];

            Assert.Equal(5, EnumerablePartitionOrEmpty(source).ElementAt(5));
            Assert.Equal(5, EnumerablePartitionOrEmpty(source).ElementAt(new Index(5)));
            Assert.Equal(5, EnumerablePartitionOrEmpty(source).ElementAt(^5));

            Assert.Equal(0, EnumerablePartitionOrEmpty(source).ElementAt(0));
            Assert.Equal(0, EnumerablePartitionOrEmpty(source).ElementAt(new Index(0)));
            Assert.Equal(0, EnumerablePartitionOrEmpty(source).ElementAt(^10));

            Assert.Equal(9, EnumerablePartitionOrEmpty(source).ElementAt(9));
            Assert.Equal(9, EnumerablePartitionOrEmpty(source).ElementAt(new Index(9)));
            Assert.Equal(9, EnumerablePartitionOrEmpty(source).ElementAt(^1));

            Assert.Throws<ArgumentOutOfRangeException>(() => EnumerablePartitionOrEmpty(source).ElementAt(-1));
            Assert.Throws<ArgumentOutOfRangeException>(() => EnumerablePartitionOrEmpty(source).ElementAt(^11));

            Assert.Throws<ArgumentOutOfRangeException>(() => EnumerablePartitionOrEmpty(source).ElementAt(10));
            Assert.Throws<ArgumentOutOfRangeException>(() => EnumerablePartitionOrEmpty(source).ElementAt(new Index(10)));
            Assert.Throws<ArgumentOutOfRangeException>(() => EnumerablePartitionOrEmpty(source).ElementAt(^0));

            Assert.Throws<ArgumentOutOfRangeException>(() => EnumerablePartitionOrEmpty(source).ElementAt(int.MinValue));
            Assert.Throws<ArgumentOutOfRangeException>(() => EnumerablePartitionOrEmpty(source).ElementAt(^int.MaxValue));

            Assert.Throws<ArgumentOutOfRangeException>(() => EnumerablePartitionOrEmpty(source).ElementAt(int.MaxValue));
            Assert.Throws<ArgumentOutOfRangeException>(() => EnumerablePartitionOrEmpty(source).ElementAt(new Index(int.MaxValue)));
        }

        [Fact]
        public void NonEmptySource_Consistency_Collection()
        {
            int[] source = [0, 1, 2, 3, 4, 5, 6, 7, 8, 9];

            Assert.Equal(5, new TestCollection<int>(source).ElementAt(5));
            Assert.Equal(5, new TestCollection<int>(source).ElementAt(new Index(5)));
            Assert.Equal(5, new TestCollection<int>(source).ElementAt(^5));

            Assert.Equal(0, new TestCollection<int>(source).ElementAt(0));
            Assert.Equal(0, new TestCollection<int>(source).ElementAt(new Index(0)));
            Assert.Equal(0, new TestCollection<int>(source).ElementAt(^10));

            Assert.Equal(9, new TestCollection<int>(source).ElementAt(9));
            Assert.Equal(9, new TestCollection<int>(source).ElementAt(new Index(9)));
            Assert.Equal(9, new TestCollection<int>(source).ElementAt(^1));

            Assert.Throws<ArgumentOutOfRangeException>(() => new TestCollection<int>(source).ElementAt(-1));
            Assert.Throws<ArgumentOutOfRangeException>(() => new TestCollection<int>(source).ElementAt(^11));

            Assert.Throws<ArgumentOutOfRangeException>(() => new TestCollection<int>(source).ElementAt(10));
            Assert.Throws<ArgumentOutOfRangeException>(() => new TestCollection<int>(source).ElementAt(new Index(10)));
            Assert.Throws<ArgumentOutOfRangeException>(() => new TestCollection<int>(source).ElementAt(^0));

            Assert.Throws<ArgumentOutOfRangeException>(() => new TestCollection<int>(source).ElementAt(int.MinValue));
            Assert.Throws<ArgumentOutOfRangeException>(() => new TestCollection<int>(source).ElementAt(^int.MaxValue));

            Assert.Throws<ArgumentOutOfRangeException>(() => new TestCollection<int>(source).ElementAt(int.MaxValue));
            Assert.Throws<ArgumentOutOfRangeException>(() => new TestCollection<int>(source).ElementAt(new Index(int.MaxValue)));
        }

        [Fact]
        public void NonEmptySource_Consistency_NonGenericCollection()
        {
            int[] source = [0, 1, 2, 3, 4, 5, 6, 7, 8, 9];

            Assert.Equal(5, new TestNonGenericCollection<int>(source.ToArray()).ElementAt(5));
            Assert.Equal(5, new TestNonGenericCollection<int>(source.ToArray()).ElementAt(new Index(5)));
            Assert.Equal(5, new TestNonGenericCollection<int>(source.ToArray()).ElementAt(^5));

            Assert.Equal(0, new TestNonGenericCollection<int>(source.ToArray()).ElementAt(0));
            Assert.Equal(0, new TestNonGenericCollection<int>(source.ToArray()).ElementAt(new Index(0)));
            Assert.Equal(0, new TestNonGenericCollection<int>(source.ToArray()).ElementAt(^10));

            Assert.Equal(9, new TestNonGenericCollection<int>(source.ToArray()).ElementAt(9));
            Assert.Equal(9, new TestNonGenericCollection<int>(source.ToArray()).ElementAt(new Index(9)));
            Assert.Equal(9, new TestNonGenericCollection<int>(source.ToArray()).ElementAt(^1));

            Assert.Throws<ArgumentOutOfRangeException>(() => new TestNonGenericCollection<int>(source.ToArray()).ElementAt(-1));
            Assert.Throws<ArgumentOutOfRangeException>(() => new TestNonGenericCollection<int>(source.ToArray()).ElementAt(^11));

            Assert.Throws<ArgumentOutOfRangeException>(() => new TestNonGenericCollection<int>(source.ToArray()).ElementAt(10));
            Assert.Throws<ArgumentOutOfRangeException>(() => new TestNonGenericCollection<int>(source.ToArray()).ElementAt(new Index(10)));
            Assert.Throws<ArgumentOutOfRangeException>(() => new TestNonGenericCollection<int>(source.ToArray()).ElementAt(^0));

            Assert.Throws<ArgumentOutOfRangeException>(() => new TestNonGenericCollection<int>(source.ToArray()).ElementAt(int.MinValue));
            Assert.Throws<ArgumentOutOfRangeException>(() => new TestNonGenericCollection<int>(source.ToArray()).ElementAt(^int.MaxValue));

            Assert.Throws<ArgumentOutOfRangeException>(() => new TestNonGenericCollection<int>(source.ToArray()).ElementAt(int.MaxValue));
            Assert.Throws<ArgumentOutOfRangeException>(() => new TestNonGenericCollection<int>(source.ToArray()).ElementAt(new Index(int.MaxValue)));
        }

        [Fact]
        public void EmptySource_Consistency()
        {
            int[] source = [];

            Assert.Throws<ArgumentOutOfRangeException>(() => source.ElementAt(1));
            Assert.Throws<ArgumentOutOfRangeException>(() => source.ElementAt(-1));
            Assert.Throws<ArgumentOutOfRangeException>(() => source.ElementAt(new Index(1)));
            Assert.Throws<ArgumentOutOfRangeException>(() => source.ElementAt(^1));

            Assert.Throws<ArgumentOutOfRangeException>(() => source.ElementAt(0));
            Assert.Throws<ArgumentOutOfRangeException>(() => source.ElementAt(new Index(0)));
            Assert.Throws<ArgumentOutOfRangeException>(() => source.ElementAt(^0));

            Assert.Throws<ArgumentOutOfRangeException>(() => source.ElementAt(int.MinValue));
            Assert.Throws<ArgumentOutOfRangeException>(() => source.ElementAt(^int.MaxValue));

            Assert.Throws<ArgumentOutOfRangeException>(() => source.ElementAt(int.MaxValue));
            Assert.Throws<ArgumentOutOfRangeException>(() => source.ElementAt(new Index(int.MaxValue)));
        }

        [Fact]
        public void EmptySource_Consistency_ThrowsIListIndexerException()
        {
            int[] source = [];
            Assert.Throws<ArgumentOutOfRangeException>(() => source.ElementAt(-1));
            Assert.Throws<ArgumentOutOfRangeException>(() => source.ElementAt(^1));
            // ImmutableArray<T> implements IList<T>. ElementAt calls ImmutableArray<T>'s indexer, which throws IndexOutOfRangeException instead of ArgumentOutOfRangeException.
            Assert.Throws<ArgumentOutOfRangeException>(() => ImmutableArray.Create(source).ElementAt(-1));
            Assert.Throws<ArgumentOutOfRangeException>(() => ImmutableArray.Create(source).ElementAt(^1));

            Assert.Throws<ArgumentOutOfRangeException>(() => source.ElementAt(0));
            Assert.Throws<ArgumentOutOfRangeException>(() => source.ElementAt(^0));
            Assert.Throws<ArgumentOutOfRangeException>(() => ImmutableArray.Create(source).ElementAt(0));
            Assert.Throws<ArgumentOutOfRangeException>(() => ImmutableArray.Create(source).ElementAt(^0));

            Assert.Throws<ArgumentOutOfRangeException>(() => source.ElementAt(1));
            Assert.Throws<ArgumentOutOfRangeException>(() => source.ElementAt(new Index(1)));
            Assert.Throws<ArgumentOutOfRangeException>(() => ImmutableArray.Create(source).ElementAt(1));
            Assert.Throws<ArgumentOutOfRangeException>(() => ImmutableArray.Create(source).ElementAt(new Index(1)));
        }

        [Fact]
        public void EmptySource_Consistency_NotList()
        {
            int[] source = [];

            Assert.Throws<ArgumentOutOfRangeException>(() => ForceNotCollection(source).ElementAt(1));
            Assert.Throws<ArgumentOutOfRangeException>(() => ForceNotCollection(source).ElementAt(-1));
            Assert.Throws<ArgumentOutOfRangeException>(() => ForceNotCollection(source).ElementAt(new Index(1)));
            Assert.Throws<ArgumentOutOfRangeException>(() => ForceNotCollection(source).ElementAt(^1));

            Assert.Throws<ArgumentOutOfRangeException>(() => ForceNotCollection(source).ElementAt(0));
            Assert.Throws<ArgumentOutOfRangeException>(() => ForceNotCollection(source).ElementAt(new Index(0)));
            Assert.Throws<ArgumentOutOfRangeException>(() => ForceNotCollection(source).ElementAt(^0));

            Assert.Throws<ArgumentOutOfRangeException>(() => ForceNotCollection(source).ElementAt(int.MinValue));
            Assert.Throws<ArgumentOutOfRangeException>(() => ForceNotCollection(source).ElementAt(^int.MaxValue));

            Assert.Throws<ArgumentOutOfRangeException>(() => ForceNotCollection(source).ElementAt(int.MaxValue));
            Assert.Throws<ArgumentOutOfRangeException>(() => ForceNotCollection(source).ElementAt(new Index(int.MaxValue)));
        }

        [Fact]
        public void EmptySource_Consistency_ListPartition()
        {
            int[] source = [];

            Assert.Throws<ArgumentOutOfRangeException>(() => ListPartitionOrEmpty(source).ElementAt(1));
            Assert.Throws<ArgumentOutOfRangeException>(() => ListPartitionOrEmpty(source).ElementAt(-1));
            Assert.Throws<ArgumentOutOfRangeException>(() => ListPartitionOrEmpty(source).ElementAt(new Index(1)));
            Assert.Throws<ArgumentOutOfRangeException>(() => ListPartitionOrEmpty(source).ElementAt(^1));

            Assert.Throws<ArgumentOutOfRangeException>(() => ListPartitionOrEmpty(source).ElementAt(0));
            Assert.Throws<ArgumentOutOfRangeException>(() => ListPartitionOrEmpty(source).ElementAt(new Index(0)));
            Assert.Throws<ArgumentOutOfRangeException>(() => ListPartitionOrEmpty(source).ElementAt(^0));

            Assert.Throws<ArgumentOutOfRangeException>(() => ListPartitionOrEmpty(source).ElementAt(int.MinValue));
            Assert.Throws<ArgumentOutOfRangeException>(() => ListPartitionOrEmpty(source).ElementAt(^int.MaxValue));

            Assert.Throws<ArgumentOutOfRangeException>(() => ListPartitionOrEmpty(source).ElementAt(int.MaxValue));
            Assert.Throws<ArgumentOutOfRangeException>(() => ListPartitionOrEmpty(source).ElementAt(new Index(int.MaxValue)));
        }

        [Fact]
        public void EmptySource_Consistency_EnumerablePartition()
        {
            int[] source = [];

            Assert.Throws<ArgumentOutOfRangeException>(() => EnumerablePartitionOrEmpty(source).ElementAt(1));
            Assert.Throws<ArgumentOutOfRangeException>(() => EnumerablePartitionOrEmpty(source).ElementAt(-1));
            Assert.Throws<ArgumentOutOfRangeException>(() => EnumerablePartitionOrEmpty(source).ElementAt(new Index(1)));
            Assert.Throws<ArgumentOutOfRangeException>(() => EnumerablePartitionOrEmpty(source).ElementAt(^1));

            Assert.Throws<ArgumentOutOfRangeException>(() => EnumerablePartitionOrEmpty(source).ElementAt(0));
            Assert.Throws<ArgumentOutOfRangeException>(() => EnumerablePartitionOrEmpty(source).ElementAt(new Index(0)));
            Assert.Throws<ArgumentOutOfRangeException>(() => EnumerablePartitionOrEmpty(source).ElementAt(^0));

            Assert.Throws<ArgumentOutOfRangeException>(() => EnumerablePartitionOrEmpty(source).ElementAt(int.MinValue));
            Assert.Throws<ArgumentOutOfRangeException>(() => EnumerablePartitionOrEmpty(source).ElementAt(^int.MaxValue));

            Assert.Throws<ArgumentOutOfRangeException>(() => EnumerablePartitionOrEmpty(source).ElementAt(int.MaxValue));
            Assert.Throws<ArgumentOutOfRangeException>(() => EnumerablePartitionOrEmpty(source).ElementAt(new Index(int.MaxValue)));
        }

        [Fact]
        public void EmptySource_Consistency_Collection()
        {
            int[] source = [];

            Assert.Throws<ArgumentOutOfRangeException>(() => new TestCollection<int>(source).ElementAt(1));
            Assert.Throws<ArgumentOutOfRangeException>(() => new TestCollection<int>(source).ElementAt(-1));
            Assert.Throws<ArgumentOutOfRangeException>(() => new TestCollection<int>(source).ElementAt(new Index(1)));
            Assert.Throws<ArgumentOutOfRangeException>(() => new TestCollection<int>(source).ElementAt(^1));

            Assert.Throws<ArgumentOutOfRangeException>(() => new TestCollection<int>(source).ElementAt(0));
            Assert.Throws<ArgumentOutOfRangeException>(() => new TestCollection<int>(source).ElementAt(new Index(0)));
            Assert.Throws<ArgumentOutOfRangeException>(() => new TestCollection<int>(source).ElementAt(^0));

            Assert.Throws<ArgumentOutOfRangeException>(() => new TestCollection<int>(source).ElementAt(int.MinValue));
            Assert.Throws<ArgumentOutOfRangeException>(() => new TestCollection<int>(source).ElementAt(^int.MaxValue));

            Assert.Throws<ArgumentOutOfRangeException>(() => new TestCollection<int>(source).ElementAt(int.MaxValue));
            Assert.Throws<ArgumentOutOfRangeException>(() => new TestCollection<int>(source).ElementAt(new Index(int.MaxValue)));
        }

        [Fact]
        public void EmptySource_Consistency_NonGenericCollection()
        {
            int[] source = [];

            Assert.Throws<ArgumentOutOfRangeException>(() => new TestNonGenericCollection<int>(source.ToArray()).ElementAt(1));
            Assert.Throws<ArgumentOutOfRangeException>(() => new TestNonGenericCollection<int>(source.ToArray()).ElementAt(-1));
            Assert.Throws<ArgumentOutOfRangeException>(() => new TestNonGenericCollection<int>(source.ToArray()).ElementAt(new Index(1)));
            Assert.Throws<ArgumentOutOfRangeException>(() => new TestNonGenericCollection<int>(source.ToArray()).ElementAt(^1));

            Assert.Throws<ArgumentOutOfRangeException>(() => new TestNonGenericCollection<int>(source.ToArray()).ElementAt(0));
            Assert.Throws<ArgumentOutOfRangeException>(() => new TestNonGenericCollection<int>(source.ToArray()).ElementAt(new Index(0)));
            Assert.Throws<ArgumentOutOfRangeException>(() => new TestNonGenericCollection<int>(source.ToArray()).ElementAt(^0));

            Assert.Throws<ArgumentOutOfRangeException>(() => new TestNonGenericCollection<int>(source.ToArray()).ElementAt(int.MinValue));
            Assert.Throws<ArgumentOutOfRangeException>(() => new TestNonGenericCollection<int>(source.ToArray()).ElementAt(^int.MaxValue));

            Assert.Throws<ArgumentOutOfRangeException>(() => new TestNonGenericCollection<int>(source.ToArray()).ElementAt(int.MaxValue));
            Assert.Throws<ArgumentOutOfRangeException>(() => new TestNonGenericCollection<int>(source.ToArray()).ElementAt(new Index(int.MaxValue)));
        }
    }
}
