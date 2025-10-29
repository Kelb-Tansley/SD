Feature: ElementLength

@EffectiveLength2 @Axis2 @Beam1
Scenario: Determine the effective length 2 of beam 1 in axis 2
	Given the fem test file name is Element Length Tests.st7
	When SANS ULS design is run
	Then the L2 chain length of beam 1 should be 14366.6

@EffectiveLength1 @Axis2 @Beam1
Scenario: Determine the effective length 1 of beam 1 in axis 2
	Given the fem test file name is Element Length Tests.st7
	When SANS ULS design is run
	Then the L1 chain length of beam 1 should be 16733.2

@EffectiveLength2 @Axis2 @Beam3
Scenario: Determine the effective length 2 of beam 3 in axis 2
	Given the fem test file name is Element Length Tests.st7
	When SANS ULS design is run
	Then the L2 chain length of beam 3 should be 3000

@EffectiveLength2 @Axis1 @Beam3
Scenario: Determine the effective length 2 of beam 3 in axis 1
	Given the fem test file name is Element Length Tests.st7
	When SANS ULS design is run
	Then the L2 chain length of beam 3 should be 6000

@EffectiveLength1 @Axis3 @Beam2
Scenario: Determine the effective length 1 of beam 2 in axis 3
	Given the fem test file name is Element Length Tests.st7
	When SANS ULS design is run
	Then the L1 chain length of beam 2 should be 9000

@EffectiveLength1 @Axis3 @Beam4
Scenario: Determine the effective length 1 of beam 4 in axis 3
	Given the fem test file name is Element Length Tests.st7
	When SANS ULS design is run
	Then the L1 chain length of beam 4 should be 9000

@EffectiveLength2 @Axis3 @Beam4
Scenario: Determine the effective length 2 of beam 4 in axis 3
	Given the fem test file name is Element Length Tests.st7
	When SANS ULS design is run
	Then the L2 chain length of beam 4 should be 14366.6
	
@EffectiveLength1 @Axis1 @Beam2
Scenario: Determine the effective length 1 of beam 2 in axis 1
	Given the fem test file name is Element Length Tests.st7
	When SANS ULS design is run
	Then the L1 chain length of beam 2 should be 9000

@EffectiveLength1 @Axis1 @Beam3
Scenario: Determine the effective length 1 of beam 3 in axis 1 with beam restraints
	Given the fem test file name is Element Length Restraint Tests.st7
	When SANS ULS design is run
	Then the L1 chain length of beam 3 should be 1000

@EffectiveLength1 @Axis1 @Beam1
Scenario: Determine the effective length 1 of beam 1 in axis 1 with beam restraints
	Given the fem test file name is Element Length Restraint Tests.st7
	When SANS ULS design is run
	Then the L1 chain length of beam 1 should be 2000

@EffectiveLength1 @Axis3 @Beam4
Scenario: Determine the effective length 1 of beam 4 in axis 3 with beam restraints
	Given the fem test file name is Element Length Restraint Tests.st7
	When SANS ULS design is run
	Then the L1 chain length of beam 4 should be 2000

@EffectiveLength2 @Axis3 @Beam2
Scenario: Determine the effective length 2 of beam 2 in axis 3 with beam restraints
	Given the fem test file name is Element Length Restraint Tests.st7
	When SANS ULS design is run
	Then the L2 chain length of beam 2 should be 2000

@EffectiveLength2 @Axis2 @Beam10
Scenario: Determine the effective length 2 of beam 10 in axis 2 with beam restraints
	Given the fem test file name is Element Length Restraint Tests.st7
	When SANS ULS design is run
	Then the L2 chain length of beam 10 should be 2000