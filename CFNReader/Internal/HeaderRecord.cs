using System.Runtime.InteropServices;

namespace CFNReader.Internal;

[StructLayout(LayoutKind.Explicit, Size = 22)]
internal readonly record struct HeaderRecord
{
    [FieldOffset(0)] public readonly double SampleRate;             // samples / sec
    [FieldOffset(8)] public readonly uint StartCurrentCondition;    // mA
    [FieldOffset(12)] public readonly uint StopCurrentCondition;    // mA
    [FieldOffset(16)] public readonly uint StopTimeCondition;       // sec
    [FieldOffset(20)] public readonly ushort ChannelCount;
}
