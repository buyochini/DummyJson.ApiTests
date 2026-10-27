@products
Feature: Products
  As a test analyst
  I want to search, paginate and update products via DummyJSON
  So that product resource behaviour can be validated

  Scenario: Verify and validate the price of iPhone 13 Pro
    When I search products for "iPhone 13 Pro"
    Then the products search response status code should be 200
    And the product titled "iPhone 13 Pro" should have price 1099.99

  Scenario: Retrieve the 15th product using pagination
    When I retrieve products with limit 1 and skip 14 selecting "id,title,price"
    Then the products pagination response status code should be 200
    And the products pagination response should contain exactly 1 product
    And the products pagination skip should be 14
    And the products pagination limit should be 1
    And the paginated product id should be 15
    And the paginated product title should be "Wooden Bathroom Sink With Mirror"
    And the paginated product price should be 799.99

  Scenario: Update the title of product id 121
    When I update product 121 title to "STA Coding Challenge Updated Title"
    Then the product update response status code should be 200
    And the updated product id should be 121
    And the updated product title should be "STA Coding Challenge Updated Title"
