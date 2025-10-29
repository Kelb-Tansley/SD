using SD.Tests.Shared.Helpers;

namespace SD.Tests.Mathcad.AS.Support;
public class Startup
{
    [ScenarioDependencies]
    public static IServiceCollection CreateServices()
    {
        return SpecFlowStartupHelper.StartupSpecflowTests();
    }
}