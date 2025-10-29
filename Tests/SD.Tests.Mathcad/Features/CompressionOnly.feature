Feature: CompressionOnly

@CompressionOnly
Scenario: Determine the compression utilization of beam 1
	Given the compression test file name is Columns Resistance Tests.st7
	And the mathcad compression test file name is I or H.mcdx
	When the compressive analysis is run
	And the mathcad compression file inputs are populated and run for beam 1
	Then the compressive resistance should match the mathcad output