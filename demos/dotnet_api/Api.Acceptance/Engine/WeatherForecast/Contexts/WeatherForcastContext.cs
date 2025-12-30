using NorthStandard.Testing.ScreenPlayFramework.Domain.Abstractions;

namespace AcceptanceTesting.Api.Acceptance.Engine.WeatherForecast.Contexts
{
    public class WeatherForcastContext : IContext
    {
        public List<Api.WeatherForecast> RequestedForecasts = new List<Api.WeatherForecast>();
    }
}
