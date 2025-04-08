﻿using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZLinq;
using ZLinq.Simd;

namespace Benchmark;

[Orderer(SummaryOrderPolicy.FastestToSlowest)]
public class SimdRange
{
    int[] dest = new int[10000];

    public SimdRange()
    {

    }

    [Benchmark]
    public void For()
    {
        for (int i = 0; i < dest.Length; i++)
        {
            dest[i] = i;
        }
    }

    [Benchmark]
    public void Range()
    {
        ValueEnumerable.Range(0, 10000).CopyTo(dest.AsSpan());
    }

//#if NET8_0_OR_GREATER
//    [Benchmark]
//    public void Range2()
//    {
//        dest.VectorizedFillRange(0);
//    }

//#endif
}
