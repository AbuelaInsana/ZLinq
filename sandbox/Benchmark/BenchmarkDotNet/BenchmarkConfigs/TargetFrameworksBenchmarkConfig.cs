﻿using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Toolchains.CsProj;

namespace Benchmark;

/// <summary>
/// BenchmarkConfig that run benchmarks for multiple target frameworks.
/// </summary>
public class TargetFrameworksBenchmarkConfig : BaseBenchmarkConfig
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultBenchmarkConfig"/> class.
    /// </summary>
    public TargetFrameworksBenchmarkConfig() : base()
    {
        var baseJobConfig = GetBaseJobConfig().Freeze();

        // Note: Run benchmark with `-runtimes` parameters.
        AddJob(baseJobConfig.WithToolchain(CsProjCoreToolchain.NetCoreApp80).WithId(".NET 8").AsBaseline());
        AddJob(baseJobConfig.WithToolchain(CsProjCoreToolchain.NetCoreApp90).WithId(".NET 9"));
        // TODO: Currently BenchmarkDotNet don't support .NET10
        //AddJob(baseJobConfig.WithToolchain(CsProjCoreToolchain.NetCoreApp10_0).WithId(".NET 10"));

        // Configure additional settings.
        AddConfigurations();
    }

    protected override void AddColumnHidingRules()
    {
        HideColumns(Column.Toolchain); // There is same information at JobId column.
    }
}
