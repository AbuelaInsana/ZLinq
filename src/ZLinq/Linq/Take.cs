﻿using System;

namespace ZLinq
{
    partial class ValueEnumerableExtensions
    {
        public static ValueEnumerable<Take<TEnumerator, TSource>, TSource> Take<TEnumerator, TSource>(this ValueEnumerable<TEnumerator, TSource> source, Int32 count)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            => new(new(source.Enumerator, count));

        public static ValueEnumerable<TakeRange<TEnumerator, TSource>, TSource> Take<TEnumerator, TSource>(this ValueEnumerable<TEnumerator, TSource> source, Range range)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            => new(new(source.Enumerator, range));

        // Optimize

        public static ValueEnumerable<TakeSkip<TEnumerator, TSource>, TSource> Skip<TEnumerator, TSource>(this ValueEnumerable<Take<TEnumerator, TSource>, TSource> source, int count)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            => new(source.Enumerator.Skip(count));

        public static ValueEnumerable<TakeSkip<TEnumerator, TSource>, TSource> Skip<TEnumerator, TSource>(this ValueEnumerable<TakeSkip<TEnumerator, TSource>, TSource> source, int count)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            => new(source.Enumerator.Skip(count));
    }
}

namespace ZLinq.Linq
{
    [StructLayout(LayoutKind.Auto)]
    [EditorBrowsable(EditorBrowsableState.Never)]
#if NET9_0_OR_GREATER
    public ref
#else
    public
#endif
    struct Take<TEnumerator, TSource>(TEnumerator source, Int32 count)
        : IValueEnumerator<TSource>
        where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        TEnumerator source = source;
        internal readonly int takeCount = Math.Max(0, count); // allows negative count
        int index;

        public bool TryGetNonEnumeratedCount(out int count)
        {
            if (source.TryGetNonEnumeratedCount(out count))
            {
                count = Math.Min(count, takeCount); // take smaller
                return true;
            }

            count = default;
            return false;
        }

        public bool TryGetSpan(out ReadOnlySpan<TSource> span)
        {
            if (source.TryGetSpan(out span))
            {
                span = span.Slice(0, Math.Min(span.Length, takeCount));
                return true;
            }

            span = default;
            return false;
        }

        public bool TryCopyTo(Span<TSource> destination, Index offset)
        {
            if (source.TryGetNonEnumeratedCount(out var count))
            {
                var actualTakeCount = Math.Min(count, takeCount);

                // When offset IsFromEnd, calculate the source offset from take count.
                var sourceOffset = offset.GetOffset(actualTakeCount);

                if (sourceOffset < 0 || sourceOffset >= actualTakeCount)
                {
                    return false; // out of range mark as fail(for example ElementAt needs failed information).
                }

                // Remaining differs depending on whether it's from the beginning or from the end.
                var remainingElements = offset.IsFromEnd
                    ? offset.Value
                    : actualTakeCount - sourceOffset;

                var elementsToCopy = Math.Min(remainingElements, destination.Length);
                return source.TryCopyTo(destination.Slice(0, elementsToCopy), sourceOffset);
            }

            return false;
        }

        public bool TryGetNext(out TSource current)
        {
            if (index++ < takeCount && source.TryGetNext(out current))
            {
                return true;
            }

            Unsafe.SkipInit(out current);
            return false;
        }

        public void Dispose()
        {
            source.Dispose();
        }

        // Optimize
        internal TakeSkip<TEnumerator, TSource> Skip(int skipCount) => new(source, takeCount: takeCount, skipCount: skipCount);
    }

    [StructLayout(LayoutKind.Auto)]
    [EditorBrowsable(EditorBrowsableState.Never)]
#if NET9_0_OR_GREATER
    public ref
#else
    public
