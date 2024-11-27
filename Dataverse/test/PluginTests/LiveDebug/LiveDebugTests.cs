using Microsoft.Xrm.Tooling.Connector;
using SharedContext.Dao;
using SharedTest;

namespace Dataverse.PluginTests.LiveDebug;

/// <summary>
/// This class is for debugging stuff that you cannot test using XrmMockup like
/// - some of the things that works with XrmMockup but fails in D365 (like some of the special requests and some "activity"-handling)
/// - requires some specific data setup that is tedious to set up i XrmMockup
/// - requires something that is not supported by XrmMockup (like rowversion)
/// - etc etc
/// IT's a tool - not a part of the functional solution or formal tests!
///
/// DO REMEMBER TO REMOVE THE [Fact] ATTRIBUTE BEFORE PUSHING TO GIT!!
///
/// The DataverseConnectionString is supplied in the file LiveDebugConnectionString.local.txt in the same folder as this file. It is ignored by git
/// The suggested format is: "AuthType=OAuth; url=https://XXXX.crm4.dynamics.com; username=YourUsername; LoginPrompt=Always; AppId=51f81489-12ee-4a9e-aaae-a2591f45987d; RedirectUri=app://58145B91-0C36-4500-8554-080854F2AC97"
/// </summary>
public class LiveDebugTests
{
    #region sample use - kept as demo

    //[Fact]
    public void TestMethod1()
    {
        var myLiveDao = GetLiveDao();
        var someNamedAccounts = myLiveDao.RetrieveList(xrm => xrm.AccountSet.Where(a => a.Name != null).Take(10));
        throw new Exception("Remember to remove [Fact] before pushing to DevOps");
    }

    #endregion sample use - kept as demo

    #region Helper Methods

    private static DataverseAccessObject GetLiveDao()
    {
        var connString = GetDataverseConnectionstring();
        var orgService = GetOrgService(connString);
        return new DataverseAccessObject(orgService, new SimpleLogger());
    }

    private static CrmServiceClient GetOrgService(string dataverseConnectionstring)
    {
        var client = new CrmServiceClient(dataverseConnectionstring);
        if (client.LastCrmException != null)
            throw new Exception($"Connection to D365 fails with exception {client.LastCrmException.Message} Error: {client.LastCrmError}", client.LastCrmException);
        return client;
    }

    private static string GetDataverseConnectionstring()
    {
        // Attention! The file "LiveDebugConnectionString.local.txt" must be placed next to this file and is simply a text file with the dataverseConnectionstring to the dataverse environment you want to connecto to.
        // The suggested format is: "AuthType=OAuth; url=https://XXXX.crm4.dynamics.com; username=YourUsername; LoginPrompt=Always; AppId=51f81489-12ee-4a9e-aaae-a2591f45987d; RedirectUri=app://58145B91-0C36-4500-8554-080854F2AC97"
        try
        {
            using (var r = new StreamReader("..\\..\\..\\LiveDebug\\LiveDebugConnectionString.local.txt"))
            {
                var connectionString = r.ReadToEnd();
                if (string.IsNullOrWhiteSpace(connectionString))
                    throw new InvalidDataException("The file LiveDebugConnectionString.local.txt is empty but must contain a connectionstring");
                return connectionString;
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"Could not read the file LiveDebugConnectionString.local.txt or it is empty. Make sure it is in the correct location and contains the correct connection string. Details: {ex}", ex);
        }
    }
    #endregion Helper Methods
}