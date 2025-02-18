using SharedDomain.EconomyArea;

namespace DataverseService;

public interface IDataverseCustomApiService
{
    Task CreateTransactions(CreateTransactionsRequest request);
}