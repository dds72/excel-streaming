﻿namespace Gooseberry.ExcelStreaming.Helpers;

internal static class EmuConverter
{
    public static long ConvertToEnglishMetricUnits(int pixels, double resolution)
        => Convert.ToInt64(914400L * pixels / resolution);
}