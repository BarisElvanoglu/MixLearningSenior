using Microsoft.AspNetCore.Http;
using System.Diagnostics;
using System.Runtime.ConstrainedExecution;


//Middleware = zincirin halkası
//Her satır = middleware

// Request Pipeline = zincirin tamamı
// Yukarıdan aşağı tüm yol
public class RequestTimingMiddleware
{
    private readonly RequestDelegate _next;

    // Middleware constructor
    public RequestTimingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            // Bir sonraki middleware'e geç
            await _next(context);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Hata yakalandı: {ex.Message}");
            throw; // pipeline'ı bozma, yukarı fırlat
        }
        finally
        {
            stopwatch.Stop();
            Console.WriteLine(
                $"➡ {context.Request.Method} {context.Request.Path} " +
                $"| Süre: {stopwatch.ElapsedMilliseconds} ms");
        }
    }
}