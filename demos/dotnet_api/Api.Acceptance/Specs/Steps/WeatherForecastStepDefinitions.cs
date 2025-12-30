using System;
using AcceptanceTesting.Api.Acceptance.Engine.WeatherForecast.Actors;
using Reqnroll;

namespace AcceptanceTesting.Api.Acceptance.Specs.Steps
{
    [Binding]
    public class WeatherForecastStepDefinitions
    {
        private readonly WeatherForecastsActor actor;

        public WeatherForecastStepDefinitions(WeatherForecastsActor actor)
        {
            this.actor = actor;
        }
        [Given("I have defined some weather forecasts")]
        public void GivenIHaveDefinedSomeWeatherForecasts()
        {
        }

        [When("I make a request to the weatherforecast api")]
        public Task WhenIMakeARequestToTheWeatherforecastApi()
        {
            return actor.RequestWeatherForecasts();
        }

        [Then("I should see a {int} OK response")]
        public void ThenIShouldSeeAOKResponse(int p0)
        {
        }
    }
}
