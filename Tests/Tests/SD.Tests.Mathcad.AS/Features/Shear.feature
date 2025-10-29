Feature: Shear

@Shear
Scenario: Determine the I or H shear capacity of beam 1
	Given the shear test file name is Column Shear Test.st7
	And the mathcad shear test file name is AS4100 I Sections.mcdx
	When the shear analysis is run
	And the mathcad shear file inputs are populated and run for beam 1
	Then the shear resistance should match the mathcad output

@Shear
Scenario: Determine the RHS shear capacity of beam 1
	Given the shear test file name is RHS Shear Test.st7
	And the mathcad shear test file name is AS4100 RH Sections.mcdx
	When the shear analysis is run
	And the mathcad shear file inputs are populated and run for beam 1
	Then the shear resistance should match the mathcad output