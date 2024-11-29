namespace PieHandlerService.Core.Models;

public class DatabaseSettings
{
    public string? ConnectionString { get; set; } = "mongodb://localhost:27017/PieHandlerServiceDB";
    public string? Database { get; set; } = "PieHandlerServiceDB";

    public IDictionary<string, object> Options { get; set; } = new Dictionary<string, object>();
}
