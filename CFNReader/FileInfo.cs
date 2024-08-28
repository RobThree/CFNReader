using System;

namespace CFNReader;

public readonly record struct FileInfo
{
    public UnitValue SampleRate { get; init; }
    public UnitValue StartCurrentCondition { get; init; }
    public UnitValue StopCurrentCondition { get; init; }
    public TimeSpan StopTimeCondition { get; init; }
    public uint Datapoints { get; init; }
    public ChannelCollection Channels { get; init; }
}
