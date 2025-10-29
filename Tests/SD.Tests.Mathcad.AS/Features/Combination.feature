Feature: Combination

@Combination
Scenario: Determine the I or H combination capacity of beam 1
	Given the combination test file name is Column Combined Test.st7
	And the mathcad combination test file name is AS4100 I Sections.mcdx
	When the combination analysis is run
	And the mathcad combination file inputs are populated and run for beam 1
	Then the combination capacity should match the mathcad output

@Combination
Scenario: Determine the RHS combination capacity of beam 1
	Given the combination test file name is RHS Combined Test.st7
	And the mathcad combination test file name is AS4100 RH Sections.mcdx
	When the combination analysis is run
	And the mathcad combination file inputs are populated and run for beam 1
	Then the combination capacity should match the mathcad output