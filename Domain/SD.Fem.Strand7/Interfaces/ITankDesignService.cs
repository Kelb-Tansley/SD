using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SD.Fem.Strand7.Interfaces;

public interface ITankDesignService
{
    Task BuildCircularTankModel(int modelId, double diameter, List<HeightSegment> heightSegments, double meshSize, int plateNodeCount, double roofThickness, double baseThickness, string fileName);
}
