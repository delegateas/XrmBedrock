using DataverseService.Foundation.Dao;
using Microsoft.Extensions.Logging;
using Microsoft.PowerPlatform.Dataverse.Client;
using SharedContext.Dao;

namespace Azure.DataverseService.Foundation.Dao;

/// <summary>
/// This is an extension of the DataverseAccessObject with some Async methods for use in Azure Functions and Azure App Services.
/// The IOrganizationServiceAsync/IOrganizationServiceAsync2 is NOT available serverside so this extension is NOT supported for plugins.
///
/// Please note that there is no parallelisation benefit of using Async-operations if you await them imediately. Please read this illustrative article to see how you can benefit from Async-use:
/// https://learn.microsoft.com/en-us/dotnet/csharp/asynchronous-programming/
///
/// There are two types of Async methods in this class UnawaitedAsync and Async.
///
/// In Async methods we await the async call to be able to catch exceptions, meassure time spent etc like we do for the sync methods. This kills the option for parallelisation though.
/// In UnawaitedAsync methods we do not await, so we will not be able to catch exceptions etc, so code using these will have to account for that. The gain of using these is the option to start Tasks in parallel.
///
/// This class is implemented as partial to split an otherwise large file into smaller files.
/// </summary>
public partial class DataverseAccessObjectAsync : DataverseAccessObject, IDataverseAccessObjectAsync
{
    private readonly IOrganizationServiceAsync2 orgServiceAsync;
    private CancellationToken cancellationToken = CancellationToken.None;

    public DataverseAccessObjectAsync(IOrganizationServiceAsync2 orgServiceAsync, ILogger logger)
        : base(orgServiceAsync, logger)
    {
        this.orgServiceAsync = orgServiceAsync;
    }

    public void SetCancellationToken(CancellationToken cancellationToken)
    {
        this.cancellationToken = cancellationToken;
    }

    private bool CancellationTokenIsSet() => cancellationToken != CancellationToken.None;
}