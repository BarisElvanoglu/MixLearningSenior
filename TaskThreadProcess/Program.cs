using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

class Program
{
    static async Task Main()
    {
        // --- 1. PROCESS (Dükkanın Kendisi) ---
        // Uygulamanın tamamını temsil eder.
        Process dukkan = Process.GetCurrentProcess();
        Console.WriteLine($"Şu an '{dukkan.ProcessName}' isimli dükkanın (Process) içindeyiz.");
        Console.WriteLine($"Dükkanın ID'si: {dukkan.Id}");
        // Başka bir uygulama açarsan yeni bir dükkan (Process) açılmış olur.

        Console.WriteLine("\n------------------------------------\n");

        // --- 2. THREAD (Dükkandaki Usta) ---
        // Manuel bir Thread oluşturmak, yeni bir usta kiralamak gibidir.
        // Ona "Sen sadece bu işle ilgilen" dersin.
        Thread usta = new Thread(() =>
        {
            Console.WriteLine($"[Usta/Thread {Thread.CurrentThread.ManagedThreadId}]: Ben sadece bu kabloyu lehimliyorum...");
            Thread.Sleep(1000); // İşin süresi
            Console.WriteLine("[Usta/Thread]: Lehim bitti, ben gidiyorum.");
        });

        // Usta işe başlar ama dükkanın (Process) geri kalanı ondan bağımsız çalışmaya devam eder.
        usta.Start();

        Console.WriteLine("\n------------------------------------\n");

        // --- 3. TASK (Tezgaha Bırakılan Tamir Fişi) ---
        // Task, "Şu iş yapılacak" talimatıdır. 
        // Bunu kimin yapacağına (hangi thread'in) sistem karar verir.
        Console.WriteLine("[Dükkan Sahibi]: Bir tamir fişi (Task) oluşturuluyor...");

        Task tamirFisi = Task.Run(() =>
        {
            // Bu işi muhtemelen boşta bekleyen bir thread (yardımcı eleman) yapacaktır.
            Console.WriteLine($"[Görev/Task]: Bu işi {Thread.CurrentThread.ManagedThreadId} ID'li thread üstlendi.");
            Thread.Sleep(10000);
            Console.WriteLine("[Görev/Task]: Tamir tamamlandı!");
        });

        // Task'ın bitmesini bekliyoruz
        await tamirFisi;

        // Manuel oluşturduğumuz ustanın işini bitirmesini bekleyelim
        usta.Join();

        Console.WriteLine("\n[Dükkan Sahibi]: Tüm işler bitti, dükkanı kapatıyoruz.");
    }
}