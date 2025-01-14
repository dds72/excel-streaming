﻿using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;

namespace Gooseberry.ExcelStreaming.Benchmarks;

[MemoryDiagnoser]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
public class RealWorldReportBenchmarks
{
    [Params(100, 1000, 10_000, 100_000, 500_000)]
    public int RowsCount { get; set; }

    [Benchmark]
    public async Task RealWorldReport()
    {
        await using var writer = new ExcelWriter(Stream.Null);

        await writer.StartSheet("PNL");
        var dateTime = DateTime.Now;

        for (var row = 0; row < RowsCount; row++)
        {
            await writer.StartRow();

            for (int i = 0; i < 5; i++)
            {
                writer.AddCell(row);
                writer.AddCell(dateTime);
                writer.AddCellUtf8String("\"Alice’s Adventures in Wonderland\" by Lewis Carroll"u8);
                writer.AddCell(1789);
                writer.AddCell(1234567.9876M);
                writer.AddCell(-936.9M);
                writer.AddCell(0.999M);
                writer.AddCell(23.00M);
                writer.AddCell(56688900.56M);
                writer.AddCellUtf8String("7895-654-098-45"u8);
                writer.AddCell(1789);
                writer.AddCell(1234567.9876M);
                writer.AddCell(-936.9M);
                writer.AddCell(0.999M);
                writer.AddCell(23.00M);
                writer.AddCell(56688900.56M);
                writer.AddCell(784000);
                writer.AddCell(dateTime);
                writer.AddCell(56688900.56M);
                writer.AddCell(784000);
            }
        }

        await writer.Complete();
    }
}