Feature: Weather forecast api tests

This is a very basic test that just checks the functionality of the web app.

Scenario: Confirm home page of web app
	When I navigate to the home page
	Then I should see the home page

Scenario: Confirm home page of web app again
	When I navigate to the home page
	Then I should see the home page
