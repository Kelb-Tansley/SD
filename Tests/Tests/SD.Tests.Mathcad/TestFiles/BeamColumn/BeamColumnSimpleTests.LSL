*SOLUTION COMMENCED AT 1:56 PM ON 8/19/2024 
  
 Strand7 [R3.1.3 a][Solver Build R3.1.3] (64-Bit) 
  
 ANALYSIS TYPE               : LINEAR STATIC 
  
 COMPUTER NAME               : 5CG2385H40 
 USER LOGON NAME             : TANS73647 
 CPU                         : 12th Gen Intel(R) Core(TM) i7-12800H 
 USABLE PHYSICAL MEMORY      : 31.6 GB 
 USABLE VIRTUAL MEMORY       : 41.6 GB 
  
 MODEL FILE                  : "D:\dev\repos\Pdg\Hatch.Pdg.Structural.Design\Tests\SD.Tests.Mathcad\TestFiles\BeamColumn\BeamColumnSimpleTests.st7" 
 RESULT FILE                 : "D:\dev\repos\Pdg\Hatch.Pdg.Structural.Design\Tests\SD.Tests.Mathcad\TestFiles\BeamColumn\BeamColumnSimpleTests.LSA" 
 SCRATCH PATH                : "C:\Users\tans73647\Strand7 R31\Tmp\" 
  
 TOTALS 
 Nodes                       : 10 
 Beams                       : 7 
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
  
 NUMBER OF EQUATIONS         : 37 
 MATRIX FILL-IN              : 0.0% 
 [K] MATRIX SIZE             : 1.6 KB 
 OPTIMUM RAM NEEDED          : 128.0 KB 
 FREE SCRATCH SPACE          : 96.7 GB 
  
 SUMMATION OF APPLIED LOADS  
          Case        FX           FY           FZ           MX           MY           MZ        Case Name 
 Nodes       1   +0.00000E+00 -6.70000E+04 +0.00000E+00 +4.50000E+08 +0.00000E+00 +1.00080E+08   "1:Load Case 1" 
 Beams       1   +1.60000E+07 -1.30000E+06 +8.00000E+06 -4.76837E-07 +0.00000E+00 +9.53674E-07   "1:Load Case 1" 
 Total       1   +1.60000E+07 -1.36700E+06 +8.00000E+06 +4.50000E+08 +0.00000E+00 +1.00080E+08   "1:Load Case 1" 
 Vector      1   +0.00000E+00 -4.67000E+05 +0.00000E+00 +4.50000E+08 +7.45058E-09 +1.00080E+08   "1:Load Case 1" 
  
 SUMMATION OF MOMENTS OF APPLIED LOADS ABOUT THE ORIGIN [Vector]  
          Case        MXo          MYo          MZo       Case Name 
             1   +4.50000E+08 +7.45058E-09 -8.66920E+08   "1:Load Case 1" 
  
 Reducing 37 equations using 128.0 KB RAM for [K] of 1.6 KB... 
  
 MAXIMUM PIVOT               : 1.590536E+11 (Node 4 RZ) 
 MINIMUM PIVOT               : 6.844444E+02 (Node 3 DZ) 
  
 Results for 1 load case... 
  
 MAXIMUM NODE DISPLACEMENT MAGNITUDES 
   Case        DX           DY           DZ           RX           RY           RZ       Case Name 
      1    2.46711E+01  5.05763E+00  3.65260E+01  3.17235E+00  4.17333E+00  1.38318E-01  "1:Load Case 1" 
  
 DIRECT SUMMATION OF NODE REACTION FORCES 
   Case        FX           FY           FZ           MX           MY           MZ       Case Name 
      1   -1.60000E+07 +1.36700E+06 -8.00000E+06 +3.96743E-07 -4.48364E+02 -2.16995E-06  "1:Load Case 1" 
  
 TOTAL CPU TIME              : 0.062 Seconds 
  
 
*SUMMARY OF MESSAGES
*Number of Notes    : 0
*Number of Warnings : 0
*Number of Errors   : 0
 
*SOLUTION COMPLETED AT 1:56 PM ON 8/19/2024 
*SOLUTION TIME: 3 Seconds 
