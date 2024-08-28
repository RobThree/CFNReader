namespace CFNReader.Tests;

[TestClass]
public class CFNReaderTests
{
    [TestMethod]
    public async Task CFNReader_ReadsCorrectly()
    {
        using var testfile = File.OpenRead("Testfiles/test.cfn");
        var cfnreader = new CFNStreamReader(testfile);
        var fileinfo = await cfnreader.ReadFileInfoAsync();

        Assert.AreEqual(new UnitValue(100, Unit.Hz), fileinfo.SampleRate);
        Assert.AreEqual(new UnitValue(0.05, Unit.A), fileinfo.StartCurrentCondition);
        Assert.AreEqual(new UnitValue(0, Unit.A), fileinfo.StopCurrentCondition);
        Assert.AreEqual(TimeSpan.FromSeconds(5), fileinfo.StopTimeCondition);
        Assert.AreEqual(6768u, fileinfo.Datapoints);

        Assert.AreEqual(4, fileinfo.Channels.Count);

        Assert.IsTrue(fileinfo.Channels.Select(c => c.Key).SequenceEqual([Channel.VBusVoltage, Channel.VBusCurrent, Channel.AccumulatedCapacity, Channel.AccumulatedEnergy]));

        Assert.AreEqual(new UnitValue(5.16831, Unit.V), fileinfo.Channels[Channel.VBusVoltage].Max);
        Assert.AreEqual(new UnitValue(4.51257, Unit.V), fileinfo.Channels[Channel.VBusVoltage].Min);
        Assert.AreEqual(0, fileinfo.Channels[Channel.VBusVoltage].Index);

        Assert.AreEqual(new UnitValue(0.8599, Unit.A), fileinfo.Channels[Channel.VBusCurrent].Max);
        Assert.AreEqual(new UnitValue(0, Unit.A), fileinfo.Channels[Channel.VBusCurrent].Min);
        Assert.AreEqual(1, fileinfo.Channels[Channel.VBusCurrent].Index);

        Assert.AreEqual(new UnitValue(double.NaN, Unit.Ah), fileinfo.Channels[Channel.AccumulatedCapacity].Max);
        Assert.AreEqual(new UnitValue(double.NaN, Unit.Ah), fileinfo.Channels[Channel.AccumulatedCapacity].Min);
        Assert.AreEqual(2, fileinfo.Channels[Channel.AccumulatedCapacity].Index);

        Assert.AreEqual(new UnitValue(double.NaN, Unit.Wh), fileinfo.Channels[Channel.AccumulatedEnergy].Max);
        Assert.AreEqual(new UnitValue(double.NaN, Unit.Wh), fileinfo.Channels[Channel.AccumulatedEnergy].Min);
        Assert.AreEqual(3, fileinfo.Channels[Channel.AccumulatedEnergy].Index);

        var datapoints = await cfnreader.ReadDatapointsAsync(fileinfo).ToArrayAsync();

        Assert.AreEqual(6768, datapoints.Length);

        Assert.AreEqual(228995.28, datapoints.Select(d => d.Values[Channel.VBusVoltage].Value).Sum());
        Assert.AreEqual(34353.95317999959, datapoints.Select(d => d.Values[Channel.VBusCurrent].Value).Sum());
        Assert.AreEqual(842.6989099999994, datapoints.Select(d => d.Values[Channel.AccumulatedCapacity].Value).Sum());
        Assert.AreEqual(8.23502898181939, datapoints.Select(d => d.Values[Channel.AccumulatedEnergy].Value).Sum());
    }
}