using SD.Core.Shared.Models;
using SD.Core.Shared.Models.Loading;
using SD.Element.Design.Services;

namespace SD.Fem.Strand7.Services;
public class StrandApiService(
    IDesignCodeAdapter femDesignAdapter,
    IConnectionService connectionService,
    IEffectiveLengthService effectiveLengthService,
    IContourFileService contourFileService) : IStrandApiService
{
    private readonly IDesignCodeAdapter _femDesignAdapter = femDesignAdapter ?? throw new ArgumentNullException(nameof(femDesignAdapter));
    private readonly IConnectionService _connectionService = connectionService ?? throw new ArgumentNullException(nameof(connectionService));
    private readonly IContourFileService _contourFileService = contourFileService ?? throw new ArgumentNullException(nameof(contourFileService));

    public bool OpenFemFile(int modelId, string fileName, bool closeFirst = true, bool openReadOnly = true)
    {
        if (!_connectionService.IsApiConnected)
            return false;

        if (closeFirst)
            _ = St7.St7CloseFile(modelId).HandleApiError();

        if (openReadOnly)
            return OpenFileReadOnly(modelId, fileName);

        var result = St7.St7OpenFile(modelId, fileName, _connectionService.GetScratchLocation()).HandleApiError();
        if (result.ErrorCode == St7.ERR7_FileAlreadyOpen)
            return true;

        return !result.IsValid ? OpenFileReadOnly(modelId, fileName) : result.IsValid;
    }

    private bool OpenFileReadOnly(int modelId, string fileName)
    {
        var readOnlyResult = St7.St7OpenFileReadOnly(modelId, fileName, _connectionService.GetScratchLocation()).HandleApiError();
        return readOnlyResult.ErrorCode == St7.ERR7_FileAlreadyOpen || readOnlyResult.IsValid;
    }

    public void ClearFemDisplayModel(int modelId)
    {
        _ = St7.St7DestroyModelWindow(modelId).HandleApiError();
        _ = St7.St7ClearModelWindow(modelId).HandleApiError();
    }

    public IEnumerable<Beam> GetDisplayedByGroupBeams(int modelId, IEnumerable<Beam> beams)
    {
        var visibleBeams = new List<Beam>();
        foreach (var beam in beams)
        {
            var groupId = 0;
            byte visible = St7.btFalse;
            St7.St7GetEntityGroup(modelId, St7.tyBEAM, beam.Number, ref groupId).ThrowIfFails();
            St7.St7GetGroupVisibility(modelId, groupId, ref visible).ThrowIfFails();
            if (visible == St7.btTrue)
                visibleBeams.Add(beam);
        }
        return visibleBeams;
    }

    public void DisplayFemFile(int modelId, nint handle)
    {
        St7.St7CreateModelWindow(modelId).ThrowIfFails();
        St7.St7SetModelWindowParent(modelId, handle).ThrowIfFails();
        St7.St7ShowModelWindow(modelId).ThrowIfFails();

        St7.St7ShowWindowEntityPanel(modelId).ThrowIfFails();
        St7.St7ShowWindowShowHideToolbar(modelId).ThrowIfFails();
        St7.St7EnableWindowViewChanges(modelId).ThrowIfFails();
        St7.St7EnableWindowEntityInspector(modelId).ThrowIfFails();
        St7.St7ShowWindowSelectionToolbar(modelId).ThrowIfFails();
        St7.St7ShowWindowStatusBar(modelId).ThrowIfFails();
        St7.St7ShowWindowViewToolbar(modelId).ThrowIfFails();
    }

    public void RunLinearStaticAnalysis(int modelId)
    {
        St7.St7RunSolver(modelId, St7.stLinearStatic, St7.smProgressRun, St7.btTrue).ThrowIfFails();
    }

    public void RunLinearBucklingAnalysis(int modelId)
    {
        St7.St7RunSolver(modelId, St7.stLinearBuckling, St7.smNormalCloseRun, St7.btTrue).ThrowIfFails();
    }

    public void EnableFirstLoadFreedomCase(int modelId, string resultFilePath)
    {
        St7.St7SetResultFileName(modelId, resultFilePath).ThrowIfFails();
        St7.St7EnableLSALoadCase(modelId, 1, 1).ThrowIfFails();
    }

    public void SetLinearBucklingModes(int modelId, string fileName, string resultFilePath, int numModes, int variableCaseNum, int fixedCaseNum)
    {
        St7.St7SetLBAInitial(modelId, $"{fileName}.LSA", variableCaseNum, fixedCaseNum).ThrowIfFails();
        St7.St7SetLBANumModes(modelId, numModes).ThrowIfFails();
        St7.St7SetResultLogFileName(modelId, resultFilePath).ThrowIfFails();
        St7.St7SetResultFileName(modelId, resultFilePath).ThrowIfFails();
    }

    public void DisplayFocusedFemFile(int modelId, nint handle, int loadCase, ref int childId, bool isInitialized, ZoomLevel zoomLevel, Beam focusedBeam, IEnumerable<Beam> beams, BeamDisplayComponent beamDisplayComponent)
    {
        if (!isInitialized)
            InitializeResultModelWindow(modelId, handle);

        HideNonBeamEntities(modelId);

        St7.St7SetWindowResultCase(modelId, loadCase).ThrowIfFails();

        St7.St7DeleteGroup(modelId, childId).HandleApiError();

        var numGroups = 0;
        St7.St7GetNumGroups(modelId, ref numGroups).ThrowIfFails();

        var maxId = 0;
        for (var i = 1; i <= numGroups; i++)
        {
            var groupName = new StringBuilder(0, 100) { Capacity = 100 };
            var groupId = 0;
            St7.St7GetGroupByIndex(modelId, i, groupName, 255, ref groupId).ThrowIfFails();
            St7.St7HideGroup(modelId, groupId).ThrowIfFails();

            if (groupId >= maxId)
                maxId = groupId;
        }
        var mainGroupName = new StringBuilder(0, 100) { Capacity = 100 };
        var mainGroupId = 0;
        St7.St7GetGroupByIndex(modelId, 1, mainGroupName, 255, ref mainGroupId).ThrowIfFails();

        if (childId != maxId + 1)
            childId = maxId + 1;

        St7.St7NewChildGroup(modelId, mainGroupId, $"Design for Beam {focusedBeam.Number}", ref childId).ThrowIfFails();

        var connectedBeams = GetConnectedBeamsByZoomLevel(focusedBeam, zoomLevel, beams);

        foreach (var beam in connectedBeams)
            St7.St7SetEntityGroup(modelId, St7.tyBEAM, beam.Number, childId).ThrowIfFails();

        St7.St7ShowGroup(modelId, mainGroupId).ThrowIfFails();
        St7.St7ShowGroup(modelId, childId).ThrowIfFails();
        St7.St7HideGroup(modelId, mainGroupId).ThrowIfFails();
        St7.St7HideGroup(modelId, childId).ThrowIfFails();
        St7.St7ShowGroup(modelId, childId).ThrowIfFails();

        var beamResultsSettings = new int[13];
        beamResultsSettings[St7.ipResultType] = St7.rtAsDiagram;
        beamResultsSettings[St7.ipResultQuantity] = St7.rqBeamForceC;
        beamResultsSettings[St7.ipResultSystem] = St7.stBeamLocal;

        beamResultsSettings[St7.ipDiagram1] = beamDisplayComponent.ShearForceX ? St7.btTrue : St7.btFalse;
        beamResultsSettings[St7.ipDiagram2] = beamDisplayComponent.BendingMomentX ? St7.btTrue : St7.btFalse;
        beamResultsSettings[St7.ipDiagram3] = beamDisplayComponent.ShearForceY ? St7.btTrue : St7.btFalse;
        beamResultsSettings[St7.ipDiagram4] = beamDisplayComponent.BendingMomentY ? St7.btTrue : St7.btFalse;
        beamResultsSettings[St7.ipDiagram5] = beamDisplayComponent.AxialForce ? St7.btTrue : St7.btFalse;
        beamResultsSettings[St7.ipDiagram6] = beamDisplayComponent.Torque ? St7.btTrue : St7.btFalse;

        St7.St7SetBeamResultDisplay(modelId, beamResultsSettings).ThrowIfFails();
        St7.St7SetDisplacementScale(modelId, 0, St7.dsPercent).ThrowIfFails();

        St7.St7RedrawModel(modelId, St7.btTrue).ThrowIfFails();
    }
    public void ApplyBeamWindLoads(int modelId, int loadCase, double[] windLoadVector, WindLoadingModel windLoadingModel, IEnumerable<Beam> beams, UnitFactor unitFactor)
    {
        var tolerance = 0.01;

        CloseFemResultsFile(modelId);
        var convertedPressure = windLoadingModel.WindPressure * unitFactor.Force / (unitFactor.Length * unitFactor.Length);
        foreach (var beam in beams)
        {
            var shapeFactor = GetSectionShapeFactor(beam.Section.SectionType, windLoadingModel);

            var beamAxis = new double[9]; //Unit vectors defining directions 1, 2 and 3 of the beam
            St7.St7GetBeamAxisSystemInitial(modelId, beam.Number, beamAxis).ThrowIfFails();

            var vectorAxis1 = new double[3] { beamAxis[0], beamAxis[1], beamAxis[2] };
            var vectorAxis2 = new double[3] { beamAxis[3], beamAxis[4], beamAxis[5] };
            var vectorAxis3 = new double[3] { beamAxis[6], beamAxis[7], beamAxis[8] };

            var longitudinalAngle = Math.Abs(VectorService.AngleBetweenTwoVectors(windLoadVector, vectorAxis3));
            if (Math.Abs(longitudinalAngle - 180) > 180 - tolerance || Math.Abs(longitudinalAngle - 180) < tolerance)
                continue;

            var theta = longitudinalAngle > 90 ? longitudinalAngle - 90 : 90 - longitudinalAngle;

            var factoredPressure = shapeFactor * convertedPressure * Math.Abs(Math.Cos(theta.DegreesToRadians()));

            //Project global vector onto the plane vector for a plane representing the principal 1 direction of the beam
            var projectionVectorG1 = VectorService.AngleBetweenTwoVectors(windLoadVector, vectorAxis1);
            var orientation1 = VectorService.VectorOrientation(vectorAxis1, windLoadVector);
            var windPressure1 = orientation1 * factoredPressure * Math.Cos(projectionVectorG1.DegreesToRadians());


            var projectionVectorG2 = VectorService.AngleBetweenTwoVectors(windLoadVector, vectorAxis2);
            var orientation2 = VectorService.VectorOrientation(vectorAxis2, windLoadVector);
            var windPressure2 = orientation2 * factoredPressure * Math.Cos(projectionVectorG2.DegreesToRadians());

            var appliedPressure1 = new double[6];
            appliedPressure1[0] = windPressure1 * beam.Section.B1 / unitFactor.Length;

            var appliedPressure2 = new double[6];
            appliedPressure2[0] = windPressure2 * beam.Section.D / unitFactor.Length;


            St7.St7SetBeamDistributedForcePrincipal6ID(modelId, beam.Number, 1, loadCase, St7.dlConstant, 1, appliedPressure1).ThrowIfFails();
            St7.St7SetBeamDistributedForcePrincipal6ID(modelId, beam.Number, 2, loadCase, St7.dlConstant, 1, appliedPressure2).ThrowIfFails();
        }

        St7.St7SaveFile(modelId).ThrowIfFails();
    }

    public List<LoadCase> GetPrimaryLoadCases(int modelId)
    {
        int numCases = 0;
        St7.St7GetNumLoadCase(modelId, ref numCases).ThrowIfFails();
        var loadCases = new List<LoadCase>();
        for (int i = 1; i <= numCases; i++)
        {
            var caseName = new StringBuilder(St7.kMaxStrLen);
            St7.St7GetLoadCaseName(modelId, i, caseName, caseName.Capacity).ThrowIfFails();
            if (!string.IsNullOrWhiteSpace(caseName.ToString()))
                loadCases.Add(new LoadCase(caseName.ToString(), i));
        }
        return loadCases;
    }

    public int GetNumberOfPrimaryLoadCases(int modelId)
    {
        int numCases = 0;
        St7.St7GetNumLoadCase(modelId, ref numCases).ThrowIfFails();
        return numCases;
    }

    public int GetNumberOfLSALoadCaseCombinations(int modelId)
    {
        int numCases = 0;
        St7.St7GetNumLSACombinations(modelId, ref numCases).ThrowIfFails();
        return numCases;
    }

    public double GetBucklingFactor(int modelId, int modeNumber)
    {
        double bucklingFactor = 0;
        St7.St7GetBucklingFactor(modelId, modeNumber, ref bucklingFactor).ThrowIfFails();
        return bucklingFactor;
    }

    private static double GetSectionShapeFactor(SectionType sectionType, WindLoadingModel windLoadingModel)
    {
        return sectionType switch
        {
            SectionType.IorH => windLoadingModel.SharpEdgeFactor,
            SectionType.LipChannel => windLoadingModel.SharpEdgeFactor,
            SectionType.Angle => windLoadingModel.SharpEdgeFactor,
            SectionType.CircularHollow => windLoadingModel.CircularSectionFactor,
            SectionType.RectangularHollow => windLoadingModel.RectangularSectionFactor,
            SectionType.T => windLoadingModel.SharpEdgeFactor,
            SectionType.Unknown => 2,
            _ => throw new NotImplementedException(),
        };
    }

    private static void HideNonBeamEntities(int modelId)
    {
        St7.St7HideEntity(modelId, St7.tyNODE).ThrowIfFails();
        St7.St7HideEntity(modelId, St7.tyPLATE).ThrowIfFails();
        St7.St7HideEntity(modelId, St7.tyBRICK).ThrowIfFails();
        St7.St7HideEntity(modelId, St7.tyLINK).ThrowIfFails();
        St7.St7HideEntity(modelId, St7.tyVERTEX).ThrowIfFails();
        St7.St7HideEntity(modelId, St7.tyLOADPATH).ThrowIfFails();
    }

    private static void InitializeResultModelWindow(int modelId, nint handle)
    {
        St7.St7SetFreeNodes(modelId, St7.nsFreeNodeNone).ThrowIfFails();
        St7.St7CreateModelWindow(modelId).ThrowIfFails();
        St7.St7SetModelWindowParent(modelId, handle).ThrowIfFails();
        St7.St7ShowModelWindow(modelId).ThrowIfFails();

        St7.St7EnableWindowViewChanges(modelId).ThrowIfFails();
        St7.St7EnableWindowEntityInspector(modelId).ThrowIfFails();
        St7.St7HideWindowCaption(modelId).ThrowIfFails();
        St7.St7ShowWindowViewToolbar(modelId).ThrowIfFails();
        St7.St7ShowWindowShowHideToolbar(modelId).ThrowIfFails();
        St7.St7ShowWindowCombos(modelId).ThrowIfFails();
        St7.St7HideWindowResultsToolbar(modelId).ThrowIfFails();
        St7.St7ShowWindowEntityPanel(modelId).ThrowIfFails();
        St7.St7ShowWindowSelectionToolbar(modelId).ThrowIfFails();
        St7.St7ShowWindowStatusBar(modelId).ThrowIfFails();
    }

    private static List<Beam> GetConnectedBeamsByZoomLevel(Beam focusedBeam, ZoomLevel zoomLevel, IEnumerable<Beam> beams)
    {
        var connectedBeams = new List<Beam>();

        switch (zoomLevel)
        {
            case ZoomLevel.Level0:
                connectedBeams.Add(focusedBeam);
                break;
            case ZoomLevel.Level1:
                foreach (var beam in focusedBeam.BeamChain.LongestChain)
                    connectedBeams.Add(beam);
                break;
            case ZoomLevel.Level2:
                connectedBeams.AddRange(GetConnectedBeams(focusedBeam, beams));
                break;
            case ZoomLevel.Level3:
                {
                    var level1ConnectedBeams = GetConnectedBeams(focusedBeam, beams);
                    foreach (var beam in level1ConnectedBeams)
                        connectedBeams.AddRange(GetConnectedBeams(beam, beams));
                }
                break;
            case ZoomLevel.Level4:
                {
                    var level1ConnectedBeams = GetConnectedBeams(focusedBeam, beams);
                    foreach (var beam1 in level1ConnectedBeams)
                    {
                        var level2ConnectedBeams = GetConnectedBeams(beam1, beams);
                        foreach (var beam2 in level2ConnectedBeams)
                            connectedBeams.AddRange(GetConnectedBeams(beam2, beams));
                    }
                }
                break;
            case ZoomLevel.Level5:
                {
                    var level1ConnectedBeams = GetConnectedBeams(focusedBeam, beams);
                    foreach (var beam1 in level1ConnectedBeams)
                    {
                        var level2ConnectedBeams = GetConnectedBeams(beam1, beams);
                        foreach (var beam2 in level2ConnectedBeams)
                        {
                            var level3ConnectedBeams = GetConnectedBeams(beam2, beams);
                            foreach (var beam3 in level3ConnectedBeams)
                                connectedBeams.AddRange(GetConnectedBeams(beam3, beams));
                        }
                    }
                }
                break;
            default:
                break;
        }

        return connectedBeams.Distinct().ToList();
    }

    private static List<Beam> GetConnectedBeams(Beam focusedBeam, IEnumerable<Beam> beams)
    {
        var connectedBeams = new List<Beam>();
        foreach (var beam in focusedBeam.BeamChain.LongestChain)
        {
            var connected = beam.GetConnectedBeams(beams);
            if (connected != null)
                connectedBeams.AddRange(connected);
        }
        return connectedBeams.Distinct().ToList();
    }

    public void UpdateFemFile(int modelId, nint handle)
    {
        St7.St7SetModelWindowParent(modelId, handle).ThrowIfFails();
        St7.St7ClearModelWindow(modelId).ThrowIfFails();
        St7.St7RedrawModel(modelId, St7.btTrue).ThrowIfFails();
        St7.St7RefreshWindowStatusBar(modelId).ThrowIfFails();
    }
    public void UpdateBeamFemFile(int modelId, nint handle)
    {
        St7.St7SetModelWindowParent(modelId, handle).ThrowIfFails();
        St7.St7ClearModelWindow(modelId).ThrowIfFails();
        St7.St7RedrawModel(modelId, St7.btTrue).ThrowIfFails();
        St7.St7RefreshWindowStatusBar(modelId).ThrowIfFails();
    }
    public bool SetSelectedBeams(int modelId, IEnumerable<Beam> beams)
    {
        var count = 0;
        St7.St7GetEntitySelectCount(modelId, St7.tyBEAM, ref count).ThrowIfFails();
        var selectionChanged = false;

        foreach (var beam in beams)
        {
            byte isSelected = 0;
            St7.St7GetEntitySelectState(modelId, St7.tyBEAM, beam.Number, 0, ref isSelected).ThrowIfFails();
            var isSelectedBool = isSelected == 1;
            if (beam.IsSelected != isSelectedBool)
            {
                beam.IsSelected = isSelectedBool;
                selectionChanged = true;
            }
        }

        return selectionChanged;
    }
    public async Task DisplayFemDesignResults(int modelId, IEnumerable<UlsResultPeak> results)
    {
        var visibleResults = new List<UlsResultPeak>();
        var maxResults = GetMaxForEachBeamId(results);
        foreach (var result in maxResults)
        {
            byte visible = St7.btFalse;
            St7.St7GetEntityNumVisibility(modelId, St7.tyBEAM, result.BeamId, ref visible).ThrowIfFails();
            if (visible == St7.btTrue)
                visibleResults.Add(result);
        }

        DisplayContour(modelId, await _contourFileService.GenerateResultsContourFile(visibleResults));
    }

    public async Task DisplayDeflectionContours(int modelId, double minDeflectionRatio, DeflectionAxis deflectionAxis, IEnumerable<DeflectionResult>? results)
    {
        DisplayContour(modelId, await _contourFileService.GenerateSlsResultsContourFile(results?.ToList(), deflectionAxis));
        double[] limits = [minDeflectionRatio, 1000D];
        var integers = new int[5];
        integers[St7.ipContourLimit] = St7.clUserRange;
        integers[St7.ipContourMode] = St7.cmContinuous;
        integers[St7.ipNumContours] = 0;
        integers[St7.ipSetMinLimit] = St7.btTrue;
        integers[St7.ipSetMaxLimit] = St7.btFalse;
        St7.St7SetEntityContourSettingsLimits(modelId, St7.tyBEAM, integers, limits).ThrowIfFails();
    }
    private static List<UlsResultPeak> GetMaxForEachBeamId(IEnumerable<UlsResultPeak> results)
    {
        var grouped = results.GroupBy(x => x.BeamId).ToList();
        var maxResults = new List<UlsResultPeak>();
        foreach (var group in grouped)
        {
            var max = group.Max(x => x.Utilization);
            var matched = group.FirstOrDefault(gr => gr.Utilization == max);
            if (matched != null)
                maxResults.Add(matched);
        }

        return maxResults;
    }

    public StrandResultFile OpenFemResultsFile(int modelId, string fileName, SolverType solverType, bool closeFirst = false)
    {
        var strandResultFile = new StrandResultFile();
        if (!_connectionService.IsApiConnected)
            return strandResultFile;

        var iNumPrimary = 0;
        var iNumSecondary = 0;

        //Close previous results file - This allows multiple model tests
        if (closeFirst)
            _ = St7.St7CloseResultFile(modelId).HandleApiError();

        //Assign result file name
        string sResultFileName;
        switch (solverType)
        {
            case SolverType.LSA:
                {
                    sResultFileName = fileName[..fileName.LastIndexOf('.')] + ".LSA";

                    //Open result file
                    St7.St7OpenResultFile(modelId, sResultFileName, "", St7.btTrue, ref iNumPrimary, ref iNumSecondary).ThrowIfFails();

                    strandResultFile.NumberOfLoadCases = iNumSecondary;
                    strandResultFile.KStart = iNumPrimary + 1;
                    strandResultFile.KEnd = iNumSecondary + iNumPrimary;
                    break;
                }
            case SolverType.NLA:
                {
                    sResultFileName = fileName[..fileName.LastIndexOf('.')] + ".NLA";

                    //Open result file
                    St7.St7OpenResultFile(modelId, sResultFileName, "", St7.btTrue, ref iNumPrimary, ref iNumSecondary).ThrowIfFails();

                    strandResultFile.NumberOfLoadCases = iNumPrimary;
                    strandResultFile.KStart = 1;
                    strandResultFile.KEnd = iNumPrimary;
                    break;
                }
            case SolverType.LBA:
                {
                    sResultFileName = fileName[..fileName.LastIndexOf('.')] + ".LBA";

                    //Open result file
                    St7.St7OpenResultFile(modelId, sResultFileName, "", St7.btTrue, ref iNumPrimary, ref iNumSecondary).ThrowIfFails();

                    strandResultFile.NumberOfLoadCases = iNumPrimary;
                    strandResultFile.KStart = 1;
                    strandResultFile.KEnd = iNumPrimary;
                    break;
                }
            default:
                {
                    St7.St7CloseFile(modelId).ThrowIfFails();
                    break;
                }
        }

        strandResultFile.INumPrimary = iNumPrimary;
        strandResultFile.INumSecondary = iNumSecondary;
        return strandResultFile;
    }
    public void CloseAllFemFiles(int modelId)
    {
        _ = St7.St7CloseResultFile(modelId).HandleApiError();
        _ = St7.St7CloseFile(modelId).HandleApiError();
    }
    public void CloseFemResultsFile(int modelId)
    {
        _ = St7.St7CloseResultFile(modelId).HandleApiError();
    }
    public void GetFemModelParameters(IFemModelParameters femModelParameters, DesignCode designCode, int modelId, SolverType solverType, StrandResultFile strandResultFile)
    {
        var unitFactor = DetermineUnitFactors.GetModelUnitFactors(modelId);
        var beamProperties = GetFemBeamSections(modelId, unitFactor, designCode);

        femModelParameters.IsInitialized = true;
        femModelParameters.BeamProperties.SetRange(beamProperties);
        femModelParameters.LoadCaseCombinations.SetRange(GetFemModelLoadCaseCombinations(modelId, solverType, strandResultFile));
        femModelParameters.Beams.SetRange(GetBeamLengths(modelId, unitFactor, beamProperties));
        femModelParameters.UnitFactor = unitFactor;

        St7.St7SetBeamResultPosMode(modelId, St7.bpParam).ThrowIfFails();
    }
    public List<LoadCaseCombination> GetFemModelLoadCaseCombinations(int modelId, SolverType solverType, StrandResultFile strandResultFile)
    {
        return GetLoadCaseCombinations(modelId, solverType, strandResultFile.INumPrimary, strandResultFile.INumSecondary);
    }
    public List<Section> GetFemBeamSections(int modelId, UnitFactor unitFactor, DesignCode designCode)
    {
        var lastProperty = new int[St7.kMaxEntityTotals];
        var numProperties = new int[St7.kMaxEntityTotals];

        St7.St7GetTotalProperties(modelId, numProperties, lastProperty).ThrowIfFails();

        var beamProperties = new List<Section>();

        for (var i = 1; i <= numProperties[St7.ipBeamPropTotal]; i++)
        {
            var beamName = new StringBuilder(0, 100) { Capacity = 100 };

            // Get property index
            var propNum = 0;
            St7.St7GetPropertyNumByIndex(modelId, St7.ptBEAMPROP, i, ref propNum).ThrowIfFails();

            // Check if St7 Beam is Designable
            var integers = new int[5];
            var sectionData = new double[St7.kNumBeamSectionData];
            var materialData = new double[St7.kNumMaterialData];
            St7.St7GetBeamPropertyData(modelId, propNum, integers, sectionData, materialData).ThrowIfFails();
            St7.St7GetPropertyName(modelId, St7.ptBEAMPROP, propNum, beamName, 100).ThrowIfFails();

            var sectionDesignable = integers[St7.ipBeamPropSectionType] switch
            {
                St7.bsISection => true,
                St7.bsLipChannel => true,
                St7.bsLSection => true,
                St7.bsCircularHollow => true,
                St7.bsSquareHollow => true,
                St7.bsTSection => true,
                //St7.bsBXSSection => true,
                _ => false
            };

            //if (sectionDesignable)
            //{
            //    int[] integ = new int[100];
            //    double[] dubs = new double[39];
            //    var named = new StringBuilder(0, 100) { Capacity = 100 };
            //    int shape = 0;
            //    St7.St7GetLibraryBeamSectionGeometryBGL(modelId, propNum, St7.luMILLIMETRE, named, int.MaxValue, ref shape, dubs);

            //    sectionDesignable = shape switch
            //    {
            //        St7.bgNullSection => false,
            //        St7.bgRectangularHollow => true,
            //        St7.bgISection => true,
            //        St7.bgChannel => true,
            //        St7.bgTSection => true,
            //        St7.bgAngle => true,
            //        St7.bgBulbFlat => false,
            //        _ => false
            //    };

            //    var name = new StringBuilder(0, 100) { Capacity = 100 };
            //    double[] dubsd = new double[St7.kNumBeamSectionData];
            //    St7.St7GetLibraryBeamSectionPropertyDataBGL(modelId, propNum, St7.luMILLIMETRE, name, int.MaxValue, dubsd);
            //}
            if (!sectionDesignable)
                continue;

            var beamPropertyChecked = integers[St7.ipBeamPropBeamType] == St7.btBeam && integers[St7.ipBeamPropMirrorType] == St7.mtNone && !string.IsNullOrWhiteSpace(beamName?.ToString());
            var sectiontype = SectionTypeHelper.SectionTypeFromStrand(integers[St7.ipBeamPropSectionType]);


            var section = _femDesignAdapter.GetBeamPropertiesService(designCode).GetBeamSection(beamName?.ToString(), sectiontype, beamPropertyChecked, materialData, sectionData, unitFactor, propNum);

            beamProperties.Add(section);
        }
        return beamProperties;
    }
    private static List<LoadCaseCombination> GetLoadCaseCombinations(int modelId, SolverType solverType, int iNumPrimary, int iNumSecondary)
    {
        var loadCaseCombinations = new List<LoadCaseCombination>();

        if (solverType == SolverType.LSA)
        {
            for (var i = 1; i <= iNumSecondary; i++)
            {
                var tempStringLc = new StringBuilder(0, 100) { Capacity = 100 };
                St7.St7GetLSACombinationName(modelId, i, tempStringLc, 100).ThrowIfFails();
                loadCaseCombinations.Add(new LoadCaseCombination() { Number = i + iNumPrimary, Name = tempStringLc.ToString() });
            }
        }
        else
        {
            for (var i = 1; i <= iNumPrimary; i++)
            {
                loadCaseCombinations.Add(new LoadCaseCombination() { Number = i, Name = "Increment " + i.ToString() });
            }
        }

        return loadCaseCombinations ?? [];
    }
    private static List<Beam> GetBeamLengths(int modelId, UnitFactor unitFactor, List<Section> beamProperties)
    {
        var beamTotalCount = 0;
        St7.St7GetTotal(modelId, St7.tyBEAM, ref beamTotalCount).ThrowIfFails();
        var beamLengths = new List<Beam>();

        for (var i = 1; i <= beamTotalCount; i++)
        {
            var propNumber = 0;
            var beamLength = 0D;

            // Get Beam properties
            St7.St7GetElementProperty(modelId, St7.tyBEAM, i, ref propNumber).ThrowIfFails();

            // Get Beam Length
            St7.St7GetElementData(modelId, St7.tyBEAM, i, 0, ref beamLength).ThrowIfFails();

            // Get beam nodes
            var beamNodes = new int[St7.kMaxElementNode];
            St7.St7GetElementConnection(modelId, St7.tyBEAM, i, beamNodes).ThrowIfFails();

            var nodeToNodeLength = Math.Round(beamLength * unitFactor.Length, 2); // TODO: Requires more work for AS Code Section 6.3.2 as L_e is effective length with Ke already factored in.

            beamLengths.Add(new Beam()
            {
                Number = i,
                Section = beamProperties.First(bp => bp.Number == propNumber),
                BeamL2 = nodeToNodeLength,
                BeamL1 = nodeToNodeLength,
                BeamLz = nodeToNodeLength,
                BeamLeTop = nodeToNodeLength,
                BeamLeBottom = nodeToNodeLength,
                Node1 = beamNodes[1],
                Node2 = beamNodes[2]
            });
        }
        return beamLengths;
    }

    public async Task DisplayDesignLengths(int modelId, BeamAxis beamAxisEnum, IEnumerable<Beam> beams, double lengthFactor)
    {
        var visibleBeams = new List<Beam>();
        foreach (var beam in beams)
        {
            byte visible = 1;
            St7.St7GetEntityNumVisibility(modelId, St7.tyBEAM, beam.Number, ref visible).ThrowIfFails();
            if (visible != 0)
                visibleBeams.Add(beam);
        }

        switch (beamAxisEnum)
        {
            case BeamAxis.Principal1:
                DisplayContour(modelId, await _contourFileService.GenerateL1ContourFile(visibleBeams, lengthFactor));
                break;
            case BeamAxis.Principal2:
                DisplayContour(modelId, await _contourFileService.GenerateL2ContourFile(visibleBeams, lengthFactor));
                break;
            case BeamAxis.PrincipalZ:
                DisplayContour(modelId, await _contourFileService.GenerateLzContourFile(visibleBeams, lengthFactor));
                break;
            case BeamAxis.PrincipalETop:
                DisplayContour(modelId, await _contourFileService.GenerateLeTopContourFile(visibleBeams, lengthFactor));
                break;
            case BeamAxis.PrincipalEBottom:
                DisplayContour(modelId, await _contourFileService.GenerateLeBottomContourFile(visibleBeams, lengthFactor));
                break;
            case BeamAxis.All:
                break;
        }

        HideNonBeamEntities(modelId);
    }

    public async Task DisplayDesignSlenderness(int modelId, BeamAxis beamAxisEnum, IEnumerable<Beam> beams, double lengthFactor)
    {
        var visibleBeams = new List<Beam>();
        foreach (var beam in beams)
        {
            byte visible = 1;
            St7.St7GetEntityNumVisibility(modelId, St7.tyBEAM, beam.Number, ref visible).ThrowIfFails();
            if (visible != 0)
                visibleBeams.Add(beam);
        }

        switch (beamAxisEnum)
        {
            case BeamAxis.Principal1:
                DisplayContour(modelId, await _contourFileService.GenerateL1R1ContourFile(visibleBeams, lengthFactor));
                break;
            case BeamAxis.Principal2:
                DisplayContour(modelId, await _contourFileService.GenerateL2R2ContourFile(visibleBeams, lengthFactor));
                break;
            case BeamAxis.PrincipalZ:
                break;
            case BeamAxis.All:
                break;
        }

        HideNonBeamEntities(modelId);
    }

    private static void DisplayContour(int modelId, string fileName)
    {
        St7.St7SetEntityContourFile(modelId, St7.tyBEAM, St7.ucElement, fileName).ThrowIfFails();
    }

    public List<StrandBeamResults> GetBeamResults(int modelId, ResultType resultType, IEnumerable<Beam> beams, IEnumerable<LoadCaseCombination> loadCaseCombinations, int minStations)
    {
        var st7Results = new List<StrandBeamResults>();
        foreach (var combination in loadCaseCombinations)
        {
            if (!combination.Include)
                continue;

            foreach (var beam in beams)
            {
                var numStations = 1;
                var numColumns = 1;
                var beamPos = new double[St7.kMaxBeamResult];
                var beamRes = new double[St7.kMaxBeamResult];
                var stressRes = new double[St7.kMaxBeamResult];
                var quarterPoints = new double[St7.kMaxBeamResult];

                switch (resultType)
                {
                    case ResultType.Force:
                        {
                            St7.St7GetBeamResultArray(modelId, St7.rtBeamForce, St7.stBeamPrincipal, beam.Number, minStations, combination.Number,
                                ref numStations, ref numColumns, beamPos, beamRes).ThrowIfFails();

                            stressRes = GetStressResults(modelId, beam.Number, combination.Number);

                            // Quarter Points
                            St7.St7SetBeamResultPosMode(modelId, St7.bpParam).ThrowIfFails();
                            St7.St7GetBeamResultArrayPos(modelId, St7.rtBeamForce, St7.stBeamPrincipal, beam.Number, combination.Number, 3, [0.25, 0.5, 0.75], ref numColumns, quarterPoints).ThrowIfFails();
                        }
                        break;
                    case ResultType.Deflection:
                        St7.St7GetBeamResultArray(modelId, St7.rtBeamDisp, St7.stBeamGlobal, beam.Number, minStations, combination.Number,
                            ref numStations, ref numColumns, beamPos, beamRes).ThrowIfFails();

                        // Quarter Points
                        St7.St7SetBeamResultPosMode(modelId, St7.bpParam).ThrowIfFails();
                        St7.St7GetBeamResultArrayPos(modelId, St7.rtBeamDisp, St7.stBeamGlobal, beam.Number, combination.Number, 3, [0.25, 0.5, 0.75], ref numColumns, quarterPoints).ThrowIfFails();
                        break;
                    default:
                        break;
                }

                st7Results.Add(new StrandBeamResults()
                {
                    Beam = beam,
                    LoadCaseId = combination.Number,
                    NumStations = numStations,
                    NumColumns = numColumns,
                    BeamRes = beamRes,
                    BeamStressRes = stressRes,
                    BeamQuarters = quarterPoints
                });
            }
        }
        return st7Results;
    }

    private static double[] GetStressResults(int modelId, int beamNumber, int combination)
    {
        var beamPos = new double[St7.kMaxBeamResult];
        var stressRes = new double[St7.kMaxBeamResult];

        var stations = 3;
        var numStations = 0;
        var numColumns = 0;

        St7.St7GetBeamResultArray(modelId, St7.rtBeamCombinedStress, St7.stBeamPrincipal, beamNumber, stations, combination, ref numStations, ref numColumns, beamPos, stressRes).ThrowIfFails();

        var beamStressResults = new double[numStations];

        for (int i = 1; i <= numStations; i++)
        {
            var beamResult = stressRes[(i - 1) * numColumns + St7.ipVonMisesStress];
            beamStressResults[i - 1] = beamResult;
        }

        return beamStressResults;
    }


}
