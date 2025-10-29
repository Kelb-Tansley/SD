Feature: SectionProperties

@SectionProperties @RedBookComparison
Scenario: The section properties of property number 1 should match
	Given the section properties test file name is Section Properties.st7
	When the section properties for property number 1 are loaded with accuracy 0.3%
	Then the section property Agr should be 13.4
	And the section property IMajor should be 1190
	And the section property ZeMajor should be 3310
	And the section property ZplMajor should be 3720
	And the section property RMajor should be 298
	And the section property IMinor should be 26.7
	And the section property ZeMinor should be 267
	And the section property RMinor should be 44.6
	And the section property J should be 1126.25
	And the section property Cw should be 3270
	#Red book value of 1140 for J may include the radius at flange to web connection

@SectionProperties @RedBookComparison
Scenario: The section properties of property number 2 should match
	Given the section properties test file name is Section Properties.st7
	When the section properties for property number 2 are loaded with accuracy 1%
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

@SectionProperties @RedBookComparison
Scenario: The section properties of property number 4 should match
	Given the section properties test file name is Section Properties.st7
	When the section properties for property number 4 are loaded with accuracy 2%
	Then the section property Agr should be 3.09
	And the section property AMajor should be 0
	And the section property AMinor should be 45.7
	And the section property CeMajor should be 100
	And the section property CeMinor should be 22.5
	And the section property IMajor should be 19.1
	And the section property ZeMajor should be 191
	And the section property ZplMajor should be 220.128
	And the section property RMajor should be 78.6
	And the section property IMinor should be 1.67
	And the section property ZeMinor should be 31.8
	And the section property ZplMinor should be 57.5496
	And the section property RMinor should be 23.2
	And the section property J should be 104
	And the section property Cw should be 10.6

@SectionProperties @RedBookComparison
Scenario: The section properties of property number 6 should match
	Given the section properties test file name is Section Properties.st7
	When the section properties for property number 6 are loaded with accuracy 4.5%
	Then the section property Agr should be 0.941
	And the section property CeMajor should be 25.2
	And the section property CeMinor should be 12.9
	And the section property V1 should be 21.8
	And the section property V2 should be 26.2
	And the section property U1 should be 50.8
	And the section property U2 should be 37.8
	And the section property Ixx should be 0.52
	And the section property Zexx should be 10.4
	And the section property Zplxx should be 18.978
	And the section property Rxx should be 23.5
	And the section property Iyy should be 0.184
	And the section property Zeyy should be 4.95
	And the section property Zplyy should be 9.22368
	And the section property Ryy should be 14
	And the section property IMajor should be 0.596
	And the section property ZeMajor should be 11.7
	And the section property RMajor should be 25.2
	And the section property IMinor should be 0.108
	And the section property ZeMinor should be 4.10
	And the section property RMinor should be 10.7
	And the section property J should be 21.3
	And the section property Cw should be 0

@SectionProperties @RedBookComparison
Scenario: The section properties of property number 7 should match
	Given the section properties test file name is Section Properties.st7
	When the section properties for property number 7 are loaded with accuracy 3%
	Then the section property Agr should be 2.27
	And the section property CeMajor should be 29
	And the section property CeMinor should be 29
	And the section property V1 should be 41.1
	And the section property V2 should be 35.7
	And the section property U1 should be 75
	And the section property U2 should be 75
	And the section property Ixx should be 2.07
	And the section property Zexx should be 29.1
	And the section property Zplxx should be 53.6122
	And the section property Rxx should be 30.2
	And the section property Iyy should be 2.07
	And the section property Zeyy should be 29.1
	And the section property Zplyy should be 53.6122
	And the section property Ryy should be 30.2
	And the section property IMajor should be 3.28
	And the section property ZeMajor should be 46.3
	And the section property RMajor should be 38
	And the section property IMinor should be 0.857
	And the section property ZeMinor should be 20.9
	And the section property RMinor should be 19.4
	And the section property J should be 118
	And the section property Cw should be 0

@SectionProperties
Scenario: The section properties of property number 8 should match
	Given the section properties test file name is Section Properties.st7
	When the section properties for property number 8 are loaded with accuracy 0.35%
	Then the section property Agr should be 8.36454
	And the section property CeMajor should be 130.5
	And the section property CeMinor should be 111.0003
	And the section property IMajor should be 37.5210
	And the section property ZeMajor should be 287.517
	And the section property RMajor should be 66.9755
	And the section property IMinor should be 8.86188
	And the section property ZeMinor should be 79.8365
	And the section property RMinor should be 32.5493
	And the section property ZplMinor should be 160.496
	And the section property ZplMajor should be 437.734
	And the section property J should be 1498.66515948385
	And the section property Cw should be 0
	#Red book value of 1620 for J may include the radius at flange to web connection
	
@SectionProperties
Scenario: The section properties of property number 9 should match
	Given the section properties test file name is Section Properties.st7
	When the section properties for property number 9 are loaded with accuracy 0.25%
	Then the section property Agr should be 6
	And the section property AMajor should be 0
	And the section property AMinor should be 0
	And the section property CeMajor should be 0
	And the section property CeMinor should be 0
	And the section property IMajor should be 86
	And the section property ZeMajor should be 538
	And the section property RMajor should be 120
	And the section property IMinor should be 41.9
	And the section property ZeMinor should be 419
	And the section property RMinor should be 83.5
	And the section property J should be 88.9
	And the section property Cw should be 0
	And the section property ZplMinor should be 478.512
	And the section property ZplMajor should be 661.392
	#Red book value of 469 for ZplMinor is incorrect
	#Red book value of 647 for ZplMajor is incorrect
	
@SectionProperties
Scenario: The section properties of property number 10 should match
	Given the section properties test file name is Section Properties.st7
	When the section properties for property number 10 are loaded with accuracy 0.2%
	Then the section property Agr should be 6.57
	And the section property AMajor should be 0
	And the section property AMinor should be 0
	And the section property CeMajor should be 0
	And the section property CeMinor should be 0
	And the section property IMajor should be 36
	And the section property ZeMajor should be 328
	And the section property ZplMajor should be 438
	And the section property RMajor should be 74
	And the section property IMinor should be 36
	And the section property ZeMinor should be 328
	And the section property ZplMinor should be 438
	And the section property RMinor should be 74
	And the section property J should be 72
	And the section property Cw should be 0

@SectionProperties
Scenario: The section properties of property number 11 should match
	Given the section properties test file name is Section Properties.st7
	When the section properties for property number 11 are loaded with accuracy 0.35%
	Then the section property Agr should be 1.53536
	And the section property CeMajor should be 107.0884
	And the section property CeMinor should be 50.8
	And the section property IMajor should be 3.70097
	And the section property ZeMajor should be 34.5599
	And the section property RMajor should be 49.0967
	And the section property IMinor should be 0.596672
	And the section property ZeMinor should be 11.7455
	And the section property RMinor should be 19.71343
	And the section property ZplMinor should be 18.7728
	And the section property ZplMajor should be 62.8102
	And the section property J should be 19.977
	And the section property Cw should be 0
	#Red book value of 24.1 for J may include the radius at flange to web connection