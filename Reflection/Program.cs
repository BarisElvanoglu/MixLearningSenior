using System;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;

namespace ReflectionExamples
{
    // Helper types moved outside Main so the file compiles when using a Program.Main
    public class Kişi
    {

        public string Ad { get; set; }
        public int Yaş { get; set; }
        public void SelamVer() => Console.WriteLine($"Merhaba, ben {Ad}");
        public static void StatikMetot() => Console.WriteLine("Statik Metot");
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
    public class BilgiAttribute : Attribute
    {
        public string Açıklama { get; set; }
        public BilgiAttribute(string açıklama) => Açıklama = açıklama;
    }

    [Bilgi("Kullanıcı Bilgileri")]
    public class Kullanıcı
    {
        [Bilgi("Kullanıcı adı")]
        public string KullanıcıAdı { get; set; }
    }

    public class Hayvan { }
    public class Köpek : Hayvan { }

    public class Araç
    {
        public string Marka = "Toyota";
        private int _hız = 0;
    }

    public class Düğme
    {
        public event EventHandler Tıklandı;
        public void TıklanmışOlarak() => Tıklandı?.Invoke(this, EventArgs.Empty);
    }

    public static class Program
    {
        public static void Main()
        {
            Console.WriteLine("=== REFLECTION EXAMPLES ===\n");

            // ============================================
            // 1. GetType vs typeof Karşılaştırması
            // ============================================
            Console.WriteLine("--- 1. GetType vs typeof ---");

            string text = "Merhaba";
            Type type1 = text.GetType();      // Çalışma zamanında - nesne gerekli
            Type type2 = typeof(string);      // Derleme zamanında - nesne gerekmez

            Console.WriteLine($"GetType:  {type1.Name}");
            Console.WriteLine($"typeof:   {type2.Name}");
            Console.WriteLine($"Eşit mi?: {type1 == type2}\n");

            // GetType - null referans hatasına açık
            string nullText = null;
            // Type type3 = nullText.GetType(); // ❌ NullReferenceException

            // typeof - null olsa da sorun yok
            Type type4 = typeof(string);        // ✅ Çalışır

            // ============================================
            // 2. Sınıfları ve Üyeleri Keşfetme (Reflection)
            // ============================================
            Console.WriteLine("--- 2. Sınıfları Keşfetme ---");

            Type kisiType = typeof(Kişi);

            // Tüm Properties'i getir
            PropertyInfo[] properties = kisiType.GetProperties();
            Console.WriteLine("Properties:");
            foreach (var prop in properties)
            {
                Console.WriteLine($"  - {prop.Name} ({prop.PropertyType.Name})");
            }

            // Tüm Methods'u getir
            MethodInfo[] methods = kisiType.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
            Console.WriteLine("Methods:");
            foreach (var method in methods)
            {
                Console.WriteLine($"  - {method.Name}()");
            }
            Console.WriteLine();

            // ============================================
            // 3. Property Değerlerini Dinamik Oku/Yaz
            // ============================================
            Console.WriteLine("--- 3. Properties Dinamik Oku/Yaz ---");

            Kişi kişi1 = new Kişi();
            PropertyInfo adProperty = kisiType.GetProperty("Ad");
            PropertyInfo yaşProperty = kisiType.GetProperty("Yaş");

            // Yazma (Set)
            adProperty.SetValue(kişi1, "Ahmet");
            yaşProperty.SetValue(kişi1, 25);

            // Okuma (Get)
            Console.WriteLine($"Ad: {adProperty.GetValue(kişi1)}");
            Console.WriteLine($"Yaş: {yaşProperty.GetValue(kişi1)}\n");

            // ============================================
            // 4. Metotları Dinamik Çağırma
            // ============================================
            Console.WriteLine("--- 4. Metotları Dinamik Çağırma ---");

            MethodInfo selamVerMethod = kisiType.GetMethod("SelamVer");
            selamVerMethod.Invoke(kişi1, null);  // null parametresi: argüman yok

            MethodInfo statikMetot = kisiType.GetMethod("StatikMetot", BindingFlags.Static | BindingFlags.Public);
            statikMetot.Invoke(null, null);       // null object: statik metot
            Console.WriteLine();

            // ============================================
            // 5. Constructor ile Dinamik Nesne Oluşturma
            // ============================================
            Console.WriteLine("--- 5. Activator ile Nesne Oluşturma ---");

            // Constructor'ı otomatik bul ve çağır
            object yeniKişi1 = Activator.CreateInstance(kisiType);
            Console.WriteLine($"Boş Constructor ile oluştu: {yeniKişi1 != null}");

            // Parametreli Constructor (Kişi sınıfında tanımlı olması gerekir)
            // object yeniKişi2 = Activator.CreateInstance(kisiType, "Ali", 30);
            Console.WriteLine();

            // ============================================
            // 6. Assembly ve Namespace Bilgisi
            // ============================================
            Console.WriteLine("--- 6. Assembly Bilgisi ---");

            Assembly assembly = kisiType.Assembly;
            Console.WriteLine($"Assembly: {assembly.GetName().Name}");
            Console.WriteLine($"Versiyon: {assembly.GetName().Version}");
            Console.WriteLine($"Namespace: {kisiType.Namespace}\n");

            // ============================================
            // 7. Type Türlerini Kontrol Etme
            // ============================================
            Console.WriteLine("--- 7. Type Kontrolleri ---");

            Console.WriteLine($"Is Reference Type: {!kisiType.IsValueType}");
            Console.WriteLine($"Is Value Type: {kisiType.IsValueType}");
            Console.WriteLine($"Is Class: {kisiType.IsClass}");
            Console.WriteLine($"Is Interface: {kisiType.IsInterface}");
            Console.WriteLine($"Is Abstract: {kisiType.IsAbstract}\n");

            // ============================================
            // 8. Attribute'ları Okuma
            // ============================================
            Console.WriteLine("--- 8. Custom Attributes ---");

            Type kullanıcıType = typeof(Kullanıcı);
            var classAttribute = kullanıcıType.GetCustomAttribute<BilgiAttribute>();
            Console.WriteLine($"Sınıf Attribute: {classAttribute?.Açıklama}");

            var propAttribute = kullanıcıType.GetProperty("KullanıcıAdı")?.GetCustomAttribute<BilgiAttribute>();
            Console.WriteLine($"Property Attribute: {propAttribute?.Açıklama}\n");

            // ============================================
            // 9. Generic Types
            // ============================================
            Console.WriteLine("--- 9. Generic Types ---");

            Type listType = typeof(List<int>);
            Type genericDef = listType.GetGenericTypeDefinition();

            Console.WriteLine($"Generic Type: {listType.Name}");
            Console.WriteLine($"Generic Definition: {genericDef.Name}");
            Console.WriteLine($"Generic Arguments: {string.Join(", ", listType.GetGenericArguments().Select(t => t.Name))}\n");

            // ============================================
            // 10. Type Inheritance Kontrolü
            // ============================================
            Console.WriteLine("--- 10. Inheritance Kontrolü ---");

            Type hayvanType = typeof(Hayvan);
            Type köpekType = typeof(Köpek);

            Console.WriteLine($"Köpek, Hayvan'dan mı türedi? {hayvanType.IsAssignableFrom(köpekType)}");
            Console.WriteLine($"Hayvan, Köpek'ten mi türedi? {köpekType.IsAssignableFrom(hayvanType)}\n");

            // ============================================
            // 11. Field Bilgisi
            // ============================================
            Console.WriteLine("--- 11. Fields ---");

            Type aracType = typeof(Araç);
            FieldInfo[] fields = aracType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            Console.WriteLine("Fields:");
            foreach (var field in fields)
            {
                Console.WriteLine($"  - {field.Name} ({field.FieldType.Name})");
            }
            Console.WriteLine();

            // ============================================
            // 12. Event Bilgisi
            // ============================================
            Console.WriteLine("--- 12. Events ---");

            Type düğmeType = typeof(Düğme);
            EventInfo[] events = düğmeType.GetEvents();

            Console.WriteLine("Events:");
            foreach (var evt in events)
            {
                Console.WriteLine($"  - {evt.Name}");
            }
            Console.WriteLine();

            // ============================================
            // ÖZET: GetType vs typeof
            // ============================================
            Console.WriteLine("=== GETYTYPE vs TYPEOF ÖZETI ===");
            Console.WriteLine(@"
GetType():
  ✓ Çalışma zamanında (Runtime) kullanılır
  ✓ Nesne instance'ı gerekli
  ✓ Polimorfik davranış gösterir (derived type döner)
  ✗ Null referans hatasına açık
  ✗ Daha yavaş (reflection yapması gerekir)

typeof:
  ✓ Derleme zamanında (Compile-time) kullanılır
  ✓ Nesneye ihtiyaç yok, sınıf adı yazılır
  ✓ Null güvenli
  ✓ Daha hızlı
  ✗ Base type döner (polimorfizm yok)

Örnek:
  object obj = new Kişi();
  GetType()  → Kişi      (derived type)
  typeof()   → object    (base type)
");
        }
    }
}