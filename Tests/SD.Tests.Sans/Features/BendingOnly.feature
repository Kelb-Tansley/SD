Feature: BendingOnly

@CompressionOnly @Beam1
Scenario: Determine the bending utilization of beam 1
	Given the bending test file name is Beam Resistance Test.st7
	When the bending analysis is run
	Then the bending resistance of beam 1 should be 163.12160230202508

@CompressionOnly @Beam3
Scenario: Determine the bending utilization of beam 3
	Given the bending test file name is Beam Resistance Test.st7
	When the bending analysis is run
	Then the bending resistance of beam 3 should be 176.59141006499993

@CompressionOnly @Beam5
Scenario: Determine the bending utilization of beam 5
	Given the bending test file name is Beam Resistance Test.st7
	When the bending analysis is run
	Then the bending resistance of beam 5 should be 107.55192962744609

@CompressionOnly @Beam7
Scenario: Determine the bending utilization of beam 7
	Given the bending test file name is Beam Resistance Test.st7
	When the bending analysis is run
	Then the bending resistance of beam 7 should be 47.49316006992967