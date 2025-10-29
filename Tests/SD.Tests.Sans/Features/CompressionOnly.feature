Feature: CompressionOnly

@CompressionOnly
Scenario: Determine the compression utilization of beam 1
	Given the compression test file name is Columns Resistance Tests.st7
	When the compressive analysis is run
	Then the compressive resistance of beam 1 should be 623.72