using DataverseService.Foundation.Dao;

namespace DataverseService.ActivityArea;

public class DataverseActivityService : IDataverseActivityService
{
    private readonly IDataverseAccessObjectAsync adminDao;

    public DataverseActivityService(IDataverseAccessObjectAsync adminDao)
    {
        this.adminDao = adminDao;
    }
}