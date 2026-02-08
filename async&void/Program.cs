using System;
using System.Threading.Tasks;

namespace AsyncMasterClass
{
    // ============================================================
    // ASYNC VOID: NEDEN ÖNERİLMEZ? HANGİ DURUMDA İSTİSNA?
    // ============================================================
    //
    // Kısa özet (Türkçe açıklamalar script içinde):
    //   * async void normal metotlarda KESİNLİKLE kaçınılmalıdır.
    //   * Await edilemez 
    //   * Exception'lar çağırana dönmez; genellikle AppDomain / process seviyesinde
    //     "unhandled exception" olarak değerlendirilir.
    //   * Çağıran metot metotun tamamlandığını bilemez (tamamlanma takibi yok).
    //   * Task bazlı koordinasyon (Task.WhenAll, await, vb.) imkansız olur.
    //
    // - İstisna: Event handler'lar (ör. button.Click) — event handler'ların imzası
    //   void olduğu için async void burada kabul edilir. Yine de handler içinde
    //   exception'ları açıkça handle etmek gerekir (try/catch).
    //
    // Aşağıda örnekler ve açıklamalar ile birlikte bir Main metodu bulunmaktadır.
    // Tüm açıklamalar comment içinde yer almaktadır.

    // ============================================================
    // MODELLER / YARDIMCI METOTLAR
    // ============================================================
    public class DownloadEventArgs : EventArgs
    {
        public string FileUrl { get; set; } = string.Empty;
    }

    // ============================================================
    // 1) HATA YÖNETİMİ ÖRNEKLERİ
    // ============================================================
    public class BadExample_AsyncVoid
    {
        private async Task<string> FetchFromServer()
        {
            await Task.Delay(300);
            return "Sunucudan gelen veriler";
        }

