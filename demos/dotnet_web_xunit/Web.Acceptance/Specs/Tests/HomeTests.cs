using NorthStandard.Testing.Demos.Web.Acceptance.Engine.Home.Actors;
using NorthStandard.Testing.Demos.Web.Acceptance.Engine.Home.Validators;
using NorthStandard.Testing.Playwright.XUnit.BDD;
using Xunit;

namespace NorthStandard.Testing.Demos.Web.Acceptance.Specs.Tests;

[Collection("WebServer")]
public class HomeTests
{
	private readonly WebServerFixture _fixture;
	private readonly HomePageActor _homePageActor;
	private readonly HomePageValidator _homePageValidator;

	public HomeTests(WebServerFixture fixture)
	{
		_fixture = fixture;
		_homePageActor = _fixture.Services.GetService(typeof(HomePageActor)) as HomePageActor 
			?? throw new InvalidOperationException("HomePageActor not registered");
		_homePageValidator = _fixture.Services.GetService(typeof(HomePageValidator)) as HomePageValidator 
			?? throw new InvalidOperationException("HomePageValidator not registered");
	}

	[Fact]
	[Trait("Category", "Home")]
	public async Task User_Can_View_Home_Page()
	{
		await Scenario.Create("User views the home page")
			.When("I navigate to the home page", async ctx =>
			{
				await _homePageActor.NavigateToHomePage();
			})
			.Then("I should see the home page", async ctx =>
			{
				await _homePageValidator.ValidateHomePageTitle();
			})
			.RunAsync();
	}

	[Fact]
	[Trait("Category", "Home")]
	[Trait("Priority", "High")]
	public async Task Home_Page_Displays_Welcome_Message()
	{
		await Scenario.Create("Home page displays welcome message")
			.When("I navigate to the home page", async ctx =>
			{
				await _homePageActor.NavigateToHomePage();
			})
			.Then("I should see the welcome message", async ctx =>
			{
				await _homePageValidator.ValidateHomePageTitle();
			})
			.RunAsync();
	}
}
