*SOLUTION COMMENCED AT 3:51 PM ON 7/19/2024 
  
 Strand7 [R3.1.2][Solver Build R3.1.2] (64-Bit) 
  
 ANALYSIS TYPE               : LINEAR STATIC 
  
 COMPUTER NAME               : 5CG2385H40 
 USER LOGON NAME             : TANS73647 
 CPU                         : 12th Gen Intel(R) Core(TM) i7-12800H 
 USABLE PHYSICAL MEMORY      : 31.6 GB 
 USABLE VIRTUAL MEMORY       : 45.1 GB 
  
 MODEL FILE                  : "D:\dev\repos\Pdg\Hatch.Pdg.Structural.Design\Tests\SD.Tests.Strand7\TestFiles\Applied loads.st7" 
 RESULT FILE                 : "D:\dev\repos\Pdg\Hatch.Pdg.Structural.Design\Tests\SD.Tests.Strand7\TestFiles\Applied loads.LSA" 
 SCRATCH PATH                : "C:\Users\tans73647\Strand7 R31\Tmp\" 
  
 TOTALS 
 Nodes                       : 5 
 Beams                       : 4 
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
  
 NUMBER OF EQUATIONS         : 24 
 MATRIX FILL-IN              : 0.0% 
 [K] MATRIX SIZE             : 1.4 KB 
 OPTIMUM RAM NEEDED          : 128.0 KB 
 FREE SCRATCH SPACE          : 102.8 GB 
  
 SUMMATION OF APPLIED LOADS  
          Case        FX           FY           FZ           MX           MY           MZ        Case Name 
 Nodes       1   +0.00000E+00 -4.00000E+05 +0.00000E+00 +0.00000E+00 +0.00000E+00 +0.00000E+00   "1:Load Case 1" 
 Beams       1   -1.40000E+07 +0.00000E+00 -4.00000E+06 +8.19564E-08 +0.00000E+00 -3.27826E-07   "1:Load Case 1" 
 Total       1   -1.40000E+07 -4.00000E+05 -4.00000E+06 +8.19564E-08 +0.00000E+00 -3.27826E-07   "1:Load Case 1" 
 Vector      1   -9.25000E+06 -4.00000E+05 -3.06250E+06 +4.47035E-08 +0.00000E+00 -6.55651E-07   "1:Load Case 1" 
  
 SUMMATION OF MOMENTS OF APPLIED LOADS ABOUT THE ORIGIN [Vector]  
          Case        MXo          MYo          MZo       Case Name 
             1   -6.00000E+09 +0.00000E+00 +2.10000E+10   "1:Load Case 1" 
  
 Reducing 24 equations using 128.0 KB RAM for [K] of 1.4 KB... 
  
 MAXIMUM PIVOT               : 4.864000E+10 (Node 1 RZ) 
 MINIMUM PIVOT               : 1.129333E+04 (Node 3 DZ) 
  
 Results for 1 load case... 
  
 MAXIMUM NODE DISPLACEMENT MAGNITUDES 
   Case        DX           DY           DZ           RX           RY           RZ       Case Name 
      1    4.37911E+03  2.04082E+00  3.74391E+03  2.10024E+00  0.00000E+00  2.40543E+00  "1:Load Case 1" 
  
 DIRECT SUMMATION OF NODE REACTION FORCES 
   Case        FX           FY           FZ           MX           MY           MZ       Case Name 
      1   +1.40000E+07 +4.00000E+05 +4.00000E+06 +3.20561E-05 +0.00000E+00 +7.19726E-06  "1:Load Case 1" 
  
 TOTAL CPU TIME              : 0.094 Seconds 
  
 
*SUMMARY OF MESSAGES
*Number of Notes    : 0
*Number of Warnings : 0
*Number of Errors   : 0
 
*SOLUTION COMPLETED AT 3:51 PM ON 7/19/2024 
*SOLUTION TIME: 4 Seconds 
