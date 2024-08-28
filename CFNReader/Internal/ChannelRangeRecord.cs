using System.Runtime.InteropServices;

namespace CFNReader.Internal;

[StructLayout(LayoutKind.Explicit, Size = 16)]
internal readonly record struct ChannelRangeRecord
{
    [FieldOffset(0)] public readonly double Max;
    [FieldOffset(8)] public readonly double Min;

    public ChannelRangeRecord(double max, double min)
    {
        Max = max;
        Min = min;
    }

    public static ChannelRangeRecord NoValue = new(double.NaN, double.NaN);
}
