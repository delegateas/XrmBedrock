using DataverseService.Foundation.Dao;
using DataverseService.Foundation.Exception;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;

namespace DataverseService.UtilityArea;

public class DataverseImageService : IDataverseImageService
{
    private readonly IDataverseAccessObjectAsync adminDao;

    public DataverseImageService(IDataverseAccessObjectAsync adminDao)
    {
        this.adminDao = adminDao;
    }

    public string SetImageAttributeAsBase64(Guid entityId, string entityName, string attributeName)
    {
        var entity = GetEntity(entityId, entityName, attributeName);

        if (entity.Attributes.TryGetValue(attributeName, out var imageObject) && imageObject is byte[] imageBytes)
        {
            return Convert.ToBase64String(imageBytes);
        }

        throw new CouldNotFetchProfilePictureException($"Failed to fetch profile picture for member {entityId}");
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