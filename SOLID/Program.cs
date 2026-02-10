using System;
using System.Collections.Generic;

namespace SOLID
{
    // ========================================
    // S - SINGLE RESPONSIBILITY PRINCIPLE
    // (Tek Sorumluluk Prensibi)
    // ========================================
    
    // ❌ KÖTÜ: Müşteri sınıfı çok fazla sorumluluk yüklüyor
    public class MusteriKotu
    {
        public string Ad { get; set; }
        public string Email { get; set; }
        
        public void MusteriKaydet() => Console.WriteLine($"{Ad} veritabanına kaydedildi.");
        public void EmailGonder() => Console.WriteLine($"{Email}'e email gönderildi.");
        public void FaturaYazdir() => Console.WriteLine("Fatura yazıldı.");
    }
    
    // ✅ İYİ: Her sınıf tek bir sorumluluk taşır
    public class Musteri
    {
        public string Ad { get; set; }
        public string Email { get; set; }
    }
    
    public class MusteriRepositorisi
    {
        public void Kaydet(Musteri musteri) => Console.WriteLine($"{musteri.Ad} veritabanına kaydedildi.");
    }
    
    public class EmailServisi
    {
        public void Gonder(string email) => Console.WriteLine($"{email}'e email gönderildi.");
    }
    
    public class FaturaServisi
    {
        public void Yazdir(Musteri musteri) => Console.WriteLine($"{musteri.Ad} için fatura yazıldı.");
    }
    
    // ========================================
    // O - OPEN/CLOSED PRINCIPLE
    // (Açık/Kapalı Prensibi)
    // ========================================
    
    // ❌ KÖTÜ: Yeni ödeme yöntemi eklemek için kodu değiştirmek gerekir
    public class OdemeSistemiKotu
    {
        public void Odet(string yontem, decimal tutar)
        {
            if (yontem == "KrediKarti")
                Console.WriteLine($"Kredi kartı ile {tutar}₺ ödeme yapıldı.");
            else if (yontem == "BankaHavalesi")
                Console.WriteLine($"Banka havalesi ile {tutar}₺ ödeme yapıldı.");
            // Yeni yöntem eklemek için if eklemek gerekir!
        }
    }
    
    // ✅ İYİ: Yeni ödeme yöntemi eklemek için sadece genişletir
    public interface IOdeme
    {
        void Odet(decimal tutar);
    }
    
    public class KrediKartiOdeme : IOdeme
    {
        public void Odet(decimal tutar) => Console.WriteLine($"Kredi kartı ile {tutar}₺ ödeme yapıldı.");
    }
    
    public class BankaHavalesioOdeme : IOdeme
    {
        public void Odet(decimal tutar) => Console.WriteLine($"Banka havalesi ile {tutar}₺ ödeme yapıldı.");
    }
    
    public class CuzzdanOdeme : IOdeme
    {
        public void Odet(decimal tutar) => Console.WriteLine($"Cüzdan ile {tutar}₺ ödeme yapıldı.");
    }
    
    public class OdemeSistemi
    {
        private IOdeme _odemeYontemi;
        
        public OdemeSistemi(IOdeme odemeYontemi) => _odemeYontemi = odemeYontemi;
        
        public void IslemYap(decimal tutar) => _odemeYontemi.Odet(tutar);
    }
    
    // ========================================
    // L - LISKOV SUBSTITUTION PRINCIPLE
    // (Liskov Yerine Geçebilirlik Prensibi)
    // ========================================
    
    // ❌ KÖTÜ: Türetilen sınıf, temel sınıfın sözleşmesini bozuyor
    public abstract class Hayvan
    {
        public abstract void SesCikar();
    }
    
    public class KopeKotu : Hayvan
    {
        public override void SesCikar() => Console.WriteLine("Hav hav!");
    }
    
    public class BaklaHayvanKotu : Hayvan
    {
        public override void SesCikar() 
            => throw new NotImplementedException("Balık ses çıkarmaz!"); // ❌ HATA!
    }
    
    // ✅ İYİ: Türetilen sınıflar temel sınıfın sözleşmesini kesinlikle yerine getirir
    public abstract class HayvanIyi
    {
        public abstract void SesCikar();
    }
    
    public class KopeIyi : HayvanIyi
    {
        public override void SesCikar() => Console.WriteLine("Hav hav!");
    }
    
    public class BalkIyi : HayvanIyi
    {
        public override void SesCikar() => Console.WriteLine("*Sessiz*");
    }
    
    // ========================================
    // I - INTERFACE SEGREGATION PRINCIPLE
    // (Arayüz Ayrımı Prensibi)
    // ========================================
    
    // ❌ KÖTÜ: Geniş interface, tüm metotları uygulamak zorunda
    public interface IIsci
    {
        void Calis();
        void Yemek();
        void KodYaz();
        void DesignYap();
    }
    
    public class ProgramciKotu : IIsci
    {
        public void Calis() => Console.WriteLine("Programlama yapıyor.");
        public void Yemek() => Console.WriteLine("Yemek yiyor.");
        public void KodYaz() => Console.WriteLine("Kod yazıyor.");
        public void DesignYap() => throw new NotImplementedException(); // ❌ Gereksiz!
    }
    
    // ✅ İYİ: Küçük, spesifik interfaceler
    public interface ICalisacak
    {
        void Calis();
    }
    
    public interface IYemekYeyecek
    {
        void Yemek();
    }
    
    public interface IKodYazacak
    {
        void KodYaz();
    }
    
    public interface IDesignYapacak
    {
        void DesignYap();
    }
    
    public class Programci : ICalisacak, IYemekYeyecek, IKodYazacak
    {
        public void Calis() => Console.WriteLine("Programlama yapıyor.");
        public void Yemek() => Console.WriteLine("Yemek yiyor.");
        public void KodYaz() => Console.WriteLine("Kod yazıyor.");
    }
    
    public class Tasarimci : ICalisacak, IYemekYeyecek, IDesignYapacak
    {
        public void Calis() => Console.WriteLine("Tasarım yapıyor.");
        public void Yemek() => Console.WriteLine("Yemek yiyor.");
        public void DesignYap() => Console.WriteLine("Design yapıyor.");
    }
    
    // ========================================
    // D - DEPENDENCY INVERSION PRINCIPLE
    // (Bağımlılık Ters Çevirme Prensibi)
    // ========================================
    
    // ❌ KÖTÜ: Yüksek seviye modül, düşük seviye modüle bağımlı
    public class VeriTabaniKotu
    {
        public void Kaydet(string veri) => Console.WriteLine($"SQL Server'a kaydedildi: {veri}");
    }
    
    public class MailGondericiKotu
    {
        private VeriTabaniKotu _db = new VeriTabaniKotu(); // ❌ Direkt bağımlılık!
        
        public void Gonder(string email)
        {
            _db.Kaydet(email);
            Console.WriteLine($"Mail gönderildi: {email}");
        }
    }
    
    // ✅ İYİ: Soyutlamaya bağımlı (interface)
    public interface IVeriDedposi
    {
        void Kaydet(string veri);
    }
    
    public class SqlVeriTabani : IVeriDedposi
    {
        public void Kaydet(string veri) => Console.WriteLine($"SQL Server'a kaydedildi: {veri}");
    }
    
    public class MongoDbVeriTabani : IVeriDedposi
    {
        public void Kaydet(string veri) => Console.WriteLine($"MongoDB'ye kaydedildi: {veri}");
    }
    
    public class MailGondericiIyi
    {
        private readonly IVeriDedposi _veritabani;
        
        public MailGondericiIyi(IVeriDedposi veritabani) => _veritabani = veritabani;
        
        public void Gonder(string email)
        {
            _veritabani.Kaydet(email);
            Console.WriteLine($"Mail gönderildi: {email}");
        }
    }
    
    // ========================================
    // MAIN - SOLID PRENSİPLERİ DENEMELERİ
    // ========================================
    
    class Program
    {
        static void Main()
        {
            Console.WriteLine("╔════════════════════════════════════════════════╗");
            Console.WriteLine("║     SOLID PRENSİPLERİ - TÜRKÇE ÖRNEKLER       ║");
            Console.WriteLine("╚════════════════════════════════════════════════╝\n");
            
            // S - Single Responsibility
            Console.WriteLine("📌 S - TEK SORUMLULUK PRENSİBİ");
            Console.WriteLine("Bölümlenmiş Görevler:\n");
            var musteri = new Musteri { Ad = "Ahmet", Email = "ahmet@mail.com" };
            new MusteriRepositorisi().Kaydet(musteri);
            new EmailServisi().Gonder(musteri.Email);
            new FaturaServisi().Yazdir(musteri);
            
            Console.WriteLine("\n-------------------------------------------\n");
            
            // O - Open/Closed
            Console.WriteLine("📌 O - AÇIK/KAPALI PRENSİBİ");
            Console.WriteLine("Farklı Ödeme Yöntemleri:\n");
            var odemeler = new List<IOdeme>
            {
                new KrediKartiOdeme(),
                new BankaHavalesioOdeme(),
                new CuzzdanOdeme()
            };
            
            foreach (var odeme in odemeler)
                new OdemeSistemi(odeme).IslemYap(100);
            
            Console.WriteLine("\n-------------------------------------------\n");
            
            // L - Liskov Substitution
            Console.WriteLine("📌 L - LİSKOV YERİNE GEÇEBİLİRLİK PRENSİBİ");
            Console.WriteLine("Hayvan Sınıfları:\n");
            var hayvanlar = new List<HayvanIyi>
            {
                new KopeIyi(),
                new BalkIyi()
            };
            
            foreach (var hayvan in hayvanlar)
                hayvan.SesCikar();
            
            Console.WriteLine("\n-------------------------------------------\n");
            
            // I - Interface Segregation
            Console.WriteLine("📌 I - ARAYÜZ AYRIMI PRENSİBİ");
            Console.WriteLine("Spesifik Roller:\n");
            ICalisacak programci = new Programci();
            IKodYazacak kodYazici = new Programci();
            
            programci.Calis();
            kodYazici.KodYaz();
            
            IDesignYapacak tasarimci = new Tasarimci();
            tasarimci.DesignYap();
            
            Console.WriteLine("\n-------------------------------------------\n");
            
            // D - Dependency Inversion
            Console.WriteLine("📌 D - BAĞIMLILIK TERS ÇEVİRME PRENSİBİ");
            Console.WriteLine("Soyut Veri Kaynakları:\n");
            
            var sqlMailer = new MailGondericiIyi(new SqlVeriTabani());
            sqlMailer.Gonder("test@sql.com");
            
            var mongoMailer = new MailGondericiIyi(new MongoDbVeriTabani());
            mongoMailer.Gonder("test@mongo.com");
            
            Console.WriteLine("\n╔════════════════════════════════════════════════╗");
            Console.WriteLine("║          SOLID = TEMIZ, ESTETİK KOD!          ║");
            Console.WriteLine("╚════════════════════════════════════════════════╝");
            
            Console.ReadLine();
        }
    }
}