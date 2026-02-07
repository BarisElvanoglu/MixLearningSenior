using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

// 1. ADIM: Modeller (Değişmedi)
public class Author
{
    public int Id { get; set; }
    public string Name { get; set; }
    public virtual ICollection<Book> Books { get; set; } = new List<Book>();
}

public class Book
{
    public int Id { get; set; }
    public string Title { get; set; }
    public int AuthorId { get; set; }
    public virtual Author Author { get; set; }
}

// 2. ADIM: DbContext Tanımı
// Dışarıdan bağlantı alabilsin diye constructor ekledik.
public class LibraryContext : DbContext
{
    private readonly SqliteConnection _connection;

    public LibraryContext(SqliteConnection connection)
    {
        _connection = connection;
    }

    public DbSet<Author> Authors { get; set; }
    public DbSet<Book> Books { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        // Dışarıdaki bağlantıyı kullanıyoruz ki kapanmasın
        options.UseSqlite(_connection)
               .UseLazyLoadingProxies()
               .LogTo(Console.WriteLine, LogLevel.Information);
    }
}

class Program
{
    static void Main()
    {
        // --- KRİTİK NOKTA: Bağlantıyı manuel oluşturup açıyoruz ---
        using var connection = new SqliteConnection("Data Source=:memory:");
        connection.Open();

        // Context'e bu bağlantıyı veriyoruz
        using var db = new LibraryContext(connection);
        
        // Tabloları oluştur (Şimdi kalıcı olacak)
        db.Database.EnsureCreated();

        // Veri Ekleme
        Console.WriteLine("\n--- VERİLER EKLENİYOR ---");
        var author = new Author { Name = "Barış" };
        author.Books.Add(new Book { Title = "C# Rehberi" });
        author.Books.Add(new Book { Title = "EF Core Sanatı" });
        
        db.Authors.Add(author);
        db.SaveChanges(); // ARTIK PATLAMAZ!

        Console.WriteLine("\n--- 1. EAGER LOADING ÖRNEĞİ (Hevesli) ---");
        // Include ile kitapları tek seferde çekiyoruz (Tek bir JOIN sorgusu atar)
        var eagerAuthor = db.Authors.Include(a => a.Books).First();
        foreach (var book in eagerAuthor.Books)
        {
            Console.WriteLine($"Kitap: {book.Title} (Yazarla beraber geldi)");
        }

        Console.WriteLine("\n--- 2. LAZY LOADING ÖRNEĞİ (Tembel) ---");
        db.ChangeTracker.Clear(); // Önbelleği temizle ki Lazy yükleme tetiklensin
        
        var lazyAuthor = db.Authors.First(); // Sadece Author çekilir

        // Döngüye girdiğin an 'Books' için ekstra SQL atıldığını loglarda göreceksin
        foreach (var book in lazyAuthor.Books) 
        {
            Console.WriteLine($"Kitap: {book.Title} (Erişildiği an DB'den çekildi)");
        }
        
        // Bağlantı 'using' bloğu bittiğinde kapanacak ve hafıza temizlenecek.
    }
}