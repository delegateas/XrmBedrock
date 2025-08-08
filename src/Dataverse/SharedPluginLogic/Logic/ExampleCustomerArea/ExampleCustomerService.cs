using DataverseLogic;
using Microsoft.Xrm.Sdk;
using SharedContext.Dao;
using System.Text.RegularExpressions;
using XrmBedrock.SharedContext;
using Task = XrmBedrock.SharedContext.Task;

namespace Dataverse.PluginLogic.ExampleCustomerArea;

public class ExampleCustomerService // Missing an interface here? We actually do not need one as this IS exactly the logic we want to test, so exchanging the implementation for _this_ services does no make sense
{
    private readonly IPluginExecutionContext context;
    private readonly IAdminDataverseAccessObjectService adminDao;

    public ExampleCustomerService(IPluginExecutionContext context, IAdminDataverseAccessObjectService adminDao)
    {
        this.context = context;
        this.adminDao = adminDao;
    }

    public void ValidatePhoneNumber()
    {
        var account = context.GetTargetMergedWithPreImage<Account>();
        if (string.IsNullOrEmpty(account.Telephone1))
            return;

        // Regex to check if phone number starts with "+" and contains only digits and spaces
        string pattern = @"^\+[0-9\s]+$";
        if (!Regex.IsMatch(account.Telephone1, pattern))
            throw new InvalidPluginExecutionException("The phone number must start with '+' and contain only digits and spaces.");
    }

    public void CreateCreditAssessmentTask()
    {
        var account = context.GetTarget<Account>();

        // Check if Customer Type is set to "Supplier"
        // Assuming Customer Type is 'customertypecode' with value 3 for Supplier (adjust based on actual schema)
        if (account.CustomerTypeCode == account_customertypecode.Supplier)
        {
            // Get Account Name
            string accountName = account.Name ?? "Unnamed Account";

            // Create Task entity
            Task task = new Task
            {
                Subject = $"Credit Assessment for {accountName}",
                RegardingObjectId = new EntityReference(Account.EntityLogicalName, account.Id),
                ScheduledEnd = DateTime.Now.AddDays(7), // Due date set to one week from now
                OwnerId = new EntityReference(SystemUser.EntityLogicalName, context.InitiatingUserId), // Assign to the user who created the Account
            };

            // Create the Task in Dataverse
            adminDao.Create(task);
        }
    }

    public void CopyParentTelephone()
    {
        // Get the target entity as a strongly typed Account object
        var targetAccount = context.GetTarget<Account>();

        // Check if Telephone1 is already set on the new account
        if (!string.IsNullOrEmpty(targetAccount.Telephone1))
            return;

        // Check if a parent account is specified
        if (targetAccount.ParentAccountId != null)
        {
            var parentAccountPhone = adminDao.Retrieve<Account, string?>(targetAccount.ParentAccountId.Id, a => a.Telephone1);
            if (!string.IsNullOrWhiteSpace(parentAccountPhone))
                targetAccount.Telephone1 = parentAccountPhone;
        }
    }

    public void UpdateTelephoneOnSubaccounts()
    {
        // Get the target account merged with pre-image to access old values
        var preImage = context.GetRequiredPreImage<Account>();
        var postImage = context.GetRequiredPostImage<Account>();

        // Skip if there is no real change
        if (preImage.Telephone1 == postImage.Telephone1)
            return;

        // Query for subaccounts (child accounts where ParentAccountId matches this account's ID)
        var subaccounts = adminDao.RetrieveList(xrm => xrm.AccountSet
            .Where(a => a.ParentAccountId != null && a.ParentAccountId.Id == context.PrimaryEntityId && a.Telephone1 == preImage.Telephone1));

        // Update Telephone1 for each subaccount that matches the old value
        foreach (var subaccount in subaccounts)
        {
            subaccount.Telephone1 = postImage.Telephone1;
            adminDao.Update(subaccount);
        }
    }
}