namespace DataverseService.Dto.Account;

public class CreateAccountResponse
{
    public CreateAccountResponse(Guid accountId)
    {
        AccountId = accountId;
    }

    public Guid AccountId { get; set; }
}