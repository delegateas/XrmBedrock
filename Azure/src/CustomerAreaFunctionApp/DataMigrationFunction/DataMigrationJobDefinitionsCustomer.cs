using Microsoft.Xrm.Sdk.Messages;
using System.Data;
using XrmBedrock.SharedContext;

namespace CustomerAreaFunctionApp.DataMigrationFunction;

internal sealed partial class DataMigrationJobDefinitions
{
    private UpsertRequest AccountHierarchyMapper(DataRow row)
    {
        var keyAttributes = new Microsoft.Xrm.Sdk.KeyAttributeCollection();
        var organizationId = row.GetFieldValue<int>("sourceId");
        keyAttributes.Add("accountnumber", organizationId); // Do create a new column of type that matches the sourceId and make a key of it
        var account = new Account()
        {
            KeyAttributes = keyAttributes,
            ParentAccountId = dataverseMigrateGenericService.ResolveLookup<Account>(a => a.AccountNumber, row.GetFieldValue<int>("ParentSourceId")),
        };
        return new UpsertRequest() { Target = account, };
    }

    public UpsertRequest AccountMapper(DataRow row)
    {
        var keyAttributes = new Microsoft.Xrm.Sdk.KeyAttributeCollection();
        var organizationId = row.GetFieldValue<int>("sourceId");
        keyAttributes.Add("accountnumber", organizationId); // Do create a new column of type that matches the sourceId and make a key of it
        var account = new Account()
        {
            KeyAttributes = keyAttributes,
            Name = dataMigrationTestMarking + row.GetFieldValue<string>("Name"),
            OverriddenCreatedOn = row.GetFieldValue<DateTime>("CreatedDate"),
            Address1_Line1 = row.GetFieldValue<string>("Address"),
            Address1_PostalCode = row.GetFieldValue<string>("ZipCode"),
            Address1_City = row.GetFieldValue<string>("City"),
            Telephone1 = row.GetFieldValue<string>("Phone"),
            EMailAddress1 = row.GetFieldValue<string>("Email") + dataMigrationTestMarking,
        };
        return new UpsertRequest() { Target = account, };
    }

    public UpsertRequest ContactMapper(DataRow row)
    {
        var keyAttributes = new Microsoft.Xrm.Sdk.KeyAttributeCollection();
        var organizationId = GetFieldValue<int>(row, "sourceId");
        keyAttributes.Add("nickname", organizationId); // Do create a new column of type that matches the sourceId and make a key of it
        var contact = new Contact()
        {
            KeyAttributes = keyAttributes,
            OverriddenCreatedOn = row.GetFieldValue<DateTime>("CreatedDate"),
            FirstName = dataMigrationTestMarking + row.GetFieldValue<string>("FirstName"),
            LastName = row.GetFieldValue<string>("LastName"),
            MobilePhone = row.GetFieldValue<string>("MobilePhone"),
            EMailAddress1 = row.GetFieldValue<string>("Email") + dataMigrationTestMarking,
            BirthDate = row.GetFieldValue<DateTime?>("Born"),
            GenderCode = MapGender(row.GetFieldValue<string>("Gender")), // Demo comes as K, M, '' or null

            // hjemmeadresse
            Address1_Line1 = row.GetFieldValue<string>("Address"),
            Address1_PostalCode = row.GetFieldValue<string>("ZipCode"),
            Address1_City = row.GetFieldValue<string>("City"),
        };
        return new UpsertRequest() { Target = contact, };
    }

    private static Contact_GenderCode? MapGender(string? genderFromSource)
    {
        return genderFromSource switch
        {
            null => null,
            "K" => Contact_GenderCode.Kvinde,
            "M" => Contact_GenderCode.Mand,
            _ => null,
        };
    }
}