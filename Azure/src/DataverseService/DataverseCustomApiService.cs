using DataverseService.Foundation.Dao;
using Microsoft.Xrm.Sdk;
using Newtonsoft.Json;
using SharedDomain.EconomyArea;

namespace DataverseService;

public class DataverseCustomApiService : IDataverseCustomApiService
{
    private readonly IDataverseAccessObjectAsync dao;

    public DataverseCustomApiService(IDataverseAccessObjectAsync dao)
    {
        this.dao = dao;
    }

    public Task CreateTransactions(CreateTransactionsRequest request) => dao.ExecuteAsync(new OrganizationRequest("mgs_CreateTransactions")
    {
        Parameters = new ParameterCollection()
        {
            new KeyValuePair<string, object>("Payload", JsonConvert.SerializeObject(request)),
        },
    });
}