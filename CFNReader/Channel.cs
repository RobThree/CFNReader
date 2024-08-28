namespace CFNReader;

public enum Channel : ushort
{
    VBusVoltage = 0x00,
    VBusCurrent = 0x01,
    DPlusVoltage = 0x02,
    DMinusVoltage = 0x03,
    Power = 0x04,
    AccumulatedCapacity = 0x05,
    AccumulatedEnergy = 0x06
}