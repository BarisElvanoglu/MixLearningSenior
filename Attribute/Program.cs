using System;
using System.Linq;
using System.Reflection;

// Özel Attribute (Öznitelik) tanımı
// AttributeUsage ile nerede kullanılacağını, kalıtım davranışını ve çoklu kullanım iznini belirtiriz.
[AttributeUsage(
    AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Parameter,
    Inherited = true,     // alt sınıflara miras geçsin mi
    AllowMultiple = false // aynı öğe üzerinde birden çok kez kullanılabilsin mi
)]
public sealed class AuditAttribute : Attribute
{   
    // Zorunlu (positional) parametre
    public string Action { get; }

    // İsteğe bağlı (named) parametre
    public string? Tag { get; set; }

    // Constructor ile zorunlu bilgiyi zorlarız
    public AuditAttribute(string action)
    {
        Action = action;
    }
}

public class Program
{
    // Attribute'ün sınıf, property, metod ve parametre üzerinde nasıl kullanıldığı gösteriliyor.
    [Audit("Create", Tag = "Payments")] // sınıf seviyesinde audit bilgisi
    public class PaymentService
    {
        [Audit("Update", Tag = "Amounts")] // property seviyesinde
        public decimal Amount { get; set; }

        [Audit("Process", Tag = "Orders")] // metod seviyesinde
        public void ProcessOrder([Audit("OrderId")] int orderId) // parametre seviyesinde
        {
            // iş mantığı burada olur
            Console.WriteLine($"Processing order {orderId} for amount {Amount:C}");
        }
    }

    // Program giriş noktası
    public static void Main()
    {
        var svc = new PaymentService { Amount = 123.45M };
        svc.ProcessOrder(42);

        Console.WriteLine();
        // Reflection ile attribute okuma örneği
        var type = typeof(PaymentService);

        // Sınıf üzerindeki AuditAttribute'ü al
        var classAttr = type.GetCustomAttribute<AuditAttribute>();
        if (classAttr != null)
        {
            Console.WriteLine($"Class Audit - Action: {classAttr.Action}, Tag: {classAttr.Tag}");
        }

        // Property üzerindeki AuditAttribute'ü al
        var prop = type.GetProperty(nameof(PaymentService.Amount));
        var propAttr = prop?.GetCustomAttribute<AuditAttribute>();
        if (propAttr != null)
        {
            Console.WriteLine($"Property '{prop!.Name}' Audit - Action: {propAttr.Action}, Tag: {propAttr.Tag}");
        }

        // Metod üzerindeki AuditAttribute'ü al
        var method = type.GetMethod(nameof(PaymentService.ProcessOrder));
        var methodAttr = method?.GetCustomAttribute<AuditAttribute>();
        if (methodAttr != null)
        {
            Console.WriteLine($"Method '{method!.Name}' Audit - Action: {methodAttr.Action}, Tag: {methodAttr.Tag}");
        }

        // Parametre üzerindeki AuditAttribute'ü al (metodun ilk parametresi örneği)
        var param = method?.GetParameters().FirstOrDefault();
        var paramAttr = param?.GetCustomAttribute<AuditAttribute>();
        if (paramAttr != null)
        {
            Console.WriteLine($"Parameter '{param!.Name}' Audit - Action: {paramAttr.Action}, Tag: {paramAttr.Tag}");
        }
    }
}