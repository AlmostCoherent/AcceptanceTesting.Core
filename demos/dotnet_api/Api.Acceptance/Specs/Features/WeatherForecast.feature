Feature: Weather forecast api tests

This is a very basic test that just checks the functionality of the weatherforecast api

Scenario: Retrieve weather forecasts successfully
	Given I have defined some weather forecasts
	When I make a request to the weatherforecast api
	Then I should see a 200 OK response
