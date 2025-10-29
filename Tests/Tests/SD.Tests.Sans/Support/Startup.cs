using Microsoft.Extensions.DependencyInjection;
using SD.Tests.Shared.Helpers;
using SolidToken.SpecFlow.DependencyInjection;

namespace SD.Tests.Sans.Support;
public class Startup
{
    [ScenarioDependencies]
    public static IServiceCollection CreateServices()
    {
        return SpecFlowStartupHelper.StartupSpecflowTests();
    }
}