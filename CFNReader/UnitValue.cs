using System;
using System.Diagnostics;
using System.Globalization;

namespace CFNReader;

[DebuggerDisplay("{Value}{Unit}")]
public readonly record struct UnitValue
{
    public double Value { get; init; }
    public Unit Unit { get; init; }

    public UnitValue(double value, Unit unit)
    {
        Value = value;
        Unit = unit;
    }

    public static UnitValue FromChannel(double value, Channel channel)
        => new(
            value,
            channel switch
            {
                Channel.VBusVoltage => Unit.V,
                Channel.VBusCurrent => Unit.A,
                Channel.DPlusVoltage => Unit.V,
                Channel.DMinusVoltage => Unit.V,
                Channel.Power => Unit.W,
                Channel.AccumulatedCapacity => Unit.Ah,
                Channel.AccumulatedEnergy => Unit.Wh,
                _ => throw new ArgumentOutOfRangeException(nameof(channel), channel, $"Invalid {nameof(Channel)}")
            }
        );

    public override string ToString()
        => ToString("N4");

    public string ToString(string format)
        => ToString(format, CultureInfo.InvariantCulture);

    public string ToString(string format, IFormatProvider formatProvider)
        => string.Format("{0}{1}", Value.ToString(format, formatProvider), Unit);
}
