Feature: SingleBeam

@BeamColumn @PFC
Scenario: Determine the ULS utilization of beam 1 PFC section
	Given the Strand7 single beam file name is PFC.st7
	When the beam design is run
	And the Mathcad test file is found for beam 1
	And the Mathcad file inputs are populated and run
	Then the beam resistance should match the mathcad output

@BeamColumn @IorH
Scenario: Determine the ULS utilization of beam 1 I section
	Given the Strand7 single beam file name is I or H.st7
	When the beam design is run
	And the Mathcad test file is found for beam 1
	And the Mathcad file inputs are populated and run
	Then the beam resistance should match the mathcad output