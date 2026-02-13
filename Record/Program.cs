using System;

class Program
{
    static void Main()
    {
        Console.WriteLine("=== record ve class (C# 9+) ===\n");

        // Positional record (kısa sözdizimi, değer-temelli eşitlik)
        var r1 = new PersonRecord("Ali", "Yılmaz", 30);
        var r2 = new PersonRecord("Ali", "Yılmaz", 30);

        // Normal class (varsayılan olarak referans-eşitliği)
        var c1 = new PersonClass("Ali", "Yılmaz", 30);
        var c2 = new PersonClass("Ali", "Yılmaz", 30);

        Console.WriteLine("Record'lar:");
        Console.WriteLine($" r1: {r1}");                       // record tarafından anlamlı ToString sağlanır
        Console.WriteLine($" r2: {r2}");
        Console.WriteLine($" r1 == r2 ? {r1 == r2}  (değer-temelli eşitlik)");
        Console.WriteLine();

        Console.WriteLine("Class'lar:");
        Console.WriteLine($" c1: {c1}");                       // varsayılan ToString (tip adı) unless overridden
        Console.WriteLine($" c2: {c2}");
        Console.WriteLine($" c1 == c2 ? {c1 == c2}  (referans-eşitliği)");
        Console.WriteLine();

        // with: non-destructive mutation (var olan kaydı klonlayıp değişiklik uygulama)
        var r3 = r1 with { Age = 31 };
        Console.WriteLine("Record 'with' örneği:");
        Console.WriteLine($" r1: {r1}");
        Console.WriteLine($" r3: {r3}");                       // r1 değişmez, r3 yeni bir record
        Console.WriteLine();

        // Deconstruct (positional record'lar için otomatik üretilir)
        var (first, last, age) = r3;
        Console.WriteLine("Deconstruction (ayrıştırma):");
        Console.WriteLine($" first = {first}, last = {last}, age = {age}");
    }
}

// Positional record: birincil kurucu, otomatik Deconstruct, Equals, GetHashCode, ToString üretir
public record PersonRecord(string FirstName, string LastName, int Age);

// Aynı veriye sahip klasik bir sınıf (override yoksa referans-eşitliği ve varsayılan ToString)
public class PersonClass
{
    public string FirstName { get; }
    public string LastName { get; }
    public int Age { get; }

    public PersonClass(string firstName, string lastName, int age)
    {
        FirstName = firstName;
        LastName = lastName;
        Age = age;
    }
}