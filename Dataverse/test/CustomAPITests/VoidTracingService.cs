using SharedDataverseLogic;

namespace LF.Medlemssystem.CustomAPITests;

public class VoidTracingService : IExtendedTracingService
{
    public void Trace(string format, params object[] args)
    {
    }
}