// See https://aka.ms/new-console-template for more information


// Index ve Range Operatörleri Örnekleri

// Örnek dizi
//                0   1  2   3  4   5   6  7   8           
int[] sayilar = { 10, 20, 30, 40, 50, 60, 70, 80, 90 };
string metin = "Merhaba Dünya";

Console.WriteLine("=== INDEX OPERATÖRÜ (^) ===\n");

// Son elemanı al (^1)
Console.WriteLine($"Dizinin son elemanı (^1): {sayilar[^1]}");

// Sondan ikinci elemanı al (^2)
Console.WriteLine($"Dizinin sondan ikinci elemanı (^2): {sayilar[^2]}");

// Sondan üçüncü elemanı al (^3)
Console.WriteLine($"Dizinin sondan üçüncü elemanı (^3): {sayilar[^3]}");

// String'in son karakteri
Console.WriteLine($"Metnin son karakteri (^1): {metin[^1]}");

Console.WriteLine("\n=== RANGE OPERATÖRÜ (..) ===\n");

// 2. indeksten 5. indekse kadar (5. dahil değil)
int[] aralik1 = sayilar[2..5];
Console.WriteLine($"sayilar[2..5]: {string.Join(", ", aralik1)}");

// Başlangıçtan 3. indekse kadar (3. dahil değil)
int[] aralik2 = sayilar[..3];
Console.WriteLine($"sayilar[..3]: {string.Join(", ", aralik2)}");

// 5. indeksten sona kadar
int[] aralik3 = sayilar[5..];
Console.WriteLine($"sayilar[5..]: {string.Join(", ", aralik3)}");

// Tüm diziyi kopyala
int[] aralik4 = sayilar[..];
Console.WriteLine($"sayilar[..]: {string.Join(", ", aralik4)}");

// Sondan 3 eleman
int[] aralik5 = sayilar[^3..];
Console.WriteLine($"sayilar[^3..]: {string.Join(", ", aralik5)}");

// Başlangıçtan sondan 2. elemana kadar
int[] aralik6 = sayilar[..^2];
Console.WriteLine($"sayilar[..^2]: {string.Join(", ", aralik6)}");

Console.WriteLine("\n=== STRING RANGE ÖRNEĞİ ===\n");

string kelime = "Programlama";

// İlk 4 karakter
Console.WriteLine($"Kelime[..4]: {kelime[..4]}");

// Son 3 karakter
Console.WriteLine($"Kelime[^3..]: {kelime[^3..]}");

// Ortasındaki karakterler (2. indeksten 7. indekse kadar)
Console.WriteLine($"Kelime[2..7]: {kelime[2..7]}");