#endif
    struct TakeRange<TEnumerator, TSource>(TEnumerator source, Range range)
        : IValueEnumerator<TSource>
        where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        TEnumerator source = source;
        readonly Range range = range;

        int index;
        int remains;
        int skipIndex;
        int fromEndQueueCount; // 0 is not use q
        RefBox<ValueQueue<TSource>>? q;
        bool isInitialized;

        void Init()
        {
            if (isInitialized)
            {
                return;
            }
            isInitialized = true;

            this.fromEndQueueCount = 0;
            this.remains = -1; // unknown

            if (source.TryGetNonEnumeratedCount(out var count))
            {
                var start = Math.Max(0, range.Start.GetOffset(count));
                var end = Math.Min(count, range.End.GetOffset(count));

                this.skipIndex = start;
                this.remains = end - start;
                if (end <= 0 || remains < 0)
                {
                    this.remains = 0; // isEmpty
                }
            }
            else
            {
                if (!range.Start.IsFromEnd && !range.End.IsFromEnd) // both fromstart
                {
                    this.skipIndex = range.Start.Value;
                    this.remains = range.End.Value - range.Start.Value;
                    if (remains < 0)
                    {
                        this.remains = 0; // isEmpty
                    }
                }
                else if (!range.Start.IsFromEnd && range.End.IsFromEnd) // end-fromend
                {
                    // unknown remains
                    this.skipIndex = range.Start.Value;
                    if (range.End.Value == 0)
                    {
                        this.remains = int.MaxValue; // get all
                        return;
                    }

                    this.fromEndQueueCount = int.MaxValue; // unknown queue count
                    this.q = new(new(4));
                }
                else if (range.Start.IsFromEnd && !range.End.IsFromEnd) // start-fromend
                {
                    // unknown skipIndex and remains
                    this.skipIndex = 0;
                    this.fromEndQueueCount = range.Start.Value; //queue size is fixed from end-of-start
                    if (this.fromEndQueueCount == 0) fromEndQueueCount = 1;
                    this.q = new(new(4));
                }
                else if (range.Start.IsFromEnd && range.End.IsFromEnd) // both fromend
                {
                    // unknown skipIndex and remains but maxCount can calc
                    this.skipIndex = 0;
                    var maxCount = range.Start.Value - range.End.Value; // maxCount but remains is unknown.
                    if (maxCount <= 0)
                    {
                        // empty
                        this.remains = 0;
                        return;
                    }
                    this.fromEndQueueCount = range.Start.Value;
                    if (this.fromEndQueueCount == 0) fromEndQueueCount = 1;
                    this.q = new(new(4));
                }
            }
        }

        public bool TryGetNonEnumeratedCount(out int count)
        {
            Init();

            if (source.TryGetNonEnumeratedCount(out _))
            {
                count = remains;
                return true;
            }
            count = default;
            return false;
        }

        public bool TryGetSpan(out ReadOnlySpan<TSource> span)
        {
            Init();

            if (source.TryGetSpan(out span))
            {
                span = span.Slice(skipIndex, remains);
                return true;
            }

            span = default;
            return false;
        }

        public bool TryCopyTo(Span<TSource> destination, Index offset)
        {
            Init();

            if (source.TryGetNonEnumeratedCount(out var totalCount))
            {
                var effectiveRemains = skipIndex < totalCount
                    ? Math.Min(remains, totalCount - skipIndex)
                    : 0;

                if (effectiveRemains <= 0)
                {
                    return false;
                }

                var offsetInRange = offset.GetOffset(effectiveRemains);

                if (offsetInRange < 0 || offsetInRange >= effectiveRemains)
                {
                    return false;
                }

                var sourceOffset = skipIndex + offsetInRange;

                var elementsAvailable = effectiveRemains - offsetInRange;

                var elementsToCopy = Math.Min(elementsAvailable, destination.Length);

                if (elementsToCopy <= 0)
                {
                    return false;
                }

                return source.TryCopyTo(destination.Slice(0, elementsToCopy), sourceOffset);
            }

            return false;
        }

        public bool TryGetNext(out TSource current)
        {
            Init();

            if (remains == 0)
            {
                goto END;
            }

        DEQUEUE:
            if (q != null && q.GetValueRef().Count != 0)
            {
                if (remains == -1)
                {
                    // calculate remains
                    var count = index;
                    var start = Math.Max(0, range.Start.GetOffset(count));
                    var end = Math.Min(count, range.End.GetOffset(count));

                    this.remains = end - start;
                    if (remains < 0)
                    {
                        goto END;
                    }

                    // q.Count is fromEnd
                    var offset = count - q.GetValueRef().Count;
                    var skipIndex = Math.Max(0, start - offset);
                    while (skipIndex > 0)
                    {
                        q.GetValueRef().Dequeue();
                        skipIndex--;
                    }
                }

                if (remains-- > 0)
                {
                    current = q.GetValueRef().Dequeue();
                    return true;
                }
                else
                {
                    goto END;
                }
            }

            while (source.TryGetNext(out current))
            {
                if (q == null)
                {
                    if (index++ < skipIndex)
                    {
                        continue; // skip
                    }

                    // take
                    if (remains > 0)
                    {
                        remains--;
                        return true;
                    }
                    return false;
                }
                else
                {
                    // from-last
                    if (index++ < skipIndex)
                    {
                        continue;
                    }

                    if (q.GetValueRef().Count == fromEndQueueCount)
                    {
                        q.GetValueRef().Dequeue();
                    }
                    q.GetValueRef().Enqueue(current);
                }
            }

            if (q != null && q.GetValueRef().Count != 0)
            {
                goto DEQUEUE;
            }

        END:
            this.remains = 0;
            Unsafe.SkipInit(out current);
            return false;
        }

        public void Dispose()
        {
            q?.Dispose();
            source.Dispose();
        }
    }

    [StructLayout(LayoutKind.Auto)]
    [EditorBrowsable(EditorBrowsableState.Never)]
#if NET9_0_OR_GREATER
    public ref
#else
    public
#endif
    struct TakeSkip<TEnumerator, TSource>(TEnumerator source, Int32 takeCount, Int32 skipCount)
        : IValueEnumerator<TSource>
        where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        TEnumerator source = source;
        readonly int takeCount = Math.Max(0, takeCount); // ensure non-negative
        readonly int skipCount = Math.Max(0, skipCount); // ensure non-negative
        int taken;
        int skipped;
        bool reachedTakeLimit;

        public bool TryGetNonEnumeratedCount(out int count)
        {
            if (source.TryGetNonEnumeratedCount(out count))
            {
                // Apply take limit first
                count = Math.Min(count, takeCount);

                // Then apply skip
                count = Math.Max(0, count - skipCount);

                return true;
            }

            count = default;
            return false;
        }

        public bool TryCopyTo(Span<TSource> destination, Index offset)
        {
            if (source.TryGetNonEnumeratedCount(out var sourceCount))
            {
                // First limit by take
                var afterTake = Math.Min(sourceCount, takeCount);

                // Then apply skip
                var actualSkipCount = Math.Min(afterTake, skipCount);
                var actualCount = afterTake - actualSkipCount;

                if (actualCount <= 0)
                {
                    return false;
                }

                // Calculate offset within the resulting sequence
                var offsetInResult = offset.GetOffset(actualCount);

                if (offsetInResult < 0 || offsetInResult >= actualCount)
                {
                    return false;
                }

                // Calculate the source offset (within the take window, then skip)
                var sourceOffset = actualSkipCount + offsetInResult;

                // Calculate elements available after offset
                var elementsAvailable = actualCount - offsetInResult;

                // Calculate elements to copy
                var elementsToCopy = Math.Min(elementsAvailable, destination.Length);

                if (elementsToCopy <= 0)
                {
                    return false;
                }

                return source.TryCopyTo(destination.Slice(0, elementsToCopy), sourceOffset);
            }

            return false;
        }

        public bool TryGetSpan(out ReadOnlySpan<TSource> span)
        {
            if (source.TryGetSpan(out span))
            {
                // First apply take limit
                if (span.Length > takeCount)
                {
                    span = span.Slice(0, takeCount);
                }

                // Then skip elements
                if (span.Length <= skipCount)
                {
                    span = default;
                    return true;
                }

                span = span.Slice(skipCount);
                return true;
            }

            span = default;
            return false;
        }

        public bool TryGetNext(out TSource current)
        {
            if (IsResultEmpty())
            {
                Unsafe.SkipInit(out current);
                return false;
            }

            // Once we've reached the take limit, we're done
            if (reachedTakeLimit)
            {
                Unsafe.SkipInit(out current);
                return false;
            }

            // Skip elements that have been emitted from the take portion
            while (skipped < skipCount)
            {
                // First ensure we haven't exceeded the take limit
                if (taken >= takeCount)
                {
                    reachedTakeLimit = true;
                    Unsafe.SkipInit(out current);
                    return false;
                }

                // Get next element from source
                if (!source.TryGetNext(out var _))
                {
                    Unsafe.SkipInit(out current);
                    return false;
                }

                taken++;

                // If we've reached the take limit before finishing skipping,
                // we won't be able to return any elements
                if (taken >= takeCount)
                {
                    reachedTakeLimit = true;
                    Unsafe.SkipInit(out current);
                    return false;
                }

                skipped++;
            }

            // Check if we've reached the take limit
            if (taken >= takeCount)
            {
                reachedTakeLimit = true;
                Unsafe.SkipInit(out current);
                return false;
            }

            // Return elements after taking and skipping
            if (source.TryGetNext(out current))
            {
                taken++;
                return true;
            }

            Unsafe.SkipInit(out current);
            return false;
        }

        bool IsResultEmpty()
        {
            if (takeCount == 0)
            {
                return true;
            }

            if (skipCount >= takeCount)
            {
                return true;
            }

            return false;
        }

        public void Dispose()
        {
            source.Dispose();
        }

        // Optimize

        internal TakeSkip<TEnumerator, TSource> Skip(int count)
        {
            if (count <= 0)
            {
                return this; // no changes.
            }

            // check overflow
            int newSkipCount;
            if (count > 0 && skipCount > int.MaxValue - count)
            {
                newSkipCount = int.MaxValue;
            }
            else
            {
                newSkipCount = skipCount + count;
            }

            return new TakeSkip<TEnumerator, TSource>(source, takeCount, newSkipCount);
        }
    }
}
