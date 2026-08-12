using Microsoft.Extensions.DependencyInjection;
using SD.Tests.Shared.Helpers;
using Reqnroll.Microsoft.Extensions.DependencyInjection;

namespace SD.Tests.Sans.Support;
public class Startup
{
    [ScenarioDependencies]
    public static IServiceCollection CreateServices()
    {
        return ReqnrollStartupHelper.StartupReqnrollTests();
    }
}
