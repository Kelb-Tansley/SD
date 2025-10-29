*SOLUTION COMMENCED AT 2:55 PM ON 17/10/2024 
  
 Strand7 [R3.1.3 a][Solver Build R3.1.3] (64-Bit) 
  
 ANALYSIS TYPE               : LINEAR STATIC 
  
 COMPUTER NAME               : HATCH9000500 
 USER LOGON NAME             : DEWE13600 
 CPU                         : Intel(R) Core(TM) i5-10210U CPU @ 1.60GHz 
 USABLE PHYSICAL MEMORY      : 15.8 GB 
 USABLE VIRTUAL MEMORY       : 22.8 GB 
  
 MODEL FILE                  : "C:\Users\dewe13600\Documents\Hatch.Pdg.Structural.Design\Tests\SD.Tests.Mathcad.AS\TestFiles\Combination\Column Combined Test.st7" 
 RESULT FILE                 : "C:\Users\dewe13600\Documents\Hatch.Pdg.Structural.Design\Tests\SD.Tests.Mathcad.AS\TestFiles\Combination\Column Combined Test.LSA" 
 SCRATCH PATH                : "C:\Users\dewe13600\Strand7 R31\Tmp\" 
  
 TOTALS 
 Nodes                       : 2 
 Beams                       : 1 
 Plates                      : 0 
 Bricks                      : 0 
 Links                       : 0 
  
 SOLVER UNITS 
 Length                      : m 
 Mass                        : kg 
 Force                       : N 
 Stress                      : Pa 
  
 FREEDOM CASE                : "1:Freedom Case 1" 
  
 LOAD CASES                  : "1:Load Case 1" 
                             : "2:T" 
  
 STORAGE SCHEME              : Sparse  
 SORTING METHOD              : AMD  
 SOLUTION TYPE               : Direct  
  
 NUMBER OF EQUATIONS         : 5 
 MATRIX FILL-IN              : 0.0% 
 [K] MATRIX SIZE             : < 1 KB 
 OPTIMUM RAM NEEDED          : 128.0 KB 
 FREE SCRATCH SPACE          : 25.0 GB 
  
*WARNING[237]:Default freedom conditions might be over-constrained - one or more elements might require additional freedoms. 
  
 SUMMATION OF APPLIED LOADS  
          Case        FX           FY           FZ           MX           MY           MZ        Case Name 
 Nodes       1   +0.00000E+00 -1.00000E+04 +0.00000E+00 +0.00000E+00 +0.00000E+00 +0.00000E+00   "1:Load Case 1" 
             2   +0.00000E+00 +1.00000E+04 +0.00000E+00 +0.00000E+00 +0.00000E+00 +0.00000E+00   "2:T" 
 Beams       1   +1.00000E+04 +0.00000E+00 +1.00000E+04 +0.00000E+00 +0.00000E+00 +0.00000E+00   "1:Load Case 1" 
             2   +1.00000E+04 +0.00000E+00 +1.00000E+04 +0.00000E+00 +0.00000E+00 +0.00000E+00   "2:T" 
 Total       1   +1.00000E+04 -1.00000E+04 +1.00000E+04 +0.00000E+00 +0.00000E+00 +0.00000E+00   "1:Load Case 1" 
             2   +1.00000E+04 +1.00000E+04 +1.00000E+04 +0.00000E+00 +0.00000E+00 +0.00000E+00   "2:T" 
 Vector      1   +0.00000E+00 -1.00000E+04 +0.00000E+00 +0.00000E+00 +0.00000E+00 +0.00000E+00   "1:Load Case 1" 
             2   +0.00000E+00 +1.00000E+04 +0.00000E+00 +0.00000E+00 +0.00000E+00 +0.00000E+00   "2:T" 
  
 SUMMATION OF MOMENTS OF APPLIED LOADS ABOUT THE ORIGIN [Vector]  
          Case        MXo          MYo          MZo       Case Name 
             1   +0.00000E+00 +0.00000E+00 +0.00000E+00   "1:Load Case 1" 
             2   +0.00000E+00 +0.00000E+00 +0.00000E+00   "2:T" 
  
 Reducing 5 equations using 128.0 KB RAM for [K] of < 1 KB... 
  
 MAXIMUM PIVOT               : 1.583716E+08 (Node 2 DY) 
 MINIMUM PIVOT               : 5.658374E+05 (Node 2 RX) 
  
 Results for 2 load cases... 
  
 MAXIMUM NODE DISPLACEMENT MAGNITUDES 
   Case        DX           DY           DZ           RX           RY           RZ       Case Name 
      1    0.00000E+00  6.31426E-05  0.00000E+00  1.98733E-02  0.00000E+00  2.02032E-03  "1:Load Case 1" 
      2    0.00000E+00  6.31426E-05  0.00000E+00  1.98733E-02  0.00000E+00  2.02032E-03  "2:T" 
  
 DIRECT SUMMATION OF NODE REACTION FORCES 
   Case        FX           FY           FZ           MX           MY           MZ       Case Name 
      1   -1.00000E+04 +1.00000E+04 -1.00000E+04 +0.00000E+00 +0.00000E+00 +0.00000E+00  "1:Load Case 1" 
      2   -1.00000E+04 -1.00000E+04 -1.00000E+04 +0.00000E+00 +0.00000E+00 +0.00000E+00  "2:T" 
  
 TOTAL CPU TIME              : 0.219 Seconds 
  
 
*SUMMARY OF MESSAGES
*Number of Notes    : 0
*Number of Warnings : 1
*Number of Errors   : 0
 
*SOLUTION COMPLETED AT 2:55 PM ON 17/10/2024 
*SOLUTION TIME: 2 Seconds 
