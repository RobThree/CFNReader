# ![Logo](https://raw.githubusercontent.com/RobThree/CFNReader/master/icon.png) CFNReader

# Introduction

Provides a simple way to read FNIRSI's CFN files (`*.cfn`) produced by the FNIRSI UsbMeter tool ([here](https://www.fnirsi.com/pages/download-firmware)). With this library you can also convert `*.cfn` files to CSV files easily. Available as [NuGet package](https://www.nuget.org/packages/CFNReader).

# QuickStart

```c#
using var myfile = File.OpenRead("myfile.cfn");
var cfnreader = new CFNStreamReader(myfile);

// Read general file information
var fileinfo = await cfnreader.ReadFileInfoAsync();

// Read and print data points
await foreach (var datapoint in cfnreader.ReadDatapointsAsync(fileinfo))
{
    Console.WriteLine(
        string.Join("\t", [datapoint.Time.ToString("G"), datapoint.Values[Channel.VBusVoltage], datapoint.Values[Channel.VBusCurrent]])
    );
}
```

Output:

```cmd
0:00:00:00,0000000      0.0000V 0.0126A
0:00:00:00,0100000      0.0100V 0.0126A
0:00:00:00,0200000      0.0200V 0.0126A
0:00:00:00,0300000      0.0300V 0.0126A
0:00:00:00,0400000      0.0400V 0.0126A
...
```

The `CFNStreamReader` class provides two methods to read the file: `ReadFileInfoAsync` and `ReadDatapointsAsync`. The first one reads the file header and returns a `FileInfo` object. The second one reads the data points and returns an `IAsyncEnumerable<DataPoint>`. Both methods are asynchronous and support cancellation using a `CancellationToken`.

# Convert to CSV

A simple way to convert the data to a CSV is provided by the `CFNCSVConverter`:

```c#
using var myfile = File.OpenRead("myfile.cfn");
using var csvfile = File.Create("export.csv");

var csvconverter = new CFNCSVConverter(
    channels: [Channel.AccumulatedCapacity, Channel.VBusVoltage],
    predicate: dp => dp.Values[Channel.VBusVoltage] > 0.5,
    includeUnit: true,
    limitDataPoints: 1000
);

await csvconverter.ConvertToCSVAsync(testfile, csvfile);
```

The `CFNCSVConverter` class constructor accepts the following (optional) parameters:

- `channels`: An array of `Channel` values to include in the CSV. The order of the channels in the array will be the order of the columns in the CSV.
- `predicate`: A function that receives a `Datapoint` and returns a boolean. If the function returns `true`, the `Datapoint` will be included in the CSV.
- `formatProvider`: An `IFormatProvider` to use when formatting the values.
- `separator`: The separator to use between the values (default `;`).
- `encoding`: The encoding to use when writing the CSV file (default `UTF8`).
- `timeFormat`: The format to use when formatting the time values (default `G`).
- `valueFormat`: The format to use when formatting the values (default `N4`).
- `includeHeader`: Wether to include the header row in the CSV.
- `includeTime`: Wether to include the time column (always first column).
- `includeUnit`: Wether to include the unit of the value(s) like `A` (amps) or `V` (volts) for example.
- `limitDataPoints`: The maximum number of data points to include in the CSV (default `int.MaxValue`)

# Acknowledgements

This library is based on [didim99](https://github.com/didim99)'s work in [usbmeter-utils](https://github.com/didim99/usbmeter-utils/tree/master).