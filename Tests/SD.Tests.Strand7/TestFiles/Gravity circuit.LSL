*SOLUTION COMMENCED AT 08:55 ON 2026/08/26 
  
 Strand7 [R3.1.6][Solver Build R3.1.6] (64-Bit) [EVALUATION USE ONLY] 
  
 LICENCED TO                 : Kelby Tansley 
  
 ANALYSIS TYPE               : LINEAR STATIC 
  
 COMPUTER NAME               : KELBY 
 USER LOGON NAME             : kelby 
 CPU                         : Intel(R) Core(TM) Ultra 7 155H 
 USABLE PHYSICAL MEMORY      : 15.4 GB 
 USABLE VIRTUAL MEMORY       : 45.4 GB 
  
 MODEL FILE                  : "D:\mine\SD\Tests\SD.Tests.Strand7\TestFiles\Gravity circuit.st7" 
 RESULT FILE                 : "D:\mine\SD\Tests\SD.Tests.Strand7\TestFiles\Gravity circuit.LSA" 
 SCRATCH PATH                : "C:\Users\kelby\Strand7 R31\Tmp\" 
  
 TOTALS 
 Nodes                       : 232 
 Beams                       : 496 
 Plates                      : 59 
 Bricks                      : 0 
 Links                       : 16 
  
 SOLVER UNITS 
 Length                      : m 
 Mass                        : kg 
 Force                       : N 
 Stress                      : Pa 
  
 FREEDOM CASE                : "1:Freedom Case 1" 
  
 LOAD CASES                  : "1:GZ" 
                             : "2:Wind" 
                             : "3:Equipment Static" 
                             : "4:Equipment Dynamic" 
                             : "5:Platform imposed loads" 
  
 STORAGE SCHEME              : Sparse  
 SORTING METHOD              : AMD  
 SOLUTION TYPE               : Direct  
  
 NUMBER OF EQUATIONS         : 1328 
 MATRIX FILL-IN              : 51.0% 
 [K] MATRIX SIZE             : 379.9 KB 
 OPTIMUM RAM NEEDED          : 384.0 KB 
 FREE SCRATCH SPACE          : 134.4 GB 
  
 SUMMATION OF APPLIED LOADS  
          Case        FX           FY           FZ           MX           MY           MZ        Case Name 
 Nodes       1   +0.00000E+00 +0.00000E+00 +0.00000E+00 +0.00000E+00 +0.00000E+00 +0.00000E+00   "1:GZ" 
             2   +3.80000E+04 +0.00000E+00 +0.00000E+00 +0.00000E+00 +0.00000E+00 +0.00000E+00   "2:Wind" 
             3   +0.00000E+00 +0.00000E+00 -5.20000E+05 +0.00000E+00 +0.00000E+00 +0.00000E+00   "3:Equipment Static" 
             4   -1.23793E+05 -1.23793E+05 -5.80000E+04 +0.00000E+00 +0.00000E+00 +0.00000E+00   "4:Equipment Dynamic" 
             5   +0.00000E+00 +0.00000E+00 +0.00000E+00 +0.00000E+00 +0.00000E+00 +0.00000E+00   "5:Platform imposed loads" 
 Beams       1   +8.26905E-13 -4.52998E-13 -3.44790E+05 -1.39962E+03 -7.71359E+02 +3.11600E-29   "1:GZ" 
             2   -8.50914E+04 -5.11532E+02 -3.30248E+05 -2.42662E+03 +8.23266E+03 +2.03635E-13   "2:Wind" 
             3   +0.00000E+00 +0.00000E+00 +0.00000E+00 +0.00000E+00 +0.00000E+00 +0.00000E+00   "3:Equipment Static" 
             4   +0.00000E+00 +0.00000E+00 +0.00000E+00 +0.00000E+00 +0.00000E+00 +0.00000E+00   "4:Equipment Dynamic" 
             5   -2.85568E-28 +1.38208E-27 -5.54663E+05 -2.37960E+03 -7.35037E+03 +6.47806E-29   "5:Platform imposed loads" 
 Plates      1   +0.00000E+00 +0.00000E+00 +0.00000E+00 +0.00000E+00 +0.00000E+00 +0.00000E+00   "1:GZ" 
             2   +0.00000E+00 +0.00000E+00 +0.00000E+00 +0.00000E+00 +0.00000E+00 +0.00000E+00   "2:Wind" 
             3   +0.00000E+00 +0.00000E+00 +0.00000E+00 +0.00000E+00 +0.00000E+00 +0.00000E+00   "3:Equipment Static" 
             4   +0.00000E+00 +0.00000E+00 +0.00000E+00 +0.00000E+00 +0.00000E+00 +0.00000E+00   "4:Equipment Dynamic" 
             5   +0.00000E+00 +0.00000E+00 +0.00000E+00 +0.00000E+00 +0.00000E+00 +0.00000E+00   "5:Platform imposed loads" 
 Total       1   +8.26905E-13 -4.52998E-13 -3.44790E+05 -1.39962E+03 -7.71359E+02 +3.11600E-29   "1:GZ" 
             2   -4.70914E+04 -5.11532E+02 -3.30248E+05 -2.42662E+03 +8.23266E+03 +2.03635E-13   "2:Wind" 
             3   +0.00000E+00 +0.00000E+00 -5.20000E+05 +0.00000E+00 +0.00000E+00 +0.00000E+00   "3:Equipment Static" 
             4   -1.23793E+05 -1.23793E+05 -5.80000E+04 +0.00000E+00 +0.00000E+00 +0.00000E+00   "4:Equipment Dynamic" 
             5   -2.85568E-28 +1.38208E-27 -5.54663E+05 -2.37960E+03 -7.35037E+03 +6.47806E-29   "5:Platform imposed loads" 
 Vector      1   +1.04124E-12 -2.82467E-13 -3.32628E+05 -3.17863E+03 -2.73117E+03 +4.17110E-29   "1:GZ" 
             2   -4.17234E+04 -4.28432E+02 -3.28172E+05 -5.32972E+03 +1.10054E+04 -2.80000E+03   "2:Wind" 
             3   +0.00000E+00 +0.00000E+00 -5.20000E+05 -7.00000E+04 -7.75000E+04 +0.00000E+00   "3:Equipment Static" 
             4   -1.23793E+05 -1.23793E+05 -5.80000E+04 -3.65000E+03 +7.00000E+02 -2.21250E+03   "4:Equipment Dynamic" 
             5   -1.10365E-28 +1.10401E-27 -5.54663E+05 -2.37960E+03 -7.35037E+03 +6.47806E-29   "5:Platform imposed loads" 
  
 SUMMATION OF MOMENTS OF APPLIED LOADS ABOUT THE ORIGIN [Vector]  
          Case        MXo          MYo          MZo       Case Name 
             1   +1.45493E+06 -1.26758E+06 +2.82622E-12   "1:GZ" 
             2   +1.46179E+06 -1.91087E+06 -1.91464E+05   "2:Wind" 
             3   +2.17500E+06 -2.34210E+06 +0.00000E+00   "3:Equipment Static" 
             4   +2.55346E+06 -2.61504E+06 -6.63469E+04   "4:Equipment Dynamic" 
             5   +2.33250E+06 -2.41547E+06 -6.10308E-27   "5:Platform imposed loads" 
  
 Reducing 1328 equations using 384.0 KB RAM for [K] of 379.9 KB... 
  
 MAXIMUM PIVOT               : 1.426869E+11 (Node 219 DZ) 
 MINIMUM PIVOT               : 1.520550E+03 (Node 127 RY) 
  
 Results for 5 load cases... 
  
 MAXIMUM NODE DISPLACEMENT MAGNITUDES 
   Case        DX           DY           DZ           RX           RY           RZ       Case Name 
      1    1.70029E-03  1.16610E-03  6.32596E-03  4.64579E-03  2.18929E-03  9.85561E-04  "1:GZ" 
      2    2.91960E-03  1.20311E-03  1.04730E-02  2.80940E-03  1.65683E-03  6.83494E-04  "2:Wind" 
      3    4.02850E-03  1.98119E-03  1.34007E-02  3.54400E-03  3.99332E-03  2.81724E-03  "3:Equipment Static" 
      4    8.14544E-03  6.72402E-03  4.97142E-03  5.21304E-03  5.37157E-03  2.02424E-03  "4:Equipment Dynamic" 
      5    3.10439E-03  1.93597E-03  1.71743E-02  1.20490E-02  6.74651E-03  4.37979E-03  "5:Platform imposed loads" 
  
 DIRECT SUMMATION OF NODE REACTION FORCES 
   Case        FX           FY           FZ           MX           MY           MZ       Case Name 
      1   +1.72804E-13 +1.15676E-11 +3.44790E+05 +1.38389E-09 +1.34549E+02 -2.74454E-10  "1:GZ" 
      2   +4.70914E+04 +5.11532E+02 +3.30248E+05 -6.87990E-10 +3.16691E+02 -1.27587E-10  "2:Wind" 
      3   +2.35225E-11 -1.99236E-11 +5.20000E+05 +7.23340E-09 -5.42939E+01 +1.85405E-09  "3:Equipment Static" 
      4   +1.23793E+05 +1.23793E+05 +5.80000E+04 +2.46617E-10 +8.10842E+00 -3.21538E-09  "4:Equipment Dynamic" 
      5   -8.44125E-12 +1.11839E-11 +5.54662E+05 -2.23508E-09 -5.57523E+01 -6.63192E-10  "5:Platform imposed loads" 
  
 TOTAL CPU TIME              : 0.453 Seconds 
  
 
*SUMMARY OF MESSAGES
*Number of Notes    : 0
*Number of Warnings : 0
*Number of Errors   : 0
 
*SOLUTION COMPLETED AT 08:55 ON 2026/08/26 
*SOLUTION TIME: 0 Seconds 
