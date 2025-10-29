*SOLUTION COMMENCED AT 1:20 PM ON 6/5/2024 
  
 Strand7 [R3.1.3 a][Solver Build R3.1.3] (64-Bit) 
  
 ANALYSIS TYPE               : LINEAR STATIC 
  
 COMPUTER NAME               : 5CG2385H40 
 USER LOGON NAME             : TANS73647 
 CPU                         : 12th Gen Intel(R) Core(TM) i7-12800H 
 USABLE PHYSICAL MEMORY      : 31.6 GB 
 USABLE VIRTUAL MEMORY       : 47.1 GB 
  
 MODEL FILE                  : "D:\dev\repos\Pdg\Hatch.Pdg.Structural.Design\SD.Tests\TestFiles\Beam Resistance Test.st7" 
 RESULT FILE                 : "D:\dev\repos\Pdg\Hatch.Pdg.Structural.Design\SD.Tests\TestFiles\Beam Resistance Test.LSA" 
 SCRATCH PATH                : "C:\Users\tans73647\Strand7 R31\Tmp\" 
  
 TOTALS 
 Nodes                       : 15 
 Beams                       : 10 
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
  
 NUMBER OF EQUATIONS         : 60 
 MATRIX FILL-IN              : 0.0% 
 [K] MATRIX SIZE             : 2.7 KB 
 OPTIMUM RAM NEEDED          : 128.0 KB 
 FREE SCRATCH SPACE          : 148.6 GB 
  
 SUMMATION OF APPLIED LOADS  
          Case        FX           FY           FZ           MX           MY           MZ        Case Name 
 Nodes       1   +0.00000E+00 +0.00000E+00 +0.00000E+00 +0.00000E+00 +0.00000E+00 +1.00000E+08   "1:Load Case 1" 
 Beams       1   +0.00000E+00 -1.50000E+05 +0.00000E+00 +0.00000E+00 +0.00000E+00 +0.00000E+00   "1:Load Case 1" 
 Total       1   +0.00000E+00 -1.50000E+05 +0.00000E+00 +0.00000E+00 +0.00000E+00 +1.00000E+08   "1:Load Case 1" 
 Vector      1   +0.00000E+00 -7.50000E+04 +0.00000E+00 +0.00000E+00 +0.00000E+00 +1.00000E+08   "1:Load Case 1" 
  
 SUMMATION OF MOMENTS OF APPLIED LOADS ABOUT THE ORIGIN [Vector]  
          Case        MXo          MYo          MZo       Case Name 
             1   +1.12500E+08 +0.00000E+00 -1.25000E+07   "1:Load Case 1" 
  
 Reducing 60 equations using 128.0 KB RAM for [K] of 2.7 KB... 
  
 MAXIMUM PIVOT               : 5.240000E+10 (Node 4 RZ) 
 MINIMUM PIVOT               : 1.255111E+03 (Node 13 DZ) 
  
 Results for 1 load case... 
  
 MAXIMUM NODE DISPLACEMENT MAGNITUDES 
   Case        DX           DY           DZ           RX           RY           RZ       Case Name 
      1    0.00000E+00  1.67411E+01  0.00000E+00  8.82672E-02  0.00000E+00  2.48016E-02  "1:Load Case 1" 
  
 DIRECT SUMMATION OF NODE REACTION FORCES 
   Case        FX           FY           FZ           MX           MY           MZ       Case Name 
      1   +0.00000E+00 +1.50000E+05 +0.00000E+00 +0.00000E+00 +0.00000E+00 -2.60770E-08  "1:Load Case 1" 
  
 TOTAL CPU TIME              : 0.062 Seconds 
  
 
*SUMMARY OF MESSAGES
*Number of Notes    : 0
*Number of Warnings : 0
*Number of Errors   : 0
 
*SOLUTION COMPLETED AT 1:20 PM ON 6/5/2024 
*SOLUTION TIME: 4 Seconds 
