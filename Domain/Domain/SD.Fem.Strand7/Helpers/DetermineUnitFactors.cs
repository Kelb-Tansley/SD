namespace SD.Fem.Strand7.Helpers
{
    public class DetermineUnitFactors
    {
        public static UnitFactor GetModelUnitFactors(int modelId)
        {
            var units = new int[St7.kLastUnit];
            St7.St7GetUnits(modelId, units).ThrowIfFails();
            // Factors to apply throughout calculations based on units used by user in Strand model 
            return DetermineUnitFactors.GetUnitFactors(units);
        }
        public static UnitFactor GetUnitFactors(int[] Units)
        {
            var unitFactor = new UnitFactor()
            {
                Length = 0,
                Force = 0,
                Stress = 0
            };

            switch (Units[0]) //unitFactor.Length converts the units to mm
            {
                case St7.luMETRE: unitFactor.Length = 1000; break;
                case St7.luCENTIMETRE: unitFactor.Length = 10; break;
                case St7.luMILLIMETRE: unitFactor.Length = 1; break;
                case St7.luFOOT: unitFactor.Length = 304.8; break;
                case St7.luINCH: unitFactor.Length = 25.4; break;
            }
            switch (Units[1])    //unitFactor.Force converts the units to N
            {
                case St7.fuNEWTON: unitFactor.Force = 1; break;
                case St7.fuKILONEWTON: unitFactor.Force = 1000; break;
                case St7.fuMEGANEWTON: unitFactor.Force = 1000000; break;
                case St7.fuKILOFORCE: unitFactor.Force = 9.80665; break;
                case St7.fuTONNEFORCE: unitFactor.Force = 9806.65; break;
                case St7.fuPOUNDFORCE: unitFactor.Force = 4.44822; break;
                case St7.fuKIPFORCE: unitFactor.Force = 4448.22; break;
            }
            switch (Units[2])    //unitFactor.Force converts the units to MPa - To be checked
            {
                case St7.suPASCAL: unitFactor.Stress = 0.000001; break;
                case St7.suKILOPASCAL: unitFactor.Stress = 0.001; break;
                case St7.suMEGAPASCAL: unitFactor.Stress = 1; break;
                case St7.suKSCm: unitFactor.Stress = 9.80665; break;
                case St7.suPSI: unitFactor.Stress = 9806.65; break;
                case St7.suKSI: unitFactor.Stress = 4.44822; break;
                case St7.suPSF: unitFactor.Stress = 4448.22; break;
            }
            return unitFactor;
        }
    }
}
