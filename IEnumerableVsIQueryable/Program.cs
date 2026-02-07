using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

var loggerFactory = LoggerFactory.Create(builder =>
{
    builder
        .AddConsole()
        .SetMinimumLevel(LogLevel.Information);
});

var connection = new SqliteConnection("DataSource=:memory:");
connection.Open();

var options = new DbContextOptionsBuilder<AppDbContext>()
    .UseSqlite(connection)
    .UseLoggerFactory(loggerFactory)
    .EnableSensitiveDataLogging()
    .Options;

using (var db = new AppDbContext(options))
{
    // Ensure DB and seed data
    db.Database.EnsureCreated();

    if (!db.People.Any())
    {
        db.People.AddRange(new[]
        {
            new Person { Name = "Ayşe", Age = 28 },
            new Person { Name = "Mehmet", Age = 34 },
            new Person { Name = "Fatma", Age = 42 },
            new Person { Name = "Can", Age = 22 },
        });
        db.SaveChanges();
    }

    Console.WriteLine("---- IQueryable example (server-side filtering) ----");
    // IQueryable: ifade ağaca (expression tree) dönüşür — SQL'e çevrilir ve DB'de filtrelenir
    IQueryable<Person> queryable = db.People.Where(p => p.Age > 30);
    Console.WriteLine("Expression tree:");
    Console.WriteLine(queryable.Expression); // gösterim
    var resultFromDb = queryable.ToList(); // burada SQL çalışır (ve console'da SQL logunu görürsünüz)
    Console.WriteLine("Result count (IQueryable executed on DB): " + resultFromDb.Count);
    foreach (var p in resultFromDb) Console.WriteLine($"{p.Name} ({p.Age})");

    Console.WriteLine("\n---- IEnumerable example (client-side filtering) ----");
    // IEnumerable: önce tüm veriler belleğe çekilir (ToList), sonra LINQ to Objects ile filtre uygulanır
    IEnumerable<Person> enumerable = db.People.ToList().Where(p => p.Age > 30);
    Console.WriteLine("Note: DB was already queried (ToList), filtering happens in memory afterwards.");
    Console.WriteLine("Result count (IEnumerable filtered in memory): " + enumerable.Count());
    foreach (var p in enumerable) Console.WriteLine($"{p.Name} ({p.Age})");
}

connection.Close();

class Person
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public int Age { get; set; }
}

class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public DbSet<Person> People { get; set; } = null!;
}