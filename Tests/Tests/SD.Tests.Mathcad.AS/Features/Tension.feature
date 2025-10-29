Feature: Tension

@Tension
Scenario: Determine the I or H tension capacity of beam 1
	Given the tension test file name is Column Tension Test.st7
	And the mathcad tension test file name is AS4100 I Sections.mcdx
	When the tensile analysis is run
	And the mathcad tension file inputs are populated and run for beam 1
	Then the tensile resistance should match the mathcad output

@Tension
Scenario: Determine the RHS tension capacity of beam 1
	Given the tension test file name is RHS Tension Test.st7
	And the mathcad tension test file name is AS4100 RH Sections.mcdx
	When the tensile analysis is run
	And the mathcad tension file inputs are populated and run for beam 1
	Then the tensile resistance should match the mathcad output