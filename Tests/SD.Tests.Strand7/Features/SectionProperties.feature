Feature: SectionProperties

@SectionProperties @RedBookComparison
Scenario: The section properties of property number 2 should match
	Given the fem test file name is Section Properties.st7
	When the section properties for property number 2 and 12 are loaded with accuracy 1%
	Then the section property Agr should be 15.6
	And the section property IMajor should be 762
	And the section property ZeMajor should be 2800
	And the section property ZplMajor should be 3200
	And the section property RMajor should be 221
	And the section property IMinor should be 33.9
	And the section property ZeMinor should be 320
	And the section property ZplMinor should be 500
	And the section property RMinor should be 46.6
	And the section property J should be 1810
	And the section property Cw should be 2320