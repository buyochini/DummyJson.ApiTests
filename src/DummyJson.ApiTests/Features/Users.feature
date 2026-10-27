@users
Feature: Users
  As a test analyst
  I want to filter and create users via DummyJSON
  So that user resource behaviour can be validated

  Scenario: Verify and validate how many users have red hair
    When I filter users by hair colour "Red"
    Then the users filter response status code should be 200
    And the users filter total should be 25
    And the users filter result count should equal the reported total
    And every returned user should have hair colour "Red"

  Scenario: Add a new user with required personal and address details
    When I add a new user with the following details:
      | Field             | Value              |
      | FirstName         | Jane               |
      | LastName          | Doe                |
      | Age               | 28                 |
      | Gender            | female             |
      | HairColour        | Brown              |
      | HairType          | Straight           |
      | Address           | 1745 T Street Southeast |
      | City              | Washington         |
      | State             | DC                 |
      | StateCode         | DC                 |
      | PostalCode        | 20020              |
      | Latitude          | 38.867033          |
      | Longitude         | -76.979235         |
      | Country           | United States      |
    Then the add user response status code should be 201
    And the add user response should contain a generated id
    And the add user response should echo the submitted user details
