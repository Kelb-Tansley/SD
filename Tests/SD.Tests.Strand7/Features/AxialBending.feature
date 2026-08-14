Feature: AxialBending

@AxialBending @13.8.5.a
Scenario: Determine the w1 values for beam 1
	Given the fem test file name is Calculate w1 Values Tests.st7
	When the uls analysis is run
	Then the w one major should be 1 and w one minor should be 1 for beam 1
	
@AxialBending @13.8.5.a
Scenario: Determine the w1 values for beam 2
	Given the fem test file name is Calculate w1 Values Tests.st7
	When the uls analysis is run
	Then the w one major should be 0.8 and w one minor should be 0.8 for beam 2

@AxialBending @13.8.5.a
Scenario: Determine the w1 values for beam 3
	Given the fem test file name is Calculate w1 Values Tests.st7
	When the uls analysis is run
	Then the w one major should be 0.4 and w one minor should be 0.4 for beam 3
	
@AxialBending @13.8.5.a
Scenario: Determine the w1 values for beam 4
	Given the fem test file name is Calculate w1 Values Tests.st7
	When the uls analysis is run
	Then the w one major should be 0.6 and w one minor should be 0.6 for beam 4
	
@AxialBending @13.8.5.a
Scenario: Determine the w1 values for beam 5
	Given the fem test file name is Calculate w1 Values Tests.st7
	When the uls analysis is run
	Then the w one major should be 0.8 and w one minor should be 0.8 for beam 5


	
@AxialBending @13.8.5
Scenario: Determine the w1 values for beam 6
	Given the fem test file name is Calculate w1 Values Tests.st7
	When the uls analysis is run
	Then the w one major should be 1 and w one minor should be 1 for beam 6
	
@AxialBending @13.8.5
Scenario: Determine the w1 values for beam 8
	Given the fem test file name is Calculate w1 Values Tests.st7
	When the uls analysis is run
	Then the w one major should be 1 and w one minor should be 1 for beam 8

@AxialBending @13.8.5
Scenario: Determine the w1 values for beam 10
	Given the fem test file name is Calculate w1 Values Tests.st7
	When the uls analysis is run
	Then the w one major should be 1 and w one minor should be 1 for beam 10
	
@AxialBending @13.8.5
Scenario: Determine the w1 values for beam 12
	Given the fem test file name is Calculate w1 Values Tests.st7
	When the uls analysis is run
	Then the w one major should be 1 and w one minor should be 0.6 for beam 12
	
@AxialBending @13.8.5
Scenario: Determine the w1 values for beam 14
	Given the fem test file name is Calculate w1 Values Tests.st7
	When the uls analysis is run
	Then the w one major should be 1 and w one minor should be 0.8 for beam 14

	

@AxialBending @13.8.5
Scenario: Determine the w1 values for beam 7
	Given the fem test file name is Calculate w1 Values Tests.st7
	When the uls analysis is run
	Then the w one major should be 0.85 and w one minor should be 0.85 for beam 7
	
@AxialBending @13.8.5
Scenario: Determine the w1 values for beam 9
	Given the fem test file name is Calculate w1 Values Tests.st7
	When the uls analysis is run
	Then the w one major should be 0.85 and w one minor should be 0.85 for beam 9

@AxialBending @13.8.5
Scenario: Determine the w1 values for beam 11
	Given the fem test file name is Calculate w1 Values Tests.st7
	When the uls analysis is run
	Then the w one major should be 0.85 and w one minor should be 0.4 for beam 11
	
@AxialBending @13.8.5
Scenario: Determine the w1 values for beam 13
	Given the fem test file name is Calculate w1 Values Tests.st7
	When the uls analysis is run
	Then the w one major should be 0.6 and w one minor should be 0.85 for beam 13
	
@AxialBending @13.8.5
Scenario: Determine the w1 values for beam 15
	Given the fem test file name is Calculate w1 Values Tests.st7
	When the uls analysis is run
	Then the w one major should be 0.85 and w one minor should be 0.85 for beam 15