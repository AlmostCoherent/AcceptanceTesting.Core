using NorthStandard.Testing.Demos.Web.Acceptance.Engine.Home.Actors;
using NorthStandard.Testing.Demos.Web.Acceptance.Engine.Home.Validators;
using Reqnroll;

namespace NorthStandard.Testing.Demos.Web.Acceptance.Specs.Steps
{
	[Binding]
	public class HomeSteps(HomePageActor homePageActor, HomePageValidator homePageValidator)
    {
		[When(@"I navigate to the home page")]
		public async Task GivenINavigateToTheHomePage()
		{
			await homePageActor.NavigateToHomePage();
		}

		[Then("I should see the home page")]
		public async Task ThenIShouldSeeTheHomePage()
		{
			await homePageValidator.ValidateHomePageTitle();
		}

	}
}
