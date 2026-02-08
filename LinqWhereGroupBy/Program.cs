using System;
using System.Collections.Generic;
using System.Linq;

// Örnek veri modeli
class Student
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int Age { get; set; }
    public string Department { get; set; }
    public double GPA { get; set; }
}

class Program
{
    static void Main()
    {
        // Test verileri
        var students = new List<Student>
        {
            new Student { Id = 1, Name = "Ahmet", Age = 20, Department = "Bilgisayar", GPA = 3.5 },
            new Student { Id = 2, Name = "Ayşe", Age = 21, Department = "Elektrik", GPA = 3.8 },
            new Student { Id = 3, Name = "Mehmet", Age = 20, Department = "Bilgisayar", GPA = 3.2 },
            new Student { Id = 4, Name = "Fatma", Age = 22, Department = "Elektrik", GPA = 3.9 },
            new Student { Id = 5, Name = "Ali", Age = 21, Department = "Makine", GPA = 3.1 },
            new Student { Id = 6, Name = "Zeynep", Age = 20, Department = "Bilgisayar", GPA = 3.7 }
        };

        Console.WriteLine("=== LINQ OPERATÖRLERI ÖRNEK ===\n");

        // 1. WHERE OPERATÖRÜ
        // Açıklama: Belirtilen koşulu sağlayan elemanları filtreler
        Console.WriteLine("1. WHERE - Yaşı 21'den büyük öğrenciler:");
        var studentsOver21 = students.Where(s => s.Age > 21);
        foreach (var student in studentsOver21)
        {
            Console.WriteLine($"   {student.Name} - Yaş: {student.Age}");
        }

        Console.WriteLine("\n2. WHERE - GPA'sı 3.5'ten yüksek olan öğrenciler:");
        var highGpaStudents = students.Where(s => s.GPA >= 3.5);
        foreach (var student in highGpaStudents)
        {
            Console.WriteLine($"   {student.Name} - GPA: {student.GPA}");
        }

        // 2. SELECT OPERATÖRÜ
        // Açıklama: Koleksiyonun her elemanını dönüştürerek yeni bir koleksiyon oluşturur
        Console.WriteLine("\n3. SELECT - Sadece öğrenci adlarını seç:");
        var studentNames = students.Select(s => s.Name);
        foreach (var name in studentNames)
        {
            Console.WriteLine($"   {name}");
        }

        Console.WriteLine("\n4. SELECT - Anonim tip kullanarak ad ve departman bilgisi:");
        var studentInfo = students.Select(s => new { s.Name, s.Department });
        foreach (var info in studentInfo)
        {
            Console.WriteLine($"   {info.Name} - {info.Department}");
        }

        Console.WriteLine("\n5. SELECT - Özel formatla dönüştür:");
        var formattedInfo = students.Select(s => $"{s.Name} ({s.Department})");
        foreach (var info in formattedInfo)
        {
            Console.WriteLine($"   {info}");
        }

        // 3. GROUPBY OPERATÖRÜ
        // Açıklama: Elemanları belirtilen anahtara göre gruplandırır
        Console.WriteLine("\n6. GROUPBY - Departmana göre grupla:");
        var studentsByDepartment = students.GroupBy(s => s.Department);
        foreach (var group in studentsByDepartment)
        {
            Console.WriteLine($"   {group.Key} ({group.Count()} öğrenci):");
            foreach (var student in group)
            {
                Console.WriteLine($"      - {student.Name} (GPA: {student.GPA})");
            }
        }

        Console.WriteLine("\n7. GROUPBY - Yaşa göre grupla ve bilgi göster:");
        var studentsByAge = students.GroupBy(s => s.Age);
        foreach (var group in studentsByAge)
        {
            Console.WriteLine($"   Yaş {group.Key}: {group.Count()} öğrenci");
            var names = group.Select(s => s.Name).ToList();
            Console.WriteLine($"      Adlar: {string.Join(", ", names)}");
        }

        // 4. KOMBINASYON ÖRNEKLERI
        // Açıklama: Operatörleri birleştirerek karmaşık sorgular oluşturabilirsiniz
        Console.WriteLine("\n8. KOMBINASYON - WHERE + SELECT + ORDERBY:");
        var result1 = students
            .Where(s => s.Department == "Bilgisayar")  // Bilgisayar bölümünü filtrele
            .OrderByDescending(s => s.GPA)              // GPA'ya göre sırala
            .Select(s => new { s.Name, s.GPA });        // Sadece ad ve GPA'yı seç

        Console.WriteLine("   Bilgisayar bölümü - GPA sırasına göre:");
        foreach (var item in result1)
        {
            Console.WriteLine($"      {item.Name} - GPA: {item.GPA}");
        }

        Console.WriteLine("\n9. KOMBINASYON - WHERE + GROUPBY + SELECT:");
        var result2 = students
            .Where(s => s.GPA >= 3.5)                          // GPA 3.5 ve üzeri
            .GroupBy(s => s.Department)                        // Departmana göre grupla
            .Select(g => new
            {
                Department = g.Key,
                Count = g.Count(),
                AvgGPA = g.Average(s => s.GPA)
            });

        Console.WriteLine("   Bölümlere göre başarılı öğrenciler:");
        foreach (var item in result2)
        {
            Console.WriteLine($"      {item.Department}: {item.Count} öğrenci, Ort. GPA: {item.AvgGPA:F2}");
        }

        // 5. AGGREGATE İŞLEMLERİ
        // Açıklama: Koleksiyon üzerinde toplu işlemler yapmanız sağlar
        Console.WriteLine("\n10. AGGREGATE - İstatistikler:");
        Console.WriteLine($"   Toplam öğrenci sayısı: {students.Count()}");
        Console.WriteLine($"   Ortalama GPA: {students.Average(s => s.GPA):F2}");
        Console.WriteLine($"   En yüksek GPA: {students.Max(s => s.GPA)}");
        Console.WriteLine($"   En düşük GPA: {students.Min(s => s.GPA)}");
        Console.WriteLine($"   Tüm öğrencilerin toplam yaşı: {students.Sum(s => s.Age)}");
    }
}