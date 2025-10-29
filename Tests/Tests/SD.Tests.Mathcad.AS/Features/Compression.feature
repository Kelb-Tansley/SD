Feature: Compression

@Compression
Scenario: Determine the I or H compression capacity of beam 1
	Given the compression test file name is Column Compression Test.st7
	And the mathcad compression test file name is AS4100 I Sections.mcdx
	When the compression analysis is run
	And the mathcad compression file inputs are populated and run for beam 1
	Then the compression resistance should match the mathcad output

@Compression
Scenario: Determine the RHS compression capacity of beam 1
	Given the compression test file name is RHS Compression Test.st7
	And the mathcad compression test file name is AS4100 RH Sections.mcdx
	When the compression analysis is run
	And the mathcad compression file inputs are populated and run for beam 1
	Then the compression resistance should match the mathcad output