using System;
using System.Linq;
using System.Text;
using Gooseberry.ExcelStreaming.Styles;

namespace Gooseberry.ExcelStreaming.Writers;

internal sealed class StringCellWriter
{
    private readonly byte[] _statelessPrefix;
    private readonly byte[] _statePrefix;
    private readonly byte[] _statePostfix;

    public StringCellWriter()
    {
        _statelessPrefix = Constants.Worksheet.SheetData.Row.Cell.Prefix
            .Concat(Constants.Worksheet.SheetData.Row.Cell.StringDataType)
            .Concat(Constants.Worksheet.SheetData.Row.Cell.Middle)
            .ToArray();

        _statePrefix = Constants.Worksheet.SheetData.Row.Cell.Prefix
            .Concat(Constants.Worksheet.SheetData.Row.Cell.StringDataType)
            .Concat(Constants.Worksheet.SheetData.Row.Cell.Style.Prefix)
            .ToArray();
        
        _statePostfix = Constants.Worksheet.SheetData.Row.Cell.Style.Postfix
            .Concat(Constants.Worksheet.SheetData.Row.Cell.Middle)
            .ToArray();
    }
    
    public void Write(ReadOnlySpan<char> value, BuffersChain buffer, Encoder encoder, StyleReference? style = null)
    {
        var span = buffer.GetSpan();
        var written = 0;

        if (style.HasValue)
        {
            _statePrefix.WriteTo(buffer, ref span, ref written);
            style.Value.Value.WriteTo(buffer, ref span, ref written);
            _statePostfix.WriteTo(buffer, ref span, ref written);
            value.WriteEscapedTo(buffer, encoder, ref span, ref written);
            Constants.Worksheet.SheetData.Row.Cell.Postfix.WriteTo(buffer, ref span, ref written);
            
            buffer.Advance(written);
            return;
        }

        _statelessPrefix.WriteTo(buffer, ref span, ref written);
        value.WriteEscapedTo(buffer, encoder, ref span, ref written);
        Constants.Worksheet.SheetData.Row.Cell.Postfix.WriteTo(buffer, ref span, ref written);
            
        buffer.Advance(written);        
    }
}