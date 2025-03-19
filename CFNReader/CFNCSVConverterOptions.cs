using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace CFNReader;

public record CFNCSVConverterOptions
{
    public IEnumerable<Channel> Channels { get; init; } = DefaultChannels;
    public Func<Datapoint, bool> Predicate { get; init; } = DefaultPredicate;
    public IFormatProvider? FormatProvider { get; init; } = DefaultFormatProvider;
    public string Separator { get; init; } = DefaultSeparator;
    public Encoding? Encoding { get; init; } = DefaultEncoding;
    public string TimeFormat { get; init; } = DefaultTimeFormat;
    public string ValueFormat { get; init; } = DefaultValueFormat;
    public bool IncludeHeader { get; init; } = DefaultIncludeHeader;
    public bool IncludeTime { get; init; } = DefaultIncludeTime;
    public bool IncludeUnit { get; init; } = DefaultIncludeUnit;
    public int LimitDataPoints { get; init; } = DefaultLimitDataPoints;

    public static readonly CFNCSVConverterOptions Default = new();

    public static readonly IEnumerable<Channel> DefaultChannels = [Channel.VBusVoltage, Channel.VBusCurrent];
    public static readonly Func<Datapoint, bool> DefaultPredicate = _ => true;
    public static readonly IFormatProvider DefaultFormatProvider = CultureInfo.InvariantCulture;
    public const string DefaultSeparator = ";";
    public static readonly Encoding DefaultEncoding = Encoding.UTF8;
    public const string DefaultTimeFormat = "G";
    public const string DefaultValueFormat = "N4";
    public const bool DefaultIncludeHeader = true;
    public const bool DefaultIncludeTime = true;
    public const bool DefaultIncludeUnit = false;
    public const int DefaultLimitDataPoints = int.MaxValue;
}
