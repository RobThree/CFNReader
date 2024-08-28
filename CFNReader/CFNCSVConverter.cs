using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CFNReader;

public class CFNCSVConverter(
    IEnumerable<Channel>? channels = null,
    Func<Datapoint, bool>? predicate = null,
    IFormatProvider? formatProvider = null,
    string separator = ";",
    Encoding? encoding = null,
    string timeFormat = "G",
    string valueFormat = "N4",
    bool includeTime = true,
    bool includeUnit = false,
    int limitDataPoints = int.MaxValue
)
{
    private readonly IEnumerable<Channel>? _channels = channels;
    private readonly Func<Datapoint, bool> _predicate = predicate ?? (_ => true);
    private readonly IFormatProvider _formatprovider = formatProvider ?? CultureInfo.InvariantCulture;
    private readonly string _separator = separator;
    private readonly Encoding _encoding = encoding ?? Encoding.UTF8;
    private readonly string _timeformat = timeFormat;
    private readonly string _valueformat = valueFormat;
    private readonly bool _includetime = includeTime;
    private readonly bool _includeunit = includeUnit;
    private readonly int _maxdatapoints = limitDataPoints;

    public async Task ConvertToCSVAsync(Stream cnfStream, Stream csvStream, CancellationToken cancellationToken = default)
    {
        var cfnreader = new CFNStreamReader(cnfStream);

        var fileinfo = await cfnreader.ReadFileInfoAsync(cancellationToken);

        var exportchannels = (_channels ?? fileinfo.Channels.Keys).Where(fileinfo.Channels.ContainsKey).ToArray();

        await foreach (var dp in cfnreader.ReadDatapointsAsync(fileinfo, cancellationToken).Where(_predicate).Take(_maxdatapoints))
        {
            var line = string.Join(_separator, GetValues(exportchannels, dp));
            var buffer = _encoding.GetBytes(line + Environment.NewLine);
            await csvStream.WriteAsync(buffer, 0, buffer.Length, cancellationToken);
        }
    }

    private IEnumerable<string> GetValues(Channel[] channels, Datapoint datapoint)
    {
        if (_includetime)
        {
            yield return datapoint.Time.ToString(_timeformat, _formatprovider);
        }
        foreach (var channel in channels)
        {
            yield return FormatValue(datapoint.Values[channel]);
        }
    }

    private string FormatValue(UnitValue value)
        => $"{value.Value.ToString(_valueformat, _formatprovider)}{(_includeunit ? value.Unit : string.Empty)}";
}
