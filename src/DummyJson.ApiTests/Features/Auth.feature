@auth
Feature: Authentication
  As a test analyst
  I want to authenticate against DummyJSON and inspect the current user
  So that protected API operations can be validated

  Scenario: Authenticate user and store access token
    When I authenticate with the configured credentials
    Then the authentication response status code should be 200
    And the authentication response should contain a non-empty access token
    And the authentication response should contain a non-empty refresh token
    And the access token should be stored for subsequent requests

  Scenario: Validate id, first name and last name of the current authenticated user
    Given I am authenticated with the configured credentials
    When I request the current authenticated user
    Then the current user response status code should be 200
    And the current user id should be 1
    And the current user first name should be "Emily"
    And the current user last name should be "Johnson"
