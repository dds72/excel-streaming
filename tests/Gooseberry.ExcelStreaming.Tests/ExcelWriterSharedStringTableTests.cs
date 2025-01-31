using FluentAssertions;
using Gooseberry.ExcelStreaming.Tests.Excel;
using Xunit;

namespace Gooseberry.ExcelStreaming.Tests;

public sealed class ExcelWriterSharedStringTableTests
{
    [Fact]
    public async Task WritingCorrectSharedStrings()
    {
        var outputStream = new MemoryStream();

        var builder = new SharedStringTableBuilder();
        var stringReference = builder.GetOrAdd("string");
        var otherStringReference = builder.GetOrAdd("other string");

        await using var writer = new ExcelWriter(outputStream, sharedStringTable: builder.Build());

        await writer.StartSheet("test sheet");
        await writer.StartRow();

        writer.AddCell(stringReference);
        writer.AddCell(otherStringReference);

        await writer.Complete();


        outputStream.Seek(0, SeekOrigin.Begin);
        var sharedStrings = ExcelReader.ReadSharedStrings(outputStream);

        sharedStrings.Should().BeEquivalentTo(new[] { "string", "other string" });

        outputStream.Seek(0, SeekOrigin.Begin);
        var sheets = ExcelReader.ReadSheets(outputStream);

        var expectedSheet = new Excel.Sheet(
            "test sheet",
            new[]
            {
                new Row(new[]
                {
                    new Cell("0", CellValueType.SharedString),
                    new Cell("1", CellValueType.SharedString),
                })
            });

        sheets.ShouldBeEquivalentTo(expectedSheet);
    }

    [Fact]
    public async Task AddingCellWithSharedStringAndSharedStringTable_WritesCorrect()
    {
        var outputStream = new MemoryStream();

        var builder = new SharedStringTableBuilder();
        var stringReference = builder.GetOrAdd("string");
        var otherStringReference = builder.GetOrAdd("other string");

        await using var writer = new ExcelWriter(outputStream, sharedStringTable: builder.Build());
        await writer.StartSheet("test sheet");
        await writer.StartRow();

        writer.AddCell(stringReference);
        writer.AddCell(otherStringReference);
        writer.AddCellSharedString("third string");
        writer.AddCellSharedString("one more string");

        await writer.Complete();

        outputStream.Seek(0, SeekOrigin.Begin);
        var sharedStrings = ExcelReader.ReadSharedStrings(outputStream);

        sharedStrings.Should().BeEquivalentTo(new[] { "string", "other string", "third string", "one more string" });

        outputStream.Seek(0, SeekOrigin.Begin);
        var sheets = ExcelReader.ReadSheets(outputStream);

        var expectedSheet = new Excel.Sheet(
            "test sheet",
            new[]
            {
                new Row(new[]
                {
                    new Cell("0", CellValueType.SharedString),
                    new Cell("1", CellValueType.SharedString),
                    new Cell("2", CellValueType.SharedString),
                    new Cell("3", CellValueType.SharedString),
                })
            });

        sheets.ShouldBeEquivalentTo(expectedSheet);
    }

    [Fact]
    public async Task AddingCellWithIncorrectReference_Throws()
    {
        var outputStream = new MemoryStream();

        var builder = new SharedStringTableBuilder();
        var stringReference = builder.GetOrAdd("string");

        await using var writer = new ExcelWriter(outputStream);

        var act = () => writer.AddCell(stringReference);

        act.Should().Throw<ArgumentException>().WithMessage("Invalid shared string reference*");
    }
}