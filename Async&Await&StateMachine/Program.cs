using System;
using System.Threading.Tasks;

// ÖRNEK 1: Basit async/await kullan?m?
Console.WriteLine("=== ÖRNEK 1: Basit async/await ===");
await Example1_BasicAsyncAwait();

// ÖRNEK 2: Birden fazla await (state machine davran??? görülür)
Console.WriteLine("\n=== ÖRNEK 2: Birden fazla await (state machine) ===");
await Example2_MultipleAwait();

// ÖRNEK 3: Gerçek dünya: veritaban? simülasyonu
Console.WriteLine("\n=== ÖRNEK 3: Veritaban? simülasyonu ===");
await Example3_DatabaseSimulation();

// ==================== ÖRNEK 1 ====================
async Task Example1_BasicAsyncAwait()
{
    Console.WriteLine("[1] Ba?la (Main thread)");

    // await: Task tamamlanana kadar bekle, thread geri ver
    string result = await FakeApiCall("Data1");

    Console.WriteLine($"[2] Sonuç ald?: {result}");
}

// ==================== ÖRNEK 2 ====================
async Task Example2_MultipleAwait()
{
    Console.WriteLine("[1] Durum 1: ?lk i?lem ba?la");
    string step1 = await FakeAsyncOperation("??lem1", 500);
    Console.WriteLine($"[2] Durum 2: ?lk i?lem bitti -> {step1}");

    Console.WriteLine("[3] Durum 3: ?kinci i?lem ba?la");
    string step2 = await FakeAsyncOperation("??lem2", 700);
    Console.WriteLine($"[4] Durum 4: ?kinci i?lem bitti -> {step2}");

    // Aç?klama: Derleyici bu metodu state machine'e çevirdi.
    // Dört durum (state) vard?r:
    // - Durum 0: Ba?lang?ç (console yaz?lar?)
    // - Durum 1: await FakeAsyncOperation ("??lem1") sonras?nda
    // - Durum 2: await FakeAsyncOperation ("??lem2") sonras?nda
    // - Durum 3: Son (return)
    Console.WriteLine("[5] Durum 5: Tüm i?lemler bitti");
}

// ==================== ÖRNEK 3 ====================
async Task Example3_DatabaseSimulation()
{
    Console.WriteLine("[Ana] Kullan?c? verilerini yükle...");

    // Paralel olarak birden fazla async işlem (Task.WhenAll)
    var userTask = FakeDatabaseQuery("SELECT * FROM Users", 1000);
    var orderTask = FakeDatabaseQuery("SELECT * FROM Orders", 800);

    // Her ikisi birlikte çalışır, her ikisini bekle
    var (users, orders) = await Task.WhenAll(userTask, orderTask)
        .ContinueWith(t => (
            t.IsCompletedSuccessfully ? userTask.Result : "Hata",
            t.IsCompletedSuccessfully ? orderTask.Result : "Hata"
        ));

    Console.WriteLine($"[Ana] Veriler yüklendi: {users}, {orders}");
}

// ==================== YARDIMC? METODLAR ====================

async Task<string> FakeApiCall(string param)
{
    Console.WriteLine($"  ?? API ça?r?s? yap?l?yor ({param})...");
    await Task.Delay(500); // 500ms boyunca thread geri ver
    Console.WriteLine($"  ?? API yan?t geldi");
    return $"Cevap: {param}";
}

async Task<string> FakeAsyncOperation(string name, int delayMs)
{
    Console.WriteLine($"  ?? {name} ba?lad? ({delayMs}ms)");
    await Task.Delay(delayMs); // Asynchronously bekle
    Console.WriteLine($"  ?? {name} bitti");
    return $"{name} tamamland?";
}

async Task<string> FakeDatabaseQuery(string query, int delayMs)
{
    Console.WriteLine($"  ?? Veritaban? sorgusu: {query.Substring(0, 10)}...");
    await Task.Delay(delayMs);
    return $"Sonuç ({delayMs}ms)";
}