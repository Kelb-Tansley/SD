*SOLUTION COMMENCED AT 12:36 PM ON 8/28/2024 
  
 Strand7 [R3.1.3 a][Solver Build R3.1.3] (64-Bit) 
  
 ANALYSIS TYPE               : LINEAR STATIC 
  
 COMPUTER NAME               : 5CG2385H40 
 USER LOGON NAME             : TANS73647 
 CPU                         : 12th Gen Intel(R) Core(TM) i7-12800H 
 USABLE PHYSICAL MEMORY      : 31.6 GB 
 USABLE VIRTUAL MEMORY       : 41.1 GB 
  
 MODEL FILE                  : "D:\dev\repos\Pdg\Hatch.Pdg.Structural.Design\Tests\SD.Tests.Mathcad\TestFiles\Single\PFC.st7" 
 RESULT FILE                 : "D:\dev\repos\Pdg\Hatch.Pdg.Structural.Design\Tests\SD.Tests.Mathcad\TestFiles\Single\PFC.LSA" 
 SCRATCH PATH                : "C:\Users\tans73647\Strand7 R31\Tmp\" 
  
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
  
 STORAGE SCHEME              : Sparse  
 SORTING METHOD              : AMD  
 SOLUTION TYPE               : Direct  
  
 NUMBER OF EQUATIONS         : 3 
 MATRIX FILL-IN              : 0.0% 
 [K] MATRIX SIZE             : < 1 KB 
 OPTIMUM RAM NEEDED          : 128.0 KB 
 FREE SCRATCH SPACE          : 89.3 GB 
  
*WARNING[237]:Default freedom conditions might be over-constrained - one or more elements might require additional freedoms. 
  
 SUMMATION OF APPLIED LOADS  
          Case        FX           FY           FZ           MX           MY           MZ        Case Name 
 Nodes       1   +0.00000E+00 +0.00000E+00 +0.00000E+00 +1.00000E+05 +0.00000E+00 +1.00000E+05   "1:Load Case 1" 
 Beams       1   +0.00000E+00 +1.00000E+05 +0.00000E+00 +0.00000E+00 +0.00000E+00 +0.00000E+00   "1:Load Case 1" 
 Total       1   +0.00000E+00 +1.00000E+05 +0.00000E+00 +1.00000E+05 +0.00000E+00 +1.00000E+05   "1:Load Case 1" 
 Vector      1   +0.00000E+00 +0.00000E+00 +0.00000E+00 +1.00000E+05 +0.00000E+00 +1.00000E+05   "1:Load Case 1" 
  
 SUMMATION OF MOMENTS OF APPLIED LOADS ABOUT THE ORIGIN [Vector]  
          Case        MXo          MYo          MZo       Case Name 
             1   +1.00000E+05 +0.00000E+00 +1.00000E+05   "1:Load Case 1" 
  
 Reducing 3 equations using 128.0 KB RAM for [K] of < 1 KB... 
  
 MAXIMUM PIVOT               : 1.010294E+06 (Node 2 RZ) 
 MINIMUM PIVOT               : 6.926870E+02 (Node 2 RY) 
  
 Results for 1 load case... 
  
 MAXIMUM NODE DISPLACEMENT MAGNITUDES 
   Case        DX           DY           DZ           RX           RY           RZ       Case Name 
      1    0.00000E+00  0.00000E+00  0.00000E+00  9.84252E-01  9.92927E-01  9.89811E-02  "1:Load Case 1" 
  
 DIRECT SUMMATION OF NODE REACTION FORCES 
   Case        FX           FY           FZ           MX           MY           MZ       Case Name 
      1   +0.00000E+00 -1.00000E+05 +0.00000E+00 +5.00000E+04 +0.00000E+00 +4.65502E+04  "1:Load Case 1" 
  
 TOTAL CPU TIME              : 0.062 Seconds 
  
 
*SUMMARY OF MESSAGES
*Number of Notes    : 0
*Number of Warnings : 1
*Number of Errors   : 0
 
*SOLUTION COMPLETED AT 12:36 PM ON 8/28/2024 
*SOLUTION TIME: 3 Seconds 
