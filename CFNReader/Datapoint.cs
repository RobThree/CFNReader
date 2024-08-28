using System;

namespace CFNReader;

public readonly record struct Datapoint
{
    public TimeSpan Time { get; init; }
    public ValueCollection Values { get; init; }
}
