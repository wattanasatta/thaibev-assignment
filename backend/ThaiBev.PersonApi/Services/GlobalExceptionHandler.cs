using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace ThaiBev.PersonApi.Services;

/// <summary>
/// Global exception handler: AppValidationException → 400 ValidationProblemDetails
/// ที่เหลือ → 500 ProblemDetails โดยไม่เปิดเผย stack trace
/// </summary>
public sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is AppValidationException validation)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsJsonAsync(new ValidationProblemDetails(
                new Dictionary<string, string[]>(validation.Errors))
            {
                Title = "ข้อมูลไม่ถูกต้อง",
                Status = StatusCodes.Status400BadRequest,
            }, cancellationToken);
            return true;
        }

        logger.LogError(exception, "Unhandled exception while processing {Method} {Path}",
            context.Request.Method, context.Request.Path);

        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        await context.Response.WriteAsJsonAsync(new ProblemDetails
        {
            Title = "เกิดข้อผิดพลาดภายในเซิร์ฟเวอร์",
            Detail = "กรุณาลองใหม่อีกครั้งภายหลัง",
            Status = StatusCodes.Status500InternalServerError,
        }, cancellationToken);
        return true;
    }
}
