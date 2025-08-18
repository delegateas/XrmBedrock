namespace XrmMockupApi.Models;

public class ExecuteRequest
{
    public string RequestName { get; set; } = string.Empty;

    public Dictionary<string, object> Parameters { get; set; } = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
}