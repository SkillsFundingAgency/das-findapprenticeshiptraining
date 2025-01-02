# Feature: CourseDetail
# 	As an employer
# 	I want to know if the start date has passed for a bookmarked course
# 	So that I can know it is no longer available
	
# @WireMockServer
# Scenario: Course detail page
# 	Given I navigate to the following url: /courses/14
# 	Then an http status code of 200 is returned
# 	And the page content includes the following: Apprenticeship training course
# 	And the page content does not include the following: This apprenticeship training course is no longer available for new starts
# 	And the page content does not include the following: is available for new starts until
# 	And the page content does not include the following: needs a training provider who is approved by the appropriate regulatory body

# @WireMockServer
# Scenario: Course with last start date
# 	Given I navigate to the following url: /courses/24 
# 	Then an http status code of 200 is returned
# 	And the page content includes the following: is available for new starts until
	
# @WireMockServer	
# Scenario: Expired course
# 	Given I navigate to the following url: /courses/101
# 	Then an http status code of 200 is returned
# 	And the page content includes the following: This apprenticeship training course is no longer available for new apprentices.

# @WireMockServer
# Scenario: Regulated course
# 	Given I navigate to the following url: /courses/333
# 	Then an http status code of 200 is returned
# 	And the page content includes the following: needs a training provider who is approved by test