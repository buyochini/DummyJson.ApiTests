@carts
Feature: Carts
  As a test analyst
  I want to create and delete carts via DummyJSON
  So that cart resource behaviour can be validated

  Scenario: Add 5 products to a cart and validate the total discounted percentage
    When I add a cart for user 1 with the following products:
      | ProductId | Quantity |
      | 1         | 1        |
      | 2         | 1        |
      | 3         | 1        |
      | 4         | 1        |
      | 5         | 1        |
    Then the add cart response status code should be 201
    And the cart should belong to user 1
    And the cart should contain 5 products
    And the cart should contain exactly the requested products and quantities
    And the sum of line-item discount percentages should be 62.11

  Scenario: Delete a cart
    When I delete cart 1
    Then the delete cart response status code should be 200
    And the deleted cart id should be 1
    And the deleted cart isDeleted flag should be true
    And the deleted cart should include a deletedOn timestamp
