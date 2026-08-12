*SOLUTION COMMENCED AT 23:25 ON 2026/08/12 
  
 Strand7 [R3.1.6][Solver Build R3.1.6] (64-Bit) [EVALUATION USE ONLY] 
  
 LICENCED TO                 : Kelby Tansley 
  
 ANALYSIS TYPE               : LINEAR STATIC 
  
 COMPUTER NAME               : KELBY 
 USER LOGON NAME             : kelby 
 CPU                         : Intel(R) Core(TM) Ultra 7 155H 
 USABLE PHYSICAL MEMORY      : 15.4 GB 
 USABLE VIRTUAL MEMORY       : 43.4 GB 
  
 MODEL FILE                  : "D:\mine\SD\Tests\SD.Tests.Strand7\TestFiles\Calculate w1 Values Tests.st7" 
 RESULT FILE                 : "D:\mine\SD\Tests\SD.Tests.Strand7\TestFiles\Calculate w1 Values Tests.LSA" 
 SCRATCH PATH                : "C:\Users\kelby\Strand7 R31\Tmp\" 
  
 TOTALS 
 Nodes                       : 30 
 Beams                       : 15 
 Plates                      : 0 
 Bricks                      : 0 
 Links                       : 0 
  
 SOLVER UNITS 
 Length                      : mm 
 Mass                        : t 
 Force                       : N 
 Stress                      : MPa 
  
 FREEDOM CASE                : "1:Freedom Case 1" 
  
 LOAD CASES                  : "1:Load Case 1" 
  
 STORAGE SCHEME              : Sparse  
 SORTING METHOD              : AMD  
 SOLUTION TYPE               : Direct  
  
 NUMBER OF EQUATIONS         : 75 
 MATRIX FILL-IN              : 0.0% 
 [K] MATRIX SIZE             : 1.8 KB 
 OPTIMUM RAM NEEDED          : 128.0 KB 
 FREE SCRATCH SPACE          : 142.1 GB 
  
*WARNING[237]:Default freedom conditions might be over-constrained - one or more elements might require additional freedoms. 
  
 SUMMATION OF APPLIED LOADS  
          Case        FX           FY           FZ           MX           MY           MZ        Case Name 
 Nodes       1   +0.00000E+00 +0.00000E+00 +1.50000E+02 +1.50000E+03 +1.50000E+02 +0.00000E+00   "1:Load Case 1" 
 Beams       1   +1.95000E+02 +1.90000E+02 +0.00000E+00 +4.46885E+03 +4.77400E+02 +0.00000E+00   "1:Load Case 1" 
 Total       1   +1.95000E+02 +1.90000E+02 +1.50000E+02 +5.96885E+03 +6.27400E+02 +0.00000E+00   "1:Load Case 1" 
 Vector      1   +0.00000E+00 +0.00000E+00 +1.50000E+02 +5.96885E+03 +6.27400E+02 +0.00000E+00   "1:Load Case 1" 
  
 SUMMATION OF MOMENTS OF APPLIED LOADS ABOUT THE ORIGIN [Vector]  
          Case        MXo          MYo          MZo       Case Name 
             1   +5.96885E+03 -2.99373E+05 +0.00000E+00   "1:Load Case 1" 
  
 Reducing 75 equations using 128.0 KB RAM for [K] of 1.8 KB... 
  
 MAXIMUM PIVOT               : 3.467316E+10 (Node 7 RX) 
 MINIMUM PIVOT               : 2.041620E+05 (Node 28 DZ) 
  
 Results for 1 load case... 
  
 MAXIMUM NODE DISPLACEMENT MAGNITUDES 
   Case        DX           DY           DZ           RX           RY           RZ       Case Name 
      1    0.00000E+00  0.00000E+00  4.89807E-05  4.40446E-06  3.09633E-05  0.00000E+00  "1:Load Case 1" 
  
 DIRECT SUMMATION OF NODE REACTION FORCES 
   Case        FX           FY           FZ           MX           MY           MZ       Case Name 
      1   -1.95000E+02 -1.90000E+02 -1.50000E+02 +5.29532E-12 -4.19931E-12 +0.00000E+00  "1:Load Case 1" 
  
 TOTAL CPU TIME              : 0.375 Seconds 
  
 
*SUMMARY OF MESSAGES
*Number of Notes    : 0
*Number of Warnings : 1
*Number of Errors   : 0
 
*SOLUTION COMPLETED AT 23:25 ON 2026/08/12 
*SOLUTION TIME: 4 Seconds 
