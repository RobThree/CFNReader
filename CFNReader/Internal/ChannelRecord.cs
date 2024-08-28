using System.Runtime.InteropServices;

namespace CFNReader.Internal;

[StructLayout(LayoutKind.Explicit, Size = 7)]
internal readonly record struct ChannelRecord
{
    [FieldOffset(0)] public readonly Channel Channel;
    [FieldOffset(2)] public readonly uint UnknownData;
    [FieldOffset(6)] public readonly byte MinMaxFlag;
}
