namespace DataverseService.Dto.Account;

public record CreateAccountRequest(
    string Name,
    string StreetName
);