using DataverseService.Foundation.Dao;

namespace DataverseService;

public class DataverseCustomApiService : IDataverseCustomApiService
{
#pragma warning disable S4487 // Unread private field
    private readonly IDataverseAccessObjectAsync dao;
#pragma warning restore S4487 // Unread private field

    public DataverseCustomApiService(IDataverseAccessObjectAsync dao)
    {
        this.dao = dao;
    }

    // Add your custom API method implementations here
}