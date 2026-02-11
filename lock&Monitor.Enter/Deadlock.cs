
using System;
using System.Threading;
using System.Threading.Tasks;

class Account
{
    public int Id { get; }
    public decimal Balance { get; set; }
    public readonly object lockObject = new object();

    public Account(int id, decimal balance)
    {
        Id = id;
        Balance = balance;
    }
}

class BankManager
{
    // --- KÖTÜ ÖRNEK: DEADLOCK RİSKLİ ---
    // Eğer iki kişi aynı anda birbirine para gönderirse program donar!
    public void RiskyTransfer(Account from, Account to, decimal amount)
    {
        lock (fatih)///Fatih para yollamak istiyor hesabınıı kitledi
        {
            Console.WriteLine($"Thread {Thread.CurrentThread.ManagedThreadId}: {from.Id} kilitlendi, {to.Id} bekleniyor...");
            Thread.Sleep(1000); // Deadlock ihtimalini artırmak için yapay bekleme

            lock (baris) // Karşı taraf da beni bekliyorsa kilitlendik!
            {
                from.Balance -= amount;
                to.Balance += amount;
                Console.WriteLine("Transfer bitti.");
            }
        }
    }

    public void RiskyTransfer(Account from, Account to, decimal amount)
    {
        lock (baris)//Barış para yollamak istiyor hesabını kitledi
        {
            Console.WriteLine($"Thread {Thread.CurrentThread.ManagedThreadId}: {from.Id} kilitlendi, {to.Id} bekleniyor...");
            Thread.Sleep(1000); // Deadlock ihtimalini artırmak için yapay bekleme

            lock (fatih) // Karşı taraf da beni bekliyorsa kilitlendik!
            {
                from.Balance -= amount;
                to.Balance += amount;
                Console.WriteLine("Transfer bitti.");
            }
        }
    }

    public void SafeTransfer(Account from, Account to, decimal amount)///barış 1 fatih 2
    {
        // Kilitlerin sırasını belirle (Hiyerarşi)
        Account first = from.Id < to.Id ? from : to;
        Account second = from.Id < to.Id ? to : from;

        lock (first.lockObject)
        {
            Console.WriteLine($"Thread {Thread.CurrentThread.ManagedThreadId}: {first.Id} kilitlendi (Sıralı).");
            Thread.Sleep(500);

            lock (second.lockObject)
            {
                from.Balance -= amount;
                to.Balance += amount;
                Console.WriteLine($"Thread {Thread.CurrentThread.ManagedThreadId}: Başarıyla transfer edildi.");
            }
        }
    }
    // --- İYİ ÖRNEK: GÜVENLİ (Sıralama Mantığı) ---
    // Her zaman küçük ID'li hesabı önce kilitleriz, böylece kimse ters sırada beklemez.
    public void SafeTransfer(Account from, Account to, decimal amount)///barış 1 fatih 2
    {
        // Kilitlerin sırasını belirle (Hiyerarşi)
        Account first = from.Id < to.Id ? from : to;
        Account second = from.Id < to.Id ? to : from;

        lock (first.lockObject)
        {
            Console.WriteLine($"Thread {Thread.CurrentThread.ManagedThreadId}: {first.Id} kilitlendi (Sıralı).");
            Thread.Sleep(500);

            lock (second.lockObject)
            {
                from.Balance -= amount;
                to.Balance += amount;
                Console.WriteLine($"Thread {Thread.CurrentThread.ManagedThreadId}: Başarıyla transfer edildi.");
            }
        }
    }
}