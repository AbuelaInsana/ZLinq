// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Generic;
using Xunit;

namespace System.Linq.Tests
{
    public class ToHashSetTests
    {
        [Fact]
        public void NoExplicitComparer()
        {
            var hs = Enumerable.Range(0, 50).ToHashSet();
            Assert.IsType<HashSet<int>>(hs);
            Assert.Equal(50, hs.Count);
            Assert.Equal(EqualityComparer<int>.Default, hs.Comparer);
        }

        [Fact]
        public void ExplicitComparer()
        {
            var cmp = EqualityComparer<int>.Create((x, y) => x == y, x => x);
            var hs = Enumerable.Range(0, 50).ToHashSet(cmp);
            Assert.IsType<HashSet<int>>(hs);
            Assert.Equal(50, hs.Count);
            Assert.Same(cmp, hs.Comparer);
        }

        [Fact]
        public void RunOnce()
        {
            Enumerable.Range(0, 50).RunOnce().ToHashSet();
        }

        [Fact]
        public void TolerateNullElements()
        {
            // Unlike the keys of a dictionary, HashSet tolerates null items.
            Assert.DoesNotContain(null, new HashSet<string>());
            var hs = new[] { "abc", null, "def" }.ToHashSet();
            Assert.Contains(null, hs);
        }

        [Fact]
        public void TolerateDuplicates()
        {
            // ToDictionary throws on duplicates, because that is the normal behaviour
            // of Dictionary<TKey, TValue>.Add().
            // By the same token, since the normal behaviour of HashSet<T>.Add()
            // is to signal duplicates without an exception ToHashSet should
            // tolerate duplicates.
            var hs = Enumerable.Range(0, 50).Select(i => i / 5).ToHashSet();

            // The OrderBy isn't strictly necessary, but that depends upon an
            // implementation detail of HashSet, so explicitly force ordering.
            Assert.Equal(Enumerable.Range(0, 10), hs.OrderBy(i => i));
        }

        [Fact]
        public void ThrowOnNullSource()
        {
            Assert.Throws<ArgumentNullException>(() => ((IEnumerable<object>)null).ToHashSet());
        }
    }
}
