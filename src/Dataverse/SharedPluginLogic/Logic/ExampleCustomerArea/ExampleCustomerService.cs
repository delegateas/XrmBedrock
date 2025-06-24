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
    private readonly IAdminDataverseAccessObjectService adminService;

    public ExampleCustomerService(IPluginExecutionContext context, IAdminDataverseAccessObjectService adminService)
    {
        this.context = context;
        this.adminService = adminService;
    }

    public void ValidatePhoneNumber()
    {
        var account = context.GetTargetMergedWithPreImage<Account>();

        string phoneNumber = account.Telephone1;

        if (string.IsNullOrEmpty(phoneNumber))
        {
            return;
        }

        // Regex to check if phone number starts with "+" and contains only digits and spaces
        string pattern = @"^\+[0-9\s]+$";
        bool isValid = Regex.IsMatch(phoneNumber, pattern);

        if (!isValid)
        {
            throw new InvalidPluginExecutionException("The phone number must start with '+' and contain only digits and spaces.");
        }
    }

    public void CreateCreditAssessmentTask()
    {
        var account = context.GetTarget<Account>();

        // Check if Customer Type is set to "Supplier"
        // Assuming Customer Type is 'customertypecode' with value 3 for Supplier (adjust based on actual schema)
        if (account.CustomerTypeCode == Account_CustomerTypeCode.Supplier)
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
            adminService.Create(task);
        }
    }
}