using System;
using System.Threading;
using System.Threading.Tasks;

class Program
{
        static void Main()
    {
        Console.WriteLine("=== lock Anahtar Kelimesi Örneği ===\n");

        // Örnek 1: lock kullanarak thread-safe sayaç
        var counter = new ThreadSafeCounter();
        
        // 5 thread oluştur, her biri 1000 kez sayacı arttırsın
        var tasks = new Task[5];
        for (int i = 0; i < 5; i++)
        {            tasks[i] = Task.Run(() =>
            {
                for (int j = 0; j < 1000; j++)
                {
                    counter.Increment();
                }
            });
        }

        Task.WaitAll(tasks);
        Console.WriteLine($"Örnek 1 - Lock ile Sayaç: {counter.Value}");
        Console.WriteLine("(Beklenen: 5000)\n");

        // Örnek 2: Monitor.Enter ile manual kontrol
        var bankAccount = new BankAccount(1000);
        
        tasks = new Task[3];
        for (int i = 0; i < 3; i++)
        {
            tasks[i] = Task.Run(() => bankAccount.Withdraw(200));
        }

        Task.WaitAll(tasks);
        Console.WriteLine($"Örnek 2 - Bank Hesabı (Monitor.Enter): {bankAccount.Balance} TL");
        Console.WriteLine("(Beklenen: 400 TL)\n");

        // Örnek 3: Deadlock riski
        Console.WriteLine("Örnek 3 - Nested lock örneği:");
        var account = new Account(500);
        account.TransferSafely(100);
        Console.WriteLine($"Sonuç: {account.Balance} TL\n");
    }
}

// Örnek 1: lock ile thread-safe sayaç
class ThreadSafeCounter
{
    private int value = 0;
    private readonly object lockObject = new object();

    public void Increment()
    {
        lock (lockObject)
        {
            // Kritik bölüm - sadece bir thread erişebilir
            value++;
        }
    }

    public int Value
    {
        get
        {
            lock (lockObject)
            {
                return value;
            }
        }
    }
}

// Örnek 2: Monitor.Enter ile bank hesabı
class BankAccount
{
    private decimal balance;
    private readonly object lockObject = new object();

    public BankAccount(decimal initialBalance)
    {
        balance = initialBalance;
    }

    public void Withdraw(decimal amount)
    {
        bool lockTaken = false;
        try
        {
            Monitor.Enter(lockObject, ref lockTaken);
            
            if (balance >= amount)
            {
                Thread.Sleep(10); // Gerçekçi işlem simülasyonu
                balance -= amount;
                Console.WriteLine($"Para çekildi: {amount} TL, Kalan: {balance} TL");
            }
            else
            {
                Console.WriteLine($"Yetersiz bakiye! Mevcut: {balance} TL, İstenen: {amount} TL");
            }
        }
        finally
        {
            if (lockTaken)
                Monitor.Exit(lockObject);
        }
    }

    public decimal Balance
    {
        get
        {
            lock (lockObject)
            {
                return balance;
            }
        }
    }
}

// Örnek 3: Nested lock (dikkatli olunmalı - deadlock riski)
class Account
{
    private decimal balance;
    private readonly object lockObject = new object();

    public Account(decimal initialBalance)
    {
        balance = initialBalance;
    }

    public void TransferSafely(decimal amount)
    {
        lock (lockObject)
        {
            Console.WriteLine($"Transfer başladı: {amount} TL");
            Thread.Sleep(50);
            balance -= amount;
            Console.WriteLine($"Transfer tamamlandı. Yeni bakiye: {balance} TL");
        }
    }

    public decimal Balance => balance;
}