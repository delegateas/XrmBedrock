using DataverseService.Foundation.Dao;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using OneOf;
using OneOf.Types;

namespace DataverseService.UtilityArea;

public class DataverseImageService
{
    private readonly IDataverseAccessObjectAsync adminDao;

    public DataverseImageService(IDataverseAccessObjectAsync adminDao)
    {
        this.adminDao = adminDao;
    }

    public OneOf<string, NotFound> SetImageAttributeAsBase64(Guid entityId, string entityName, string attributeName)
    {
        var entity = GetEntity(entityId, entityName, attributeName);

        if (entity.Attributes.TryGetValue(attributeName, out var imageObject) && imageObject is byte[] imageBytes)
            return Convert.ToBase64String(imageBytes);

        return default(NotFound);
    }

    private Entity GetEntity(Guid entityId, string entityName, string attributeName)
    {
        var retrieveRequest = new RetrieveRequest
        {
            Target = new EntityReference(entityName, entityId),
            ColumnSet = new ColumnSet(attributeName),
        };

        var response = (RetrieveResponse)adminDao.Execute(retrieveRequest);
        return response.Entity;
    }
}
