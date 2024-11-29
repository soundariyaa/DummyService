namespace PieHandlerService.Infrastructure.Services.Pie.Contracts;

public class ResponseBase
{
    protected readonly int[] DefaultReturnCodesNoError = { 0, 200, 201 };

    protected readonly int[] RetryableReturnCodes = { 101, 102, 103 };

    public int Code { get; set; } = 0;
    public int Status { get; set; }
    public string? Title { get; set; }
    public string Detail { get; set; } = string.Empty;
    public string ErrorDescription { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;

    internal virtual bool HasError() => DefaultReturnCodesNoError.All(value => value != Status);
    internal virtual bool HasRetryableError() => RetryableReturnCodes.Any(value => value == Code);


    /// <summary>
    /// PIE can send one of three possible error details parameters - this function sends the most prioritized error detail
    /// </summary>
    /// <returns>The most prioritized error detail</returns>
    internal string ErrorDetail()
    {
        return string.IsNullOrEmpty(Detail) ? 
                ( string.IsNullOrEmpty(ErrorDescription) ? (string.IsNullOrEmpty(Message) ? string.Empty : Message) : ErrorDescription) : Detail;
    }

    internal virtual bool IsEmpty()
    {
        return
            Status == default &&
            string.IsNullOrWhiteSpace(Title) &&
            string.IsNullOrWhiteSpace(Detail) &&
            string.IsNullOrWhiteSpace(ErrorDescription) &&
            string.IsNullOrWhiteSpace(Message);
    }
}
