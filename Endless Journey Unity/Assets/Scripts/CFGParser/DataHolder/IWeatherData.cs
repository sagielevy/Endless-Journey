namespace Assets.Scripts.CFGParser.DataHolder
{
    public enum WeatherTypes
    {
        Hail = 0,
        Rain = 1,
        Snow = 2,
        Sandstorm = 3
    }

    public interface IWeatherData : ISentenceData
    {
        bool IsWeatherActive();
        bool IsWindActive();

        float GravityModifier();
        float WindMain();
        float WindTurbulence();
        float WindPulseMag();
        float WindPulseFreq();
        WeatherTypes WeatherType();
    }
}
