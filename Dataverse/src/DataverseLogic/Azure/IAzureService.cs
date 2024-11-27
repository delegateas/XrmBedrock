using SharedDomain.CustomerArea;

namespace DataverseLogic.Azure;

public interface IAzureService
{
    void SendDemoAccountMessage(DemoAccountMessage message);
}