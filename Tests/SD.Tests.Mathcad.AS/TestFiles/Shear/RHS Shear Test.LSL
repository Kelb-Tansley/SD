*SOLUTION COMMENCED AT 3:06 PM ON 17/10/2024 
  
 Strand7 [R3.1.3 a][Solver Build R3.1.3] (64-Bit) 
  
 ANALYSIS TYPE               : LINEAR STATIC 
  
 COMPUTER NAME               : HATCH9000500 
 USER LOGON NAME             : DEWE13600 
 CPU                         : Intel(R) Core(TM) i5-10210U CPU @ 1.60GHz 
 USABLE PHYSICAL MEMORY      : 15.8 GB 
 USABLE VIRTUAL MEMORY       : 22.8 GB 
  
 MODEL FILE                  : "C:\Users\dewe13600\Documents\Hatch.Pdg.Structural.Design\Tests\SD.Tests.Mathcad.AS\TestFiles\Shear\RHS Shear Test.st7" 
 RESULT FILE                 : "C:\Users\dewe13600\Documents\Hatch.Pdg.Structural.Design\Tests\SD.Tests.Mathcad.AS\TestFiles\Shear\RHS Shear Test.LSA" 
 SCRATCH PATH                : "C:\Users\dewe13600\Strand7 R31\Tmp\" 
  
 TOTALS 
 Nodes                       : 2 
 Beams                       : 1 
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
  
 NUMBER OF EQUATIONS         : 6 
 MATRIX FILL-IN              : 0.0% 
 [K] MATRIX SIZE             : < 1 KB 
 OPTIMUM RAM NEEDED          : 128.0 KB 
 FREE SCRATCH SPACE          : 25.0 GB 
  
*WARNING[237]:Default freedom conditions might be over-constrained - one or more elements might require additional freedoms. 
  
 SUMMATION OF APPLIED LOADS  
          Case        FX           FY           FZ           MX           MY           MZ        Case Name 
 Beams       1   +1.00000E+04 +0.00000E+00 +0.00000E+00 +0.00000E+00 +0.00000E+00 +0.00000E+00   "1:Load Case 1" 
 Total       1   +1.00000E+04 +0.00000E+00 +0.00000E+00 +0.00000E+00 +0.00000E+00 +0.00000E+00   "1:Load Case 1" 
 Vector      1   +0.00000E+00 +0.00000E+00 +0.00000E+00 +0.00000E+00 +0.00000E+00 +0.00000E+00   "1:Load Case 1" 
  
 SUMMATION OF MOMENTS OF APPLIED LOADS ABOUT THE ORIGIN [Vector]  
          Case        MXo          MYo          MZo       Case Name 
             1   +0.00000E+00 +0.00000E+00 +0.00000E+00   "1:Load Case 1" 
  
 Reducing 6 equations using 128.0 KB RAM for [K] of < 1 KB... 
  
 MAXIMUM PIVOT               : 7.100356E+09 (Node 1 RZ) 
 MINIMUM PIVOT               : 2.200000E+05 (Node 2 DY) 
  
 Results for 1 load case... 
  
 MAXIMUM NODE DISPLACEMENT MAGNITUDES 
   Case        DX           DY           DZ           RX           RY           RZ       Case Name 
      1    0.00000E+00  0.00000E+00  0.00000E+00  0.00000E+00  0.00000E+00  2.09497E-03  "1:Load Case 1" 
  
 DIRECT SUMMATION OF NODE REACTION FORCES 
   Case        FX           FY           FZ           MX           MY           MZ       Case Name 
      1   -1.00000E+04 +0.00000E+00 +0.00000E+00 +0.00000E+00 +0.00000E+00 +0.00000E+00  "1:Load Case 1" 
  
 TOTAL CPU TIME              : 0.125 Seconds 
  
 
*SUMMARY OF MESSAGES
*Number of Notes    : 0
*Number of Warnings : 1
*Number of Errors   : 0
 
*SOLUTION COMPLETED AT 3:06 PM ON 17/10/2024 
*SOLUTION TIME: 1 Second 
