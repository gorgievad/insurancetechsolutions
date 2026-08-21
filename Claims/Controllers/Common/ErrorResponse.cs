namespace Claims.Controllers.Common
{
    /// <summary>
    /// Body returned for a failed request
    /// </summary>
    public sealed record ErrorResponse(string Code);
}
