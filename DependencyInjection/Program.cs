using System;

// --- SÖZLEŞME VE BANKALAR ---
public interface IBanka { void OdemeYap(); }
public class Akbank : IBanka { public void OdemeYap() => Console.WriteLine("Akbank ile ödendi."); }
public class Garanti : IBanka { public void OdemeYap() => Console.WriteLine("Garanti ile ödendi."); }

// ============================================================
// 1. CONSTRUCTOR INJECTION (YAPICI METOT)
// ============================================================
public class Constructor_Kotu
{
    private IBanka _banka = new Akbank(); // HATA: İçeride göbekten bağlandık.
    public void Calis() => _banka.OdemeYap();
}

public class Constructor_Iyi
{
    private readonly IBanka _banka;
    public Constructor_Iyi(IBanka banka) => _banka = banka; // DOĞRU: Dışarıdan enjekte edildi.
    public void Calis() => _banka.OdemeYap();
}

// ============================================================
// 2. SETTER INJECTION (PROPERTY/ÖZELLİK)
// ============================================================
public class Setter_Kotu
{
    public IBanka Banka { get; set; } = new Akbank(); // HATA: Varsayılanı içeride belirledik.
    public void Calis() => Banka.OdemeYap();
}

public class Setter_Iyi
{
    public IBanka Banka { get; set; } // DOĞRU: İhtiyaca göre dışarıdan atanır.
    public void Calis() => Banka?.OdemeYap();
}

// ============================================================
// 3. METHOD INJECTION (METOT PARAMETRESİ)
// ============================================================
public class Method_Kotu
{
    public void Calis()
    {
        var banka = new Akbank(); // HATA: Sadece bu iş için içeride bağımlılık yarattık.
        banka.OdemeYap();
    }
}

public class Method_Iyi
{
    public void Calis(IBanka banka)
    { // DOĞRU: Sadece bu metot için gerekeni dışarıdan aldık.
        banka.OdemeYap();
    }
}

// ============================================================
// ANA PROGRAM (KIYASLAMA)
// ============================================================
class Program
{
    static void Main()
    {
        Console.WriteLine("=== 1. CONSTRUCTOR INJECTION ===");
        // Kötüde seçim şansın yok, hep Akbank çalışır.
        var cIyi = new Constructor_Iyi(new Garanti()); // İstediğimiz bankayı taktık!
        cIyi.Calis();

        Console.WriteLine("\n=== 2. SETTER INJECTION ===");
        var sIyi = new Setter_Iyi();
        sIyi.Banka = new Akbank(); // Çalışma anında (runtime) atadık.
        sIyi.Calis();

        Console.WriteLine("\n=== 3. METHOD INJECTION ===");
        var mIyi = new Method_Iyi();
        mIyi.Calis(new Garanti()); // Sadece çağırma anında bankayı verdik.

        Console.ReadLine();
    }
}