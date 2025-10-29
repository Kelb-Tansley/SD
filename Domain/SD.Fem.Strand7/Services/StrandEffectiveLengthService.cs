using SD.Element.Design.Services;

namespace SD.Fem.Strand7.Services;
public class StrandEffectiveLengthService(IBeamChainService beamChainService) : IEffectiveLengthService
{
    private readonly IBeamChainService _beamChainService = beamChainService;

    public void CalculateDesignLengths(int modelId,
                                       bool designLengthCalculated,
                                       IFemModelParameters femModelParameters,
                                       BeamDesignSettings sansDesignSettings)
    {
        if (designLengthCalculated)
            CalculateEffectiveLengths(modelId, femModelParameters.Beams, sansDesignSettings);
        else
            SetDesignLengthsToDefault(femModelParameters.Beams);

        SetChainDetails(modelId, femModelParameters.Beams);
        SetChainDesignLengths(femModelParameters.Beams);
    }

    private static void SetDesignLengthsToDefault(IEnumerable<Beam> beams)
    {
        foreach (var beam in beams)
        {
            beam.ResetToDefault();
        }
    }

    private static void SetChainDetails(int modelId, IEnumerable<Beam> beams)
    {
        var beamChains = beams.Select(b => b.BeamChain)?.Distinct()?.ToList();
        if (beamChains == null || beamChains.Count == 0)
            return;
        foreach (var beamChain in beamChains)
            BeamChainHelper.SetBeamChainEndDetails(modelId, beamChain);
    }

    private static void SetChainDesignLengths(IEnumerable<Beam> beams)
    {
        foreach (var beam in beams)
        {
            beam.BeamChain.SetLengths();
            beam.BeamChain.SetConnectedChains();
        }
    }

    private void CalculateEffectiveLengths(int modelId, IEnumerable<Beam> beams, BeamDesignSettings designSettings)
    {
        FindBeamsNeighbours(modelId, beams, designSettings);

        _beamChainService.GenerateBeamChains([.. beams]);
    }

    private static void FindBeamsNeighbours(int modelId, IEnumerable<Beam> beams, BeamDesignSettings designSettings)
    {
        // We look for 0, 1 or 2 connected beams for each beam as its 'Neighbour'
        foreach (var beam in beams)
        {
            // Get the released ends of the main beam, if both released then skip to next beam
            var releasedEnds = GetBeamsReleasedEnds(modelId, beam);
            if (releasedEnds == null)
                continue;

            var beamAxisVectors = new double[9]; // Unit vectors defining directions 1, 2 and 3 of the beam
            St7.St7GetBeamAxisSystemInitial(modelId, beam.Number, beamAxisVectors).ThrowIfFails();

            var connectedBeams = beam.GetConnectedBeams(beams);

            beam.BeamChain.ResetToPrimaryBeam(beam);

            foreach (var conBeam in connectedBeams)
            {
                if (conBeam.Number == beam.Number)
                    continue;

                var beamConnected = AreBeamsConnected(modelId, beam, conBeam, releasedEnds, connectedBeams, beamAxisVectors, designSettings);
                if (beamConnected.AllDisconnected)
                    continue;

                AddConnectedBeamToChain(beam, conBeam, beamConnected);
            }
        }
    }

    private static void AddConnectedBeamToChain(Beam beam, Beam conBeam, BeamConnection beamConnected)
    {
        if (!beamConnected.Principal1Disconnected)
            beam.BeamChain.L1Beams.Add(conBeam);

        if (!beamConnected.Principal2Disconnected)
            beam.BeamChain.L2Beams.Add(conBeam);

        if (!beamConnected.PrincipalZDisconnected)
            beam.BeamChain.LzBeams.Add(conBeam);

        if (!beamConnected.PrincipalETopDisconnected)
            beam.BeamChain.LeTopBeams.Add(conBeam);

        if (!beamConnected.PrincipalEBottomDisconnected)
            beam.BeamChain.LeBottomBeams.Add(conBeam);
    }

    private static List<BeamReleasedEnds>? GetBeamsReleasedEnds(int modelId, Beam beam)
    {
        //Returns null when both ends of the beam are released, taking axis into account
        var releasedEnds = new List<BeamReleasedEnds>();

        var re1 = GetRotationReleasedEnds(modelId, beam.Number, 1);
        var re2 = GetRotationReleasedEnds(modelId, beam.Number, 2);
        var re3 = GetRotationReleasedEnds(modelId, beam.Number, 3);
        if ((re1 == null || re1.Count > 1) && (re2 == null || re2.Count > 1) && (re3 == null || re3.Count > 1))
            return null; //If both ends of the beam are released, in all axis, then a chain will not exist

        if (re1 != null && re1.Count > 0)
        {
            foreach (var re in re1)
            {
                releasedEnds.Add(new BeamReleasedEnds()
                {
                    ReleasedEnd = re,
                    ReleasedAxis = BeamAxis.Principal1
                });
            }
        }
        if (re2 != null && re2.Count > 0)
        {
            foreach (var re in re2)
            {
                releasedEnds.Add(new BeamReleasedEnds()
                {
                    ReleasedEnd = re,
                    ReleasedAxis = BeamAxis.Principal2
                });
            }
        }
        if (re3 != null && re3.Count > 0)
        {
            foreach (var re in re3)
            {
                releasedEnds.Add(new BeamReleasedEnds()
                {
                    ReleasedEnd = re,
                    ReleasedAxis = BeamAxis.PrincipalZ
                });
            }
        }
        return releasedEnds;
    }
    private static BeamConnection AreBeamsConnected(int modelId, Beam beam, Beam subBeam, [NotNull] List<BeamReleasedEnds> releasedEnds, IEnumerable<Beam> connectedBeams, double[] beamAxis, BeamDesignSettings settings)
    {
        var beamConnection = new BeamConnection();

        //Check the elements have a common node
        var connectedEnds = GetConnectedEnds(beam, subBeam);
        if (connectedEnds.BeamEnd == 0)
            return beamConnection;

        //If both ends of the sub beam are released then a chain will not exist
        var subReleasedEnds = GetBeamsReleasedEnds(modelId, subBeam);
        if (subReleasedEnds == null)
            return beamConnection;

        //Check that the main beam is not released on the connected end
        AreReleasedEndsDisconnecting(beamConnection, releasedEnds, connectedEnds.BeamEnd);
        if (beamConnection.AllDisconnected)
            return beamConnection;

        //Check that the comparison beam is not released on the connected end
        AreReleasedEndsDisconnecting(beamConnection, subReleasedEnds, connectedEnds.ComparisonBeamEnd);
        if (beamConnection.AllDisconnected)
            return beamConnection;

        var comparisonBeamAxis = new double[9];
        St7.St7GetBeamAxisSystemInitial(modelId, subBeam.Number, comparisonBeamAxis).ThrowIfFails();

        //Check that the comparison beam is colinear to the main beam
        beamConnection.AllDisconnected = !AreBeamsColinear(beamAxis, comparisonBeamAxis, settings.BeamAllignmentAngleTolerance);
        if (beamConnection.AllDisconnected)
            return beamConnection;

        //Check that there is no twist difference between the two beams outside of the tolerance
        beamConnection.AllDisconnected = !AreBeamsRotationallyAlligned(beamAxis, comparisonBeamAxis, settings.BeamRotationAngleTolerance);
        if (beamConnection.AllDisconnected)
            return beamConnection;

        var restrainingBeams = connectedBeams.Where(bm => bm.Node1 == connectedEnds.NodeNumber || bm.Node2 == connectedEnds.NodeNumber);
        //Check that there are no lateral restraint beams breaking the unsuported length and
        //that there are no torsional restraint beams breaking the torsional length(z and e)
        AreThereRestrainingBeams(modelId, restrainingBeams, beamConnection, beamAxis, beam, subBeam, settings.BeamRestraintAngleTolerance);

        beamConnection.SetDisconnected();
        return beamConnection;
    }
    private static BeamConnectedEnds GetConnectedEnds(Beam beam, Beam subBeam)
    {
        var beamConnectedEnds = new BeamConnectedEnds
        {
            BeamEnd = beam.Node1 == subBeam.Node1
                      || beam.Node1 == subBeam.Node2 ? 1
                            : beam.Node2 == subBeam.Node1
                              || beam.Node2 == subBeam.Node2 ? 2 : 0
        };

        if (beamConnectedEnds.BeamEnd == 0)
        {
            beamConnectedEnds.ComparisonBeamEnd = 0;
            return beamConnectedEnds;
        }

        var beamEndNode = beamConnectedEnds.BeamEnd == 1 ? beam.Node1 : beam.Node2;
        beamConnectedEnds.ComparisonBeamEnd = beamEndNode == subBeam.Node1 ? 1
                            : beamEndNode == subBeam.Node2 ? 2 : 0;
        beamConnectedEnds.NodeNumber = beamEndNode;
        return beamConnectedEnds;
    }
    private static void AreReleasedEndsDisconnecting(BeamConnection beamConnection, List<BeamReleasedEnds> releasedEnds, int connectedEnd)
    {
        //Check that the connected end is not released. We must consider both axis'.
        if (releasedEnds?.Count != 0)
        {
            //A released end does exist for either axis 1 or 2
            foreach (var releasedEnd in releasedEnds)
            {
                if (connectedEnd == releasedEnd.ReleasedEnd && releasedEnd.ReleasedAxis == BeamAxis.Principal1)
                    beamConnection.Principal1Disconnected = true;

                if (connectedEnd == releasedEnd.ReleasedEnd && releasedEnd.ReleasedAxis == BeamAxis.Principal2)
                    beamConnection.Principal2Disconnected = true;

                if (connectedEnd == releasedEnd.ReleasedEnd && releasedEnd.ReleasedAxis == BeamAxis.PrincipalZ)
                {
                    beamConnection.PrincipalZDisconnected = true;
                    beamConnection.PrincipalETopDisconnected = true;
                    beamConnection.PrincipalEBottomDisconnected = true;
                }

                //If the major and minor axis are released then we disqualify the beams continuity
                beamConnection.AllDisconnected = beamConnection.Principal1Disconnected && beamConnection.Principal2Disconnected;
            }
        }
        else
            beamConnection.AllDisconnected = false;
    }
    private static bool AreBeamsColinear(double[] beamAxis, double[] comparisonBeamAxis, double longAxisTolerance)
    {
        //Compare vector i3 of main beam with vector i3 of connecting beam 
        double[] beamUnitVector = [beamAxis[6], beamAxis[7], beamAxis[8]];
        double[] compBeamUnitVector = [comparisonBeamAxis[6], comparisonBeamAxis[7], comparisonBeamAxis[8]];

        var totalAngle = VectorService.AngleBetweenTwoVectors(beamUnitVector, compBeamUnitVector);

        if (totalAngle > longAxisTolerance)
            return false;

        return true;
    }
    private static bool AreBeamsRotationallyAlligned(double[] beamAxis, double[] comparisonBeamAxis, double longAxisRotationTolerance)
    {
        //The beams cross section in the 1-2 plane, 3 is longitudinal:
        //[0..2] – A unit vector in the global XYZ system, defining the 1-direction of the beam. 
        //[3..5] – A unit vector in the global XYZ system, defining the 2 - direction of the beam.
        //[6..8] – A unit vector in the global XYZ system, defining the 3 - direction of the beam.

        //This if statement checks that the elements are orientated similarly to each other (relative rotation)
        return !(Math.Abs(comparisonBeamAxis[0]) - Math.Abs(beamAxis[0]) > longAxisRotationTolerance
            || Math.Abs(comparisonBeamAxis[1]) - Math.Abs(beamAxis[1]) > longAxisRotationTolerance
            || Math.Abs(comparisonBeamAxis[2]) - Math.Abs(beamAxis[2]) > longAxisRotationTolerance
            || Math.Abs(comparisonBeamAxis[3]) - Math.Abs(beamAxis[3]) > longAxisRotationTolerance
            || Math.Abs(comparisonBeamAxis[4]) - Math.Abs(beamAxis[4]) > longAxisRotationTolerance
            || Math.Abs(comparisonBeamAxis[5]) - Math.Abs(beamAxis[5]) > longAxisRotationTolerance);
    }
    private static void AreThereRestrainingBeams(int modelId, IEnumerable<Beam> restrainingBeams, BeamConnection beamConnection, double[] beamAxis, Beam beam, Beam subBeam, double lateralRestraintAngleTolerance)
    {
        foreach (var conBeam in restrainingBeams)
        {
            if (beamConnection.AllDisconnected)
                return;

            if (conBeam.Number == beam.Number || conBeam.Number == subBeam.Number || (!conBeam.Section.IsLateralRestraint && !conBeam.Section.IsTorsionalRestraint))
                continue;

            var comparisonBeamAxis = new double[9];
            St7.St7GetBeamAxisSystemInitial(modelId, conBeam.Number, comparisonBeamAxis).ThrowIfFails();

            CheckTieInRestraints(beamConnection, beamAxis, lateralRestraintAngleTolerance, conBeam, comparisonBeamAxis);
        }
    }