        // KÖTÜ: async void - çağıran bu metodu await edemez, Exception'lar izlenmesi zordur.
        public async void DownloadData_Bad()
        {
            // Buradaki try/catch çalışsa bile dışarıya fırlayan exception'lar için
            // çağıran bir kontrol mekanizması yoktur.
            try
            {
                string data = await FetchFromServer();
                // Aşağıdaki satır bir FormatException fırlatır.
                int result = int.Parse("abc");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Bad] Internal catch: {ex.GetType().Name} - {ex.Message}");
            }
        }
    }

    public class GoodExample_AsyncTask
    {
        private async Task<string> FetchFromServer()
        {
            await Task.Delay(300);
            return "Sunucudan gelen veriler";
        }

        // İYİ: async Task - çağıran await ederek exception'ı yakalayabilir.
        public async Task DownloadData_Good()
        {
            try
            {
                string data = await FetchFromServer();
                int result = int.Parse("abc");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Good] Internal catch: {ex.GetType().Name} - {ex.Message}");
            }
        }
    }

    // ============================================================
    // 2) TAMAMLANMA TAKİBİ ÖRNEĞİ
    // ============================================================
    public class Example_CompletionProblem
    {
        // async void: çağıran tamamlanmayı bilemez.
        public async void SaveUserBad(string name)
        {
            await Task.Delay(500);
            Console.WriteLine($"Kullanıcı '{name}' kaydedildi (void)");
        }

        // async Task: çağıran await ile tamamlanmayı garanti eder.
        public async Task SaveUserGood(string name)
        {
            await Task.Delay(500);
            Console.WriteLine($"Kullanıcı '{name}' kaydedildi (task)");
        }

        public async Task DemoCompletion()
        {
            Console.WriteLine("\n--- Demo: Tamamlanma Takibi ---");
            // Kötü: SaveUserBad çağrıldıktan sonra hemen devam ederiz; tamamlanma belirsiz.
            SaveUserBad("Ahmet");
            Console.WriteLine("SaveUserBad çağrıldı (tamamlanma bilinmiyor).");

            // İyi: SaveUserGood await edildiği için burada kesinlikle tamamlandığını biliriz.
            await SaveUserGood("Hasan");
            Console.WriteLine("SaveUserGood tamamlandı (await ile garanti).");
        }
    }

    // ============================================================
    // 3) SENKRONİZASYON ÖRNEĞİ (Task.WhenAll kullanımı)
    // ============================================================
    public class Example_SynchronizationProblem
    {
        public async Task ProcessOrderGood(int orderId)
        {
            Console.WriteLine($"[Sipariş {orderId}] İşlem başladı");
            await Task.Delay(400);
            Console.WriteLine($"[Sipariş {orderId}] İşlem bitti");
        }

        public async Task DemoSynchronization()
        {
            Console.WriteLine("\n--- Demo: Toplu İşleme (WhenAll) ---");
            var t1 = ProcessOrderGood(1);
            var t2 = ProcessOrderGood(2);
            var t3 = ProcessOrderGood(3);

            // Burada Task'ları await ederek tüm işlemlerin tamamlanmasını bekleyebiliriz.
            await Task.WhenAll(t1, t2, t3);
            Console.WriteLine("Tüm siparişler tamamlandı (WhenAll ile koordinasyon).");
        }
    }

    // ============================================================
    // 4) EVENT HANDLER ÖRNEĞİ: async void'ün KABUL EDİLDİĞİ DURUM
    // ============================================================
    public class EventExample
    {
        // Event tanımı: normal EventHandler imzası void döner, bu yüzden handler async void olabilir.
        public event EventHandler<DownloadEventArgs>? DownloadCompleted;

        // Metot: event'i tetikler
        public void RaiseDownload(string url)
        {
            // Event tetiklenir; aboneler async void ise onlar kendi içinde asenkron çalışır.
            DownloadCompleted?.Invoke(this, new DownloadEventArgs { FileUrl = url });
        }
    }

    public class EventHandlers
    {
        // DOĞRU: Event handler olarak async void kullanılabilir.
        // Ancak tüm exception'ları handler içinde try/catch ile handle etmek gereklidir.
        public async void OnDownloadCompleted_Handler(object? sender, DownloadEventArgs e)
        {
            try
            {
                Console.WriteLine($"[EventHandler] İndirme başladı: {e.FileUrl}");
                await Task.Delay(600);
                Console.WriteLine("[EventHandler] İndirme tamamlandı ve UI güncellendi.");
            }
            catch (Exception ex)
            {
                // Event handler içinde exception'ları kendi başınıza handle edin.
                Console.WriteLine($"[EventHandler] Hata: {ex.Message}");
            }
        }
    }

    // ============================================================
    // 5) PRATİK KULLANIM: await edilebilir Task metot örneği
    // ============================================================
    public class PracticalExample
    {
        private async Task<string> FetchFromServer()
        {
            await Task.Delay(250);
            return "Veri paketim";
        }

        public async Task LoadDataGood()
        {
            Console.WriteLine("[Practical] Veri çekiliyor...");
            string data = await FetchFromServer();
            Console.WriteLine($"[Practical] Veri alındı: {data}");
        }

        public async Task DemoPractical()
        {
            Console.WriteLine("\n--- Demo: Kontrol Akışı (Practical) ---");
            await LoadDataGood();
            Console.WriteLine("[Practical] LoadDataGood tamamlandı; burası kesinlikle sonra çalışır.");
        }
    }

    // ============================================================
    // MAIN: TÜM ÖRNEKLERİN DEMOSU
    // ============================================================
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("=== async void Eğitimi: Demo Başladı ===");

            // 1) Practical demo - doğru kullanım
            var practical = new PracticalExample();
            await practical.DemoPractical();

            // 2) Completion demo - async void vs async Task
            var completion = new Example_CompletionProblem();
            await completion.DemoCompletion();

            // 3) Synchronization demo - Task.WhenAll
            var sync = new Example_SynchronizationProblem();
            await sync.DemoSynchronization();

            // 4) Hata yönetimi örnekleri
            Console.WriteLine("\n--- Hata Yönetimi Örnekleri ---");
            var bad = new BadExample_AsyncVoid();
            // Kötü: async void olduğu için burada await edemeyiz; exception'ların
            // dışarıya nasıl yansıyacağı kontrolümüz dışında olabilir.
            bad.DownloadData_Bad();

            var good = new GoodExample_AsyncTask();
            // İyi: await ile exception'ı çağıran yakalayabilir (varsa).
            await good.DownloadData_Good();

            // 5) Event handler örneği (async void'in kabul edildiği durum)
            Console.WriteLine("\n--- Event Handler Demo (async void is allowed for event handlers) ---");
            var eventSource = new EventExample();
            var handlers = new EventHandlers();

            // Abone ol
            eventSource.DownloadCompleted += handlers.OnDownloadCompleted_Handler;

            // Event'i tetikle -> handler async void olduğu halde çalışacaktır.
            eventSource.RaiseDownload("https://example.com/file.zip");

            // Event handler asenkron olduğundan, burada hemen sona ermez; kısa süre bekleyelim.
            await Task.Delay(1000);

            Console.WriteLine("\n=== Tüm demolar tamamlandı ===");
            Console.WriteLine("Çıkmak için bir tuşa basın...");
            Console.ReadKey();
        }
    }
}