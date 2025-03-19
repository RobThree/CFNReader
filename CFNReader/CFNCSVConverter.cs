using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CFNReader;

public class CFNCSVConverter(IOptions<CFNCSVConverterOptions> options)
{
    private readonly CFNCSVConverterOptions _options = options?.Value ?? throw new ArgumentNullException(nameof(options));

    public CFNCSVConverter(CFNCSVConverterOptions options)
        : this(Options.Create(options)) { }

    public CFNCSVConverter()
        : this(Options.Create(CFNCSVConverterOptions.Default)) { }

    public async Task ConvertToCSVAsync(Stream cnfStream, Stream csvStream, CancellationToken cancellationToken = default)
    {
        var cfnreader = new CFNStreamReader(cnfStream);

        var fileinfo = await cfnreader.ReadFileInfoAsync(cancellationToken);

        var exportchannels = (_options.Channels ?? fileinfo.Channels.Keys).Where(fileinfo.Channels.ContainsKey).ToArray();

        using var sw = new StreamWriter(csvStream, _options.Encoding, -1, true);

        if (_options.IncludeHeader)
        {
            await sw.WriteLineAsync(string.Join(_options.Separator, GetHeaderNames(exportchannels)));
        }

        await foreach (var dp in cfnreader.ReadDatapointsAsync(fileinfo, cancellationToken).Where(_options.Predicate).Take(_options.LimitDataPoints))
        {
            await sw.WriteLineAsync(string.Join(_options.Separator, GetValues(exportchannels, dp)));
        }
    }

    private IEnumerable<string> GetHeaderNames(Channel[] channels)
    {
        if (_options.IncludeTime)
        {
            yield return nameof(Datapoint.Time);
        }
        foreach (var channel in channels)
        {
            yield return channel.ToString();
        }
    }

    private IEnumerable<string> GetValues(Channel[] channels, Datapoint datapoint)
    {
        if (_options.IncludeTime)
        {
            yield return datapoint.Time.ToString(_options.TimeFormat, _options.FormatProvider);
        }
        foreach (var channel in channels)
        {
            yield return FormatValue(datapoint.Values[channel]);
        }
    }

    private string FormatValue(UnitValue value)
        => $"{value.Value.ToString(_options.ValueFormat, _options.FormatProvider)}{(_options.IncludeUnit ? value.Unit : string.Empty)}";
}
