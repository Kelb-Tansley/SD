Feature: AppliedLoad

@AppliedLoad @ShearForce
Scenario: Determine the max minor shear force on beam 1
	Given the applied load test file name is Applied loads.st7
	When the uls analysis is run
	Then the max result type: ShearForceMinor of beam 1 should be 1750000
	
@AppliedLoad @ShearForce
Scenario: Determine the min minor shear force on beam 1
	Given the applied load test file name is Applied loads.st7
	When the uls analysis is run
	Then the min result type: ShearForceMinor of beam 1 should be 250000
	
@AppliedLoad @ShearForce
Scenario: Determine the max minor shear force on beam 2
	Given the applied load test file name is Applied loads.st7
	When the uls analysis is run
	Then the max result type: ShearForceMinor of beam 2 should be 250000
		
@AppliedLoad @ShearForce
Scenario: Determine the min minor shear force on beam 2
	Given the applied load test file name is Applied loads.st7
	When the uls analysis is run
	Then the min result type: ShearForceMinor of beam 2 should be -500000

@AppliedLoad @ShearForce
Scenario: Determine the max major shear force on beam 1
	Given the applied load test file name is Applied loads.st7
	When the uls analysis is run
	Then the max result type: ShearForceMajor of beam 1 should be 7500000
	
@AppliedLoad @ShearForce
Scenario: Determine the min major shear force on beam 1
	Given the applied load test file name is Applied loads.st7
	When the uls analysis is run
	Then the min result type: ShearForceMajor of beam 1 should be -500000
	
@AppliedLoad @ShearForce
Scenario: Determine the max major shear force on beam 2
	Given the applied load test file name is Applied loads.st7
	When the uls analysis is run
	Then the max result type: ShearForceMajor of beam 2 should be -500000
		
@AppliedLoad @ShearForce
Scenario: Determine the min major shear force on beam 2
	Given the applied load test file name is Applied loads.st7
	When the uls analysis is run
	Then the min result type: ShearForceMajor of beam 2 should be -3500000

@AppliedLoad @BendingMoment
Scenario: Determine the max minor bending moment on beam 1
	Given the applied load test file name is Applied loads.st7
	When the uls analysis is run
	Then the max result type: BendingMomentMinor of beam 1 should be 0
	
@AppliedLoad @BendingMoment
Scenario: Determine the min minor bending moment on beam 1
	Given the applied load test file name is Applied loads.st7
	When the uls analysis is run
	Then the min result type: BendingMomentMinor of beam 1 should be -3000000000
	
@AppliedLoad @BendingMoment
Scenario: Determine the max major bending moment on beam 1
	Given the applied load test file name is Applied loads.st7
	When the uls analysis is run
	Then the max result type: BendingMomentMajor of beam 1 should be 0
		
@AppliedLoad @BendingMoment
Scenario: Determine the min major bending moment on beam 1
	Given the applied load test file name is Applied loads.st7
	When the uls analysis is run
	Then the min result type: BendingMomentMajor of beam 1 should be -10562500000

@AppliedLoad @AxialForce
Scenario: Determine the max axial force on beam 2
	Given the applied load test file name is Applied loads.st7
	When the uls analysis is run
	Then the max result type: AxialForce of beam 2 should be -400000
		
@AppliedLoad @AxialForce
Scenario: Determine the min axial force on beam 2
	Given the applied load test file name is Applied loads.st7
	When the uls analysis is run
	Then the min result type: AxialForce of beam 2 should be -400000