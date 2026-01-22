using NorthStandard.Testing.Demos.Api.Acceptance.Engine.WeatherForecast.Contexts;
using NorthStandard.Testing.ScreenPlayFramework.Domain.Abstractions;
using NorthStandard.Testing.ScreenPlayFramework.Infrastructure.Api.Actors;

namespace NorthStandard.Testing.Demos.Api.Acceptance.Engine.WeatherForecast.Actors
{
    public class WeatherForecastsActor : IActor
    {
        private readonly ApiActor apiActor;
        private readonly WeatherForcastContext context;

        public WeatherForecastsActor(ApiActor apiActor, WeatherForcastContext context)
        {
            this.apiActor = apiActor;
            this.context = context;
        }

        public void SetupWeatherForecasts()
        {
            /// This needs to setup the data. Ut should directly create a known 
            /// format in a db/table storage so that the subsequent request can 
            /// ask for this data.
            Console.WriteLine("Setting up weather forecasts...");
        }


        public async Task RequestWeatherForecasts()
        {
            var result = await apiActor.GetAsync<List<Api.WeatherForecast>>("weather-forecasts");
            context.RequestedForecasts = result;
        }
    }
}