    private static void CheckTieInRestraints(BeamConnection beamConnection, double[] beamAxis, double lateralRestraintAngleTolerance, Beam conBeam, double[] tieInBeamAxis)
    {
        //We must compare two vectors for each restraint axis
        //1. The first comparison is between the longitudinal vector of the beam and the longitudinal vector of the tie in beam (i3)
        var beami3 = new double[] { beamAxis[6], beamAxis[7], beamAxis[8] };
        var tieInBeami3 = new double[] { tieInBeamAxis[6], tieInBeamAxis[7], tieInBeamAxis[8] };

        //1.1 The i3 vector of the tie-in beam needs to be projected onto the plane in which the beams i3 vector sits.
        //This plane is defined by the i2 or i1 vector of the beam. We will use the i2 vector first.
        var beami2 = new double[] { beamAxis[3], beamAxis[4], beamAxis[5] };
        var tieInBeami3OntoBeami2 = VectorService.ProjectVectorOntoPlane(tieInBeami3, beami2);

        //1.2 the angle between the tie-in beams i3 vector, projected onto plane i2, and the i3 vector of the beam should be within limits.
        var longitudinalAngle = VectorService.AngleBetweenTwoVectors(tieInBeami3OntoBeami2, beami3);
        var isLongitudinalRestraint = VectorService.AngleIsOutsideColinearLimit(longitudinalAngle, lateralRestraintAngleTolerance);

        //1.3 the angle between the tie-in beams i3 vector, projected onto plane i1, and the i3 vector of the beam should be within limits.
        var beami1 = new double[] { beamAxis[0], beamAxis[1], beamAxis[2] };
        if (!isLongitudinalRestraint)
        {
            var tieInBeami3OntoBeami1 = VectorService.ProjectVectorOntoPlane(tieInBeami3, beami1);
            longitudinalAngle = VectorService.AngleBetweenTwoVectors(tieInBeami3OntoBeami1, beami3);
            isLongitudinalRestraint = VectorService.AngleIsOutsideColinearLimit(longitudinalAngle, lateralRestraintAngleTolerance);
        }

        //2. The second comparison is between a perpendicular vector of the beam (i2 for major axis restraint) and the longitudinal vector of the tie in beam (i3)
        //i2 vector of the beam is already defined. i3 of the tie in beam is also defined already.

        //2.1 The i3 vector of the tie-in beam needs to be projected onto the plane in which the beams i2 vector sits.
        //This plane is defined by the i3 vector of the beam.
        var tieInBeami3OntoBeami3 = VectorService.ProjectVectorOntoPlane(tieInBeami3, beami3);

        //2.2 the angle between the tie-in beams i3 vector, projected onto plane, and the i2 vector of the beam should be within limits.
        var perpendicularAngle2 = VectorService.AngleBetweenTwoVectors(tieInBeami3OntoBeami3, beami2);

        var isPerpendicular2Restraint = VectorService.AngleIsOutsideColinearLimit(perpendicularAngle2, lateralRestraintAngleTolerance);

        if (isLongitudinalRestraint && isPerpendicular2Restraint)
        {
            if (conBeam.Section.IsLateralRestraint)
                beamConnection.Principal1Disconnected = true;

            if (beamConnection.Principal1Disconnected && conBeam.Section.IsTorsionalRestraint)
                beamConnection.PrincipalZDisconnected = true;

            if (beamConnection.Principal1Disconnected && conBeam.Section.IsTopFlangeRestraint)
                beamConnection.PrincipalETopDisconnected = true;

            if (beamConnection.Principal1Disconnected && conBeam.Section.IsBottomFlangeRestraint)
                beamConnection.PrincipalEBottomDisconnected = true;
        }

        //3. The third comparison is between a perpendicular vector of the beam (i1 for minor axis restraint) and the longitudinal vector of the tie in beam (i3)
        //i3 of the tie in beam is also defined already.

        //3.1 The i3 vector of the tie-in beam needs to be projected onto the plane in which the beams i1 vector sits.
        //This plane is defined by the i3 vector of the beam. This projection has already been made.

        //3.2 the angle between the tie-in beams i3 vector, projected onto plane, and the i1 vector of the beam should be within limits.
        var perpendicularAngle1 = VectorService.AngleBetweenTwoVectors(tieInBeami3OntoBeami3, beami1);

        var isPerpendicular1Restraint = VectorService.AngleIsOutsideColinearLimit(perpendicularAngle1, lateralRestraintAngleTolerance);

        if (isLongitudinalRestraint && isPerpendicular1Restraint)
        {
            if (conBeam.Section.IsLateralRestraint)
                beamConnection.Principal2Disconnected = true;

            if (beamConnection.Principal2Disconnected && conBeam.Section.IsTorsionalRestraint)
                beamConnection.PrincipalZDisconnected = true;

            if (beamConnection.Principal2Disconnected && conBeam.Section.IsTopFlangeRestraint)
                beamConnection.PrincipalETopDisconnected = true;

            if (beamConnection.Principal2Disconnected && conBeam.Section.IsBottomFlangeRestraint)
                beamConnection.PrincipalEBottomDisconnected = true;
        }
    }

    private static List<int> GetRotationReleasedEnds(int modelId, int beamNum, int axis)
    {
        var endReleasePrincipalAxis = axis == 2 ? 0 : axis == 1 ? 1 : 2;

        var endReleases = new int[3];
        var releaseStiffness = new double[3];
        var releasedEnds = new List<int>();

        //Check if there is a release on end 1
        var resp1 = St7.St7GetBeamRRelease3(modelId, beamNum, 1, endReleases, releaseStiffness).HandleApiError();
        if (resp1.IsValid)
            if (endReleases[endReleasePrincipalAxis] == St7.brReleased || endReleases[endReleasePrincipalAxis] == St7.brPartial)
                releasedEnds.Add(1);

        //Check if there is a release on end 2
        var resp2 = St7.St7GetBeamRRelease3(modelId, beamNum, 2, endReleases, releaseStiffness).HandleApiError();
        if (resp2.IsValid)
            if (endReleases[endReleasePrincipalAxis] == St7.brReleased || endReleases[endReleasePrincipalAxis] == St7.brPartial)
                releasedEnds.Add(2);

        return releasedEnds;
    }
}