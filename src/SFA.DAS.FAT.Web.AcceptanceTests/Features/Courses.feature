
# Feature: Courses
#   As a FAT user
#   I want to be able to search for courses
#   So that I can find the course I want

#   @WireMockServer  
#   Scenario: Show all courses
#     Given I navigate to the following url: /courses
#     Then an http status code of 200 is returned
#     And the page content includes the following: Apprenticeship training courses
#     And there is a row for each course

#   @WireMockServer
#   Scenario: Show levels and sectors
#     Given I navigate to the following url: /courses
#     Then the sectors and levels are available to filter on
    
#   @MockApiClient
#   Scenario: Filter by keyword
#   Given I navigate to the following url: /courses?keyword=baker  
#   Then the Api is called with the keyword baker  