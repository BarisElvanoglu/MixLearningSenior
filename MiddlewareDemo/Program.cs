using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;

// Özet (kısa):
// Bu dosya, ASP.NET Core request pipeline (middleware zinciri) nasıl çalışır
// ve middleware'ların nasıl yazılıp kaydedildiğini gösterir.
// - Middleware'lar kaydedildikleri sırada çalışır.
// - Her middleware "ön işlem" yapıp await next() ile zinciri devam ettirebilir,
//   veya next() çağırmayarak pipeline'ı short-circuit (kısa devre) edebilir.
// - Response geri dönerken middleware'lar ters sırada "son işlem" yapar.

// Basit custom middleware: istekleri log'lar, /forbidden ile başlayan istekleri engeller.
public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;

    // DI ile RequestDelegate alınır
    public RequestLoggingMiddleware(RequestDelegate next) => _next = next;

    // InvokeAsync HttpContext alır: burada ön/son işlemler yapılır
    public async Task InvokeAsync(HttpContext context)
    {
        var sw = Stopwatch.StartNew();
        Console.WriteLine($"[Request] {context.Request.Method} {context.Request.Path}");

        // Örnek short-circuit: /forbidden ile başlayan istekleri 403 döndürerek engelle
        if (context.Request.Path.StartsWithSegments("/forbidden", StringComparison.OrdinalIgnoreCase))
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.Response.WriteAsync("Forbidden by middleware.");
            return; // _next çağrılmadı -> pipeline kısa devre oldu
        }

        // => next çağrısı: pipeline'daki bir sonraki middleware çalışır
        await _next(context);

        sw.Stop();
        // next döndükten sonra post-processing yapabiliriz (response bilgisi vb.)
        Console.WriteLine($"[Response] {context.Response.StatusCode} (took {sw.ElapsedMilliseconds} ms)");
    }
}

// Kayıt kolaylığı için extension metodu (temiz Program.cs görünümü)
public static class MiddlewareExtensions
{
    public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder app)
    {
        return app.UseMiddleware<RequestLoggingMiddleware>();
    }
}


// Program: pipeline örneği
var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// ÖNEMLİ: Global hata yakalama (UseExceptionHandler) en başa konmalı.
// app.UseExceptionHandler("/error");

// Inline middleware A: next çağırıyor -> hem "before" hem "after" çalışır
app.Use(async (context, next) =>
{
    Console.WriteLine("Middleware A - before");
    await next(); // pipeline devam etsin
    Console.WriteLine("Middleware A - after");
});

// Custom middleware kayıt (extension üzerinden)
app.UseRequestLogging(); // RequestLoggingMiddleware kaydedildi

// Inline middleware B: query string ile short-circuit örneği
app.Use(async (context, next) =>
{
    Console.WriteLine("Middleware B - before");
    if (context.Request.Query.ContainsKey("short"))
    {
        // next çağrılmadığı için pipeline kısa devre olur; sonraki middleware'lar çalışmaz
        context.Response.ContentType = "text/plain";
        await context.Response.WriteAsync("Short-circuited by middleware B.");
        return;
    }
    await next();
    Console.WriteLine("Middleware B - after");
});

// Routing ve auth notu:
// Eğer uygulama controller/endpoint routing kullanıyorsa sıraya dikkat edin:
// app.UseRouting();
// app.UseAuthentication();
// app.UseAuthorization();
// app.MapControllers();

// Minimal endpoint'ler
app.MapGet("/", () => "Merhaba, ana endpoint!");
app.MapGet("/forbidden", () => "Bu endpoint middleware tarafından engellenmiş olabilir.");
app.MapGet("/info", (HttpContext ctx) =>
{
    // Bu endpoint, üstteki middleware'lar tarafından işlenmiş olabilir.
    return Results.Text($"Path: {ctx.Request.Path}, QueryString: {ctx.Request.QueryString}");
});

// Uygulamayı başlat
app.Run();

/*
Kısa notlar (özet):
- Kullanım: app.Use(...) bir middleware ekler ve genelde next() çağırır.
- app.Run(...) terminal middleware'dır (next yok) — genelde son endpoint benzeri.
- Middleware sırası önemlidir: logging/exception en başta, routing/auth doğru yerde.
- Short-circuit: bir middleware next() çağırmazsa pipeline sonlanır.
- Middleware'ları küçük, tek amaçlı ve test edilebilir tutun; DI ile bağımlılık verin.

Bu tek dosyada:
- RequestLoggingMiddleware, extension ve Program.cs içeriği bir arada.
- Açıklamalar kod içinde yorum satırlarında (Türkçe) verilmiştir.
*/