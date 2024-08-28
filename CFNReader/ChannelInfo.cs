namespace CFNReader;

public readonly record struct ChannelInfo
{
    public readonly UnitValue Max { get; init; }
    public readonly UnitValue Min { get; init; }
    public readonly int Index { get; init; }
}
