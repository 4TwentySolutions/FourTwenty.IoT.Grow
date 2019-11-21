namespace GrowIoT.Enums
{
    public enum ModuleType
    {
        Humidity = 1,
        Temperature = 2,
        HumidityAndTemperature = 3,
        Relay = 4,
        TwoRelay = 5,
        Fan = 6,
        Light = 7,
        WaterPump = 8
    }

    public enum JobType
    {
        Read = 1,
        Toggle = 2,
        On = 3,
        Off = 4,
        Period = 5
    }
}
