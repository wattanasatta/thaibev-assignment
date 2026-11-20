namespace ThaiBev.PersonApi.Services;

/// <summary>Validation error ฝั่ง service (นอกเหนือจาก DataAnnotations ของ DTO) — map เป็น ValidationProblemDetails 400</summary>
public class AppValidationException(IDictionary<string, string[]> errors)
    : Exception("One or more validation errors occurred.")
{
    public IDictionary<string, string[]> Errors { get; } = errors;
}
