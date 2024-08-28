using CFNReader.Internal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace CFNReader;

public class CFNStreamReader(Stream stream)
{
    public async Task<FileInfo> ReadFileInfoAsync(CancellationToken cancellationToken = default)
    {
        if (stream.Position != 0)
        {
            throw new CFNStreamReaderException("Not at start of the stream.");
        }

        var header = await ReadHeaderAsync(cancellationToken);
        var channels = await ReadChannelsAsync(header.ChannelCount, cancellationToken);

        return new FileInfo()
        {
            SampleRate = new UnitValue(header.SampleRate, Unit.Hz),
            StartCurrentCondition = new UnitValue(header.StartCurrentCondition / 1000d, Unit.A),
            StopCurrentCondition = new UnitValue(header.StopCurrentCondition / 1000d, Unit.A),
            StopTimeCondition = TimeSpan.FromSeconds(header.StopTimeCondition),
            Channels = new ChannelCollection(channels.Select((channel, index) => (channel, index)).ToDictionary(c => c.channel.ChannelRecord.Channel, c => new ChannelInfo()
            {
                Max = UnitValue.FromChannel(c.channel.ChannelRangeRecord.Max, c.channel.ChannelRecord.Channel),
                Min = UnitValue.FromChannel(c.channel.ChannelRangeRecord.Min, c.channel.ChannelRecord.Channel),
                Index = c.index
            })),
            Datapoints = await ReadDatapointsAsync(cancellationToken)
        };
    }

    public async IAsyncEnumerable<Datapoint> ReadDatapointsAsync(FileInfo fileInfo, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var valuecount = fileInfo.Channels.Count + 1;           // Number of doubles in each record (1 for time + channelcount)
        var valuebuffer = new byte[valuecount * _doublesize];   // Allocate buffer for reading values

        for (var i = 0; i < fileInfo.Datapoints; i++)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                yield break;
            }

            var data = await ReadDataAsync(stream, valuebuffer, valuecount, cancellationToken);
            yield return new Datapoint
            {
                Time = TimeSpan.FromSeconds(data[0]),
                Values = new ValueCollection(fileInfo.Channels.ToDictionary(c => c.Key, c => UnitValue.FromChannel(data[c.Value.Index], c.Key)))
            };
        }
    }

    private static readonly int _doublesize = sizeof(double);
    private static async Task<double[]> ReadDataAsync(Stream stream, byte[] buffer, int count, CancellationToken cancellationToken = default)
    {
        var bytesread = await stream.ReadAsync(buffer, 0, buffer.Length, cancellationToken);
        return bytesread != buffer.Length
            ? throw new MalformedRecordException(typeof(UnitValue[]), stream.Position, buffer.Length, bytesread)
            : Enumerable.Range(0, count).Select(i => BitConverter.ToDouble(buffer, i * _doublesize)).ToArray();
    }

    private Task<HeaderRecord> ReadHeaderAsync(CancellationToken cancellationToken = default)
        => ReadStruct<HeaderRecord>(cancellationToken);

    private async Task<IEnumerable<(ChannelRecord ChannelRecord, ChannelRangeRecord ChannelRangeRecord)>> ReadChannelsAsync(int channelCount, CancellationToken cancellationToken = default)
    {
        var result = new List<(ChannelRecord, ChannelRangeRecord)>(channelCount);
        for (var i = 0; i < channelCount; i++)
        {
            var channel = await ReadStruct<ChannelRecord>(cancellationToken);

            var channelRange = channel.MinMaxFlag != 0 ? await ReadStruct<ChannelRangeRecord>(cancellationToken) : ChannelRangeRecord.NoValue;
            result.Add((channel, channelRange));
        }
        return result;
    }

    private Task<uint> ReadDatapointsAsync(CancellationToken cancellationToken = default)
        => ReadStruct<uint>(cancellationToken);

    private async Task<T> ReadStruct<T>(CancellationToken cancellationToken = default) where T : struct
    {
        cancellationToken.ThrowIfCancellationRequested();

        var size = Marshal.SizeOf<T>();
        var buffer = new byte[size];
        var pos = stream.Position;
        var bytesread = await stream.ReadAsync(buffer, 0, size, cancellationToken);
        return bytesread == size
            ? MemoryMarshal.Read<T>(buffer)
            : throw new MalformedRecordException(typeof(T), pos, buffer.Length, bytesread);
    }
}
