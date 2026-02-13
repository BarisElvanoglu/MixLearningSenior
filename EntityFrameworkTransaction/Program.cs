using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Threading.Tasks;

namespace TransactionDemo
{
    // Basit entity'ler
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
    }

    public class Order
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public decimal Amount { get; set; }
        public User? User { get; set; }
    }

    // DbContext
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users => Set<User>();
        public DbSet<Order> Orders => Set<Order>();

        private readonly SqliteConnection _connection;

        public AppDbContext(SqliteConnection connection)
        {
            _connection = connection;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(_connection);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().ToTable("Users");
            modelBuilder.Entity<Order>().ToTable("Orders");
            modelBuilder.Entity<Order>()
                .HasOne(o => o.User)
                .WithMany()
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }

    public class Program
    {
        // Main içeriyor: async örnekler için Task Main
        public static async Task Main(string[] args)
        {
            // In-memory SQLite: bağlantıyı açık tutmak gerekiyor
            using var connection = new SqliteConnection("Data Source=:memory:");
            connection.Open();

            // DbContext oluştur ve schema'yı hazırla
            using (var context = new AppDbContext(connection))
            {
                context.Database.EnsureCreated();
            }

            // 1) SaveChanges otomatik transaction davranışı
            Console.WriteLine("1) SaveChanges otomatik transaction:");
            using (var context = new AppDbContext(connection))
            {
                var user = new User { Name = "Ahmet" };
                var order = new Order { Amount = 100m, User = user };

                // Her iki ekleme de SaveChanges ile tek transaction içinde uygulanır
                context.Add(user);
                context.Add(order);
                context.SaveChanges();

                Console.WriteLine($"  Users: {await context.Users.CountAsync()}, Orders: {await context.Orders.CountAsync()}");
            }

            // 2) Açık (explicit) transaction ile birden fazla SaveChanges kontrolü
            Console.WriteLine("\n2) Explicit transaction (BeginTransaction):");
            using (var context = new AppDbContext(connection))
            {
                using var transaction = context.Database.BeginTransaction();
                try
                {
                    var user = new User { Name = "Fatma" };
                    context.Users.Add(user);
                    context.SaveChanges(); // burada user.Id set edilir

                    // Sonraki işlemde exception oluşturacak bir durumı simüle edelim
                    var order = new Order { Amount = 200m, UserId = user.Id };
                    context.Orders.Add(order);
                    // Uncomment aşağıdaki satırı hata simülasyonu için:
                    //throw new Exception("Simule hata");

                    context.SaveChanges();

                    transaction.Commit();
                    Console.WriteLine("  Transaction commit edildi.");
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    Console.WriteLine($"  Hata: {ex.Message} - Transaction rollback edildi.");
                }

                Console.WriteLine($"  Users: {await context.Users.CountAsync()}, Orders: {await context.Orders.CountAsync()}");
            }

            // 3) Savepoint ile kısmi rollback
            Console.WriteLine("\n3) Savepoint ile kısmi rollback:");
            using (var context = new AppDbContext(connection))
            {
                using var transaction = context.Database.BeginTransaction();
                try
                {
                    // İşlem 1
                    var user = new User { Name = "Cem" };
                    context.Users.Add(user);
                    context.SaveChanges();

                    // Savepoint oluştur
                    transaction.CreateSavepoint("sp_after_user");
                    Console.WriteLine("  Savepoint oluşturuldu.");

                    // İşlem 2 - hata olursa sadece bu kısmı geri alacağız
                    var order = new Order { Amount = 500m, UserId = user.Id };
                    context.Orders.Add(order);
                    // Simule hata için aşağıyı açabilirsiniz:
                    // throw new Exception("Hata: order kaydı başarısız");

                    context.SaveChanges();

                    transaction.Commit();
                    Console.WriteLine("  Tüm işlemler commit edildi.");
                }
                catch (Exception ex)
                {
                    // Sadece savepoint sonrası işlemleri geri al
                    transaction.RollbackToSavepoint("sp_after_user");
                    transaction.Commit(); // geri kalan değişiklikleri (user ekleme) bırakmak istiyorsak commit edebiliriz
                    Console.WriteLine($"  Hata: {ex.Message} - Savepoint'e dönüldü, önceki değişiklikler korunuyor.");
                }

                Console.WriteLine($"  Users: {await context.Users.CountAsync()}, Orders: {await context.Orders.CountAsync()}");
            }

            // 4) Async transaction örneği
            Console.WriteLine("\n4) Async transaction (BeginTransactionAsync):");
            using (var context = new AppDbContext(connection))
            {
                await using var transaction = await context.Database.BeginTransactionAsync();
                try
                {
                    var user = new User { Name = "Deniz" };
                    context.Users.Add(user);
                    await context.SaveChangesAsync();

                    var order = new Order { Amount = 750m, UserId = user.Id };
                    context.Orders.Add(order);
                    await context.SaveChangesAsync();

                    await transaction.CommitAsync();
                    Console.WriteLine("  Async transaction commit edildi.");
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    Console.WriteLine($"  Hata: {ex.Message} - Async rollback edildi.");
                }

                Console.WriteLine($"  Users: {await context.Users.CountAsync()}, Orders: {await context.Orders.CountAsync()}");
            }

            Console.WriteLine("\nDemo tamamlandı.");
        }
    }
}