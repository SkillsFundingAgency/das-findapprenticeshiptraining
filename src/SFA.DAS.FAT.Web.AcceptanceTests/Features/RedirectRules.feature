Feature: FAT v1 Redirect Rules
  As an existing FAT user
  I to make sure any I can access new pages through old urls
  So that I am not presented with a page not found

  @WireMockServer
  Scenario: Navigate from v1 apprenticeship or provider page
    Given I navigate to the following url: /ApprenticeshipOrProvider
    Then an http status code of 200 is returned
    And the page content includes the following: Find apprenticeship training for your apprentice

  @WireMockServer
  Scenario: Navigate from v1 apprenticeship apprenticeship or provider page
    Given I navigate to the following url: /Apprenticeship/ApprenticeshipOrProvider
    Then an http status code of 200 is returned
    And the page content includes the following: Find apprenticeship training for your apprentice
  
  @WireMockServer
  Scenario: Navigate from v1 provider search
    Given I navigate to the following url: /Provider/search
    Then an http status code of 200 is returned
    And the page content includes the following: Find apprenticeship training for your apprentice
  
  @WireMockServer
  Scenario: Navigate from v1 provider search results
    Given I navigate to the following url: /Provider/searchResults
    Then an http status code of 200 is returned
    And the page content includes the following: Find apprenticeship training for your apprentice

  @WireMockServer
  Scenario: Navigate from v1 provider overview
    Given I navigate to the following url: /Provider/10001234
    Then an http status code of 200 is returned
    And the page content includes the following: Find apprenticeship training for your apprentice
    
  @WireMockServer
  Scenario: Navigate from v1 provider framework results
    Given I navigate to the following url: /Provider/FrameworkResults
    Then an http status code of 200 is returned
    And the page content includes the following: Apprenticeship training courses  
  
  @WireMockServer
  Scenario: Navigate from v1 search page
    Given I navigate to the following url: /Apprenticeship/search
    Then an http status code of 200 is returned
    And the page content includes the following: Apprenticeship training courses

  @WireMockServer
  Scenario: Navigate from v1 search page results
    Given I navigate to the following url: /apprenticeship/SearchResults?keywords=baker&SelectedLevels=2&SelectedLevels=6
    Then an http status code of 200 is returned
    And the page content includes the following: &lsquo;baker&rsquo;
    And the page content includes the following: Clear this level filter Level 2
    And the page content includes the following: Clear this level filter Level 6
    
  @WireMockServer
  Scenario: Navigate from v1 search page results from frameworks
    Given I navigate to the following url: /apprenticeship/Framework/123-1-4
    Then an http status code of 200 is returned
    And the page content includes the following: Apprenticeship training courses

  @WireMockServer
  Scenario: Navigate from v1 course detail page
    Given I navigate to the following url: /apprenticeship/Standard/333
    Then an http status code of 200 is returned
    And the page content includes the following: needs a training provider who is approved by test

  @WireMockServer
  Scenario: Navigate from v1 provider standards result page
    Given I navigate to the following url: /Provider/StandardResults?apprenticeshipid=333&postcode=london
    Then an http status code of 200 is returned
    And the page content includes the following: Training providers for
    And there is a row for each course provider
    And the delivery modes for each provider are displayed
  
  @WireMockServer
  Scenario: Navigate from v1 provider standard detail page
    Given I navigate to the following url: /Provider/Detail?ukprn=1001&standardCode=2&Postcode=coventry
    Then an http status code of 200 is returned
    And the page content includes the following: This training provider does not offer Software developer (level 4) at the apprenticeship location
    And the page content includes the following: There are 4 training providers for
    