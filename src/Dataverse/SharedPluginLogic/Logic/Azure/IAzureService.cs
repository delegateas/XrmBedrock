using SharedDomain.EconomyArea;

namespace DataverseLogic.Azure;

public interface IAzureService
{
    void SendCreateInvoicesMessage(CreateInvoicesMessage message);
}