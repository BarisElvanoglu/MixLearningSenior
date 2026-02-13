
using System;
using System.Threading;
using System.Threading.Tasks;

class Account
{
    public int Id { get; }
    public string Name { get; }
    public decimal Balance { get; set; }
    public readonly object lockObject = new object();

    public Account(int id,string name, decimal balance)
    {
        Id = id;
        Balance = balance;
        Name = name;
    }
}

class BankManager
{
    // --- KÖTÜ ÖRNEK: DEADLOCK RİSKLİ ---
    // Eğer iki kişi aynı anda birbirine para gönderirse program donar!
    public static void RiskyTransfer(Account from, Account to, decimal amount)
    {
         
        lock (from.lockObject)//fatih barış
        {
            
            if (from.Id == 1) { Console.WriteLine("Barış  nesnesi kitlendi"); }
            if (from.Id == 2) { Console.WriteLine("Fatih nesnesi kitlendi"); }
            lock (to.lockObject) // Karşı taraf da beni bekliyorsa kilitlendik!
            {
                Console.WriteLine("buraya girdi");
                if (from.Id == 1) { Console.WriteLine("Barış  nesnesi kitlendi"); }
                if (from.Id == 2) { Console.WriteLine("Fatih nesnesi kitlendi"); }
                from.Balance -= amount;
                to.Balance += amount;
                Console.WriteLine("Transfer bitti.");
            }
        }
    }


    //Account baris = new Account(id: 1, "bariş", balance: 1000);
    //Account fatih = new Account(id: 2, "fatih", balance: 1000);
    public static void SafeTransfer(Account from, Account to, decimal amount)///barış 1 fatih 2
    {
        // Kilitlerin sırasını belirle (Hiyerarşi)
        Account first = from.Id < to.Id ? from : to;
        Account second = from.Id < to.Id ? to : from;

        lock (first.lockObject)//bariş
        {
            Console.WriteLine($"Thread  {first.Id} kilitlendi (Sıralı).");
            if (first.Id == 1) { Console.WriteLine("Barış  nesnesi kitlendi"); }
            if (first.Id == 2) { Console.WriteLine("Fatih nesnesi kitlendi"); }

            lock (second.lockObject)//fatih
            {
                Console.WriteLine("buraya girdi safe");
                if (second.Id == 1) { Console.WriteLine("Barış  nesnesi kitlendi"); }
                if (second.Id == 2) { Console.WriteLine("Fatih nesnesi kitlendi"); }
                from.Balance -= amount;
                to.Balance += amount;
                Console.WriteLine($"Thread: Başarıyla transfer edildi.");
            }
        }
    }
    public static async Task Main(string[] arg) {
        Account baris = new Account(id: 1, "bariş", balance: 1000);
        Account fatih = new Account(id: 2,"fatih", balance: 1000);
      
        Task t1 = Task.Run(() =>
        {
            SafeTransfer(baris, fatih, 500);//önce bariş kendi nesnesine sonra fatih kendi nesnesine

        });
        Task t2 = Task.Run(() =>
        {
            SafeTransfer(fatih, baris, 200);//önce fatih kendi nesnesine sonra bariş kendi nesnesine


        });
      
        Task sonuc=Task.WhenAll(t1, t2);
        await sonuc;
        Console.WriteLine(baris.Balance + ","+fatih.Balance);
        Console.ReadLine() ;
    }
}
