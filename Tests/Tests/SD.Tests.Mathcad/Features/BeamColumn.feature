Feature: BeamColumn

@BeamColumn
Scenario: Determine the beam column ULS utilization of beam 5
	Given the Strand7 test file name is BeamColumnSimpleTests.st7
	And the Mathcad test file name is PFC.mcdx
	When the beam column analysis is run
	And the Mathcad file inputs are populated and run for beam 5
	Then the beam column resistance should match the mathcad output