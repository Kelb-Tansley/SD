Feature: Bending

@Bending
Scenario: Determine the I or H bending capacity of beam 1
	Given the bending test file name is Column Bending Test.st7
	And the mathcad bending test file name is AS4100 I Sections.mcdx
	When the bending analysis is run
	And the mathcad bending file inputs are populated and run for beam 1
	Then the bending capacity should match the mathcad output

@Bending
Scenario: Determine the RHS bending capacity of beam 1
	Given the bending test file name is RHS Bending Test.st7
	And the mathcad bending test file name is AS4100 RH Sections.mcdx
	When the bending analysis is run
	And the mathcad bending file inputs are populated and run for beam 1
	Then the bending capacity should match the mathcad output