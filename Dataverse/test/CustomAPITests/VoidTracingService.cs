using SharedDataverseLogic;

namespace Dataverse.CustomAPITests;

public class VoidTracingService : IExtendedTracingService
{
    public void Trace(string format, params object[] args)
    {
    }
}