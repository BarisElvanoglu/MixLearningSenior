using System;

public interface IMyService { string Id { get; } }

// Bu sınıfın her "new"lendiğinde yeni bir ID'si olacak
public class MyService : IMyService
{
    public string Id { get; } = Guid.NewGuid().ToString().Substring(0, 4);
}

class Program
{
    static void Main()
    {
        // --- 1. TRANSIENT (Her seferinde yeni nesne) ---
        Console.WriteLine("--- TRANSIENT SENARYOSU ---");
        // IServiceProvider'ın "new" dediği anı simüle ediyoruz
        var t1 = new MyService();
        var t2 = new MyService();
        Console.WriteLine($"1. Çağrı: {t1.Id}");
        Console.WriteLine($"2. Çağrı: {t2.Id}");
        Console.WriteLine("Sonuç: Kimlikler FARKLI. Çünkü her seferinde fabrikadan yeni çıktı.");
        Console.WriteLine("\n---------------------------\n");

        // --- 2. SCOPED (İstek boyunca aynı nesne) ---
        Console.WriteLine("--- SCOPED SENARYOSU (Tek Bir HTTP İsteği) ---");
        var s1 = new MyService(); // İstek başladı, bir kere oluşturuldu
        var s2 = s1;              // Aynı isteğin içinde başka bir sınıf aynı nesneyi istedi
        Console.WriteLine($"1. Servis kullanımı: {s1.Id}");
        Console.WriteLine($"2. Servis kullanımı: {s2.Id}");
        Console.WriteLine("Sonuç: Kimlikler AYNI. İstek bitene kadar nesne 'çekmecede' saklandı.");
        // --- Sayfa Yenilendi (Yeni Scope) ---
        var s3 = new MyService();
        Console.WriteLine($"Sayfa Yenilendi, Yeni Kullanım: {s3.Id}");
        Console.WriteLine("Sonuç: Kimlik DEĞİŞTİ. Çünkü eski scope (çekmece) çöpe atıldı.");

        Console.WriteLine("\n---------------------------\n");

        // --- 3. SINGLETON (Uygulama boyunca tek nesne) ---
        Console.WriteLine("--- SINGLETON SENARYOSU ---");
        var singleton = new MyService(); // Uygulama ayağa kalkarken oluştu
        Console.WriteLine($"Kullanıcı Hasan: {singleton.Id}");
        Console.WriteLine($"Kullanıcı Mehmet: {singleton.Id}");
        Console.WriteLine($"Sayfa 100 kez yenilense de: {singleton.Id}");
        Console.WriteLine("Sonuç: Kimlik ASLA değişmez. Herkes aynı nesneye (bellek adresine) bakar.");
    }
}