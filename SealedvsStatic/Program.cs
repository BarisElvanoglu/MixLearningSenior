// ============================================================
// SEALED vs STATIC CLASSES - KARŞILAŞTIRMA
// ============================================================

// 1. SEALED CLASS - Kalıtımı (inheritance) engelleyen sınıf
// Özellikle: Sınıf örneğini oluşturabilirsin, ama başka sınıflar bunu kalıtamazlar
[AttributeUsage(AttributeTargets.Class)]
public sealed class SealedExample:Attribute
{
    public void DoSomething() => Console.WriteLine("Sealed instance method");
}

// Bu çalışmaz - Derleme hatası verir!
// public class DerivedSealed : SealedExample { } // ❌ ERROR

// Ama bu çalışır - Örnek oluşturabilirsin
var sealed_obj = new SealedExample(); // ✅ OK
sealed_obj.DoSomething();

// ============================================================

// 2. STATIC CLASS - Örnek oluşturulamayan, sadece statik üyeler içeren sınıf
// Özellikle: Sınıf örneği oluşturamazsın, kalıtım yapılamazsın, 
//            sadece static üyeleri direkt olarak kullanırsın
public static class StaticExample
{
    // Static üye
    public static void DoSomething() => Console.WriteLine("Static method");

    // Static özellik
    public static int Counter { get; set; }
}

// Bu çalışmaz - Derleme hatası
// var static_obj = new StaticExample(); // ❌ ERROR

// Ama bu çalışır - Static üyeleri direkt kullan
StaticExample.DoSomething(); // ✅ OK
StaticExample.Counter = 5;

// ============================================================

// ÖZET KARŞILAŞTIRMASI:
// ┌─────────────────┬──────────────────┬──────────────────┐
// │ ÖZELLİK         │ SEALED CLASS     │ STATIC CLASS     │
// ├─────────────────┼──────────────────┼──────────────────┤
// │ Örnek oluştur   │ ✅ Evet          │ ❌ Hayır         │
// │ Kalıt al        │ ❌ Hayır         │ ❌ Hayır         │
// │ Instance üye    │ ✅ Var           │ ❌ Yok           │
// │ Static üye      │ ✅ Var           │ ✅ Var           │
// │ Constructor     │ ✅ Parameterli   │ ❌ Parametresiz  │
// │ Interface impl. │ ✅ Evet          │ ❌ Hayır         │
// │ Kullanım        │ Veri taşıma      │ Yardımcı işlevler│
// └─────────────────┴──────────────────┴──────────────────┘

// PRAKTIK ÖRNEKLERİ:

// SEALED - Örneğin Logger servisi (kalıtılmasını istemediğin ama örneklendirebilmek istediğin)
public sealed class LoggerService
{
    private string _name;

    public LoggerService(string name) => _name = name;

    public void Log(string message)
        => Console.WriteLine($"[{_name}] {message}");
}

var logger = new LoggerService("App"); // ✅ Örnek oluştur
logger.Log("Uygulama başladı");

// STATIC - Math sınıfı gibi yardımcı metotlar (örnek oluşturmaya gerek yok)
public static class MathHelper
{
    public static int Add(int a, int b) => a + b;
    public static int Multiply(int a, int b) => a * b;
}

int result = MathHelper.Add(5, 3); // ✅ Direkt kullan