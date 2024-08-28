namespace CFNReader.Tests;

[TestClass]
public class CFNCSVConverterTests
{
    [TestMethod]
    public async Task CFNCSVConverter_Converts_Correctly()
    {
        var fromtime = TimeSpan.FromSeconds(10.5);
        var csvconverter = new CFNCSVConverter(
            channels: [Channel.AccumulatedCapacity, Channel.VBusVoltage],
            predicate: (dp) => dp.Time >= fromtime,
            separator: "!",
            includeUnit: true,
            limitDataPoints: 2
        );

        using var testfile = File.OpenRead("Testfiles/test.cfn");
        using var csvfile = new MemoryStream();

        await csvconverter.ConvertToCSVAsync(testfile, csvfile);

        csvfile.Position = 0;
        var reader = new StreamReader(csvfile);
        var csv = await reader.ReadToEndAsync();

        Assert.AreEqual(
            $"Time!AccumulatedCapacity!VBusVoltage{Environment.NewLine}0:00:00:10.5000000!0.1593Ah!10.5000V{Environment.NewLine}0:00:00:10.5100000!0.1412Ah!10.5100V{Environment.NewLine}",
            csv
        );
    }
}