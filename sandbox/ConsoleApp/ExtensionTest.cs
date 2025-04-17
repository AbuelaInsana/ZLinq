﻿using System.Collections;
using ZLinq;
using TakoyakiX;

[ZLinqDropInExtension]
public class GlobalList : IEnumerable<int>
{
    public IEnumerator<int> GetEnumerator()
    {
        throw new NotImplementedException();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

[ZLinqDropInExtension]
internal class GlobalListVI : IEnumerable<int>, IValueEnumerable<GlobalListVI.Enumerator, int>
{
    public ValueEnumerable<Enumerator, int> AsValueEnumerable()
    {
        throw new NotImplementedException();
    }

    public IEnumerator<int> GetEnumerator()
    {
        throw new NotImplementedException();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public struct Enumerator : IValueEnumerator<int>
    {
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public bool TryCopyTo(scoped Span<int> destination, Index offset)
        {
            throw new NotImplementedException();
        }

        public bool TryGetNext(out int current)
        {
            throw new NotImplementedException();
        }

        public bool TryGetNonEnumeratedCount(out int count)
        {
            throw new NotImplementedException();
        }

        public bool TryGetSpan(out ReadOnlySpan<int> span)
        {
            throw new NotImplementedException();
        }
    }
}

[ZLinqDropInExtension]
public class GlobalReferenceList : IEnumerable<TakoyakiX.FooBarBazBoz>
{
    public IEnumerator GetEnumerator()
    {
        throw new NotImplementedException();
    }

    IEnumerator<FooBarBazBoz> IEnumerable<FooBarBazBoz>.GetEnumerator()
    {
        throw new NotImplementedException();
    }
}

namespace TakoyakiX
{
    public class FooBarBazBoz
    {
    }
}

namespace AIU.EO.KA
{
    [ZLinqDropInExtension]
    public class GenericList<T> : IEnumerable<T>
    {
        public IEnumerator<T> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    [ZLinqDropInExtension]
    public class GenericList2<T> : IValueEnumerable<GenericList2<T>.Enumerator, T>
    {
        public ValueEnumerable<Enumerator, T> AsValueEnumerable()
        {
            throw new NotImplementedException();
        }

        public struct Enumerator : IValueEnumerator<T>
        {
            public void Dispose()
            {
                throw new NotImplementedException();
            }

            public bool TryCopyTo(scoped Span<T> destination, Index offset)
            {
                throw new NotImplementedException();
            }

            public bool TryGetNext(out T current)
            {
                throw new NotImplementedException();
            }

            public bool TryGetNonEnumeratedCount(out int count)
            {
                throw new NotImplementedException();
            }

            public bool TryGetSpan(out ReadOnlySpan<T> span)
            {
                throw new NotImplementedException();
            }
        }
    }

    [ZLinqDropInExtension]
    public class ConstraintList<T> : IEnumerable<T>
        where T : struct
    {
        public IEnumerator<T> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    [ZLinqDropInExtension]
    public class ConstraintList2<T> : IEnumerable<T>
        where T : class
    {
        public IEnumerator<T> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    // error
    // [ZLinqDropInExtension]
    public class Kankeinasu
    {

    }

    // error
    // [ZLinqDropInExtension]
    public class Kankeiaru<TKK, TVV> : IEnumerable<KeyValuePair<TKK, TVV>>
    {
        public IEnumerator<KeyValuePair<TKK, TVV>> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
