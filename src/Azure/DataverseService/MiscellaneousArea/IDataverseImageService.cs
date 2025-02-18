namespace DataverseService.UtilityArea;

public interface IDataverseImageService
{
    string SetImageAttributeAsBase64(Guid entityId, string entityName, string attributeName);
}