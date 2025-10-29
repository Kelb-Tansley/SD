Feature: DeflectionLimit

@SpanDeflectionRatio @AbsoluteMethod @YAxis
Scenario: Determine the absolute span to deflection ratio of beam 1
	Given the deflection test file name is Deflection Limit Test.st7
	And the deflection axis is Y
	And the deflection method is Absolute
	When the deflection analysis is run
	Then the span to deflection ratio of beam 1 should be 89.3265
	
@SpanDeflectionRatio @RelativeMethod @YAxis
Scenario: Determine the relative span to deflection ratio of beam 7
	Given the deflection test file name is Deflection Limit Test.st7
	And the deflection axis is Y
	And the deflection method is Relative
	When the deflection analysis is run
	Then the span to deflection ratio of beam 7 should be 857.696

@SpanDeflectionRatio @AbsoluteMethod @YAxis
Scenario: Determine the absolute span to deflection ratio of beam 4
	Given the deflection test file name is Deflection Limit Test.st7
	And the deflection axis is Y
	And the deflection method is Absolute
	When the deflection analysis is run
	Then the span to deflection ratio of beam 4 should be 553.7762
	
@SpanDeflectionRatio @AbsoluteMethod @YAxis
Scenario: Determine the absolute span to deflection ratio of beam 7
	Given the deflection test file name is Deflection Limit Test.st7
	And the deflection axis is Y
	And the deflection method is Absolute
	When the deflection analysis is run
	Then the span to deflection ratio of beam 7 should be 153.6