using System;

// Tuple vs ValueTuple örneği
class Program
{
    static void Main()
    {
        Console.WriteLine("=== TUPLE vs VALUE TUPLE ===\n");

        // 1. ValueTuple - Modern ve önerilen yol
        Console.WriteLine("1. ValueTuple Kullanımı:");
        var (name, age, email) = GetUserInfoValueTuple();
        Console.WriteLine($"   Ad: {name}, Yaş: {age}, Email: {email}\n");

        // 2. Tuple - Eski yol
        Console.WriteLine("2. Tuple Kullanımı:");
        var userTuple = GetUserInfoTuple();
        Console.WriteLine($"   Ad: {userTuple.Item1}, Yaş: {userTuple.Item2}, Email: {userTuple.Item3}\n");

        // 3. ValueTuple - Named elements ile
        Console.WriteLine("3. ValueTuple (Named Elements):");
        var user = GetUserWithNames();
        Console.WriteLine($"   Ad: {user.FirstName}, Yaş: {user.Age}, Email: {user.Email}\n");

        // 4. Matematiksel işlem örneği
        Console.WriteLine("4. Matematiksel İşlem:");
        var (quotient, remainder) = DivideNumbers(17, 5);
        Console.WriteLine($"   17 ÷ 5 = {quotient}, Kalan = {remainder}\n");

        // 5. Koşullu döndürme örneği
        Console.WriteLine("5. Başarı/Başarısızlık ile Döndürme:");
        var (success, message, value) = TryParseNumber("123");
        Console.WriteLine($"   Başarılı: {success}, Mesaj: {message}, Değer: {value}\n");

        // 6. Değerleri sakladığımızda
        Console.WriteLine("6. ValueTuple Saklama:");
        (string firstName, string lastName, int birthYear) person = ("Barış", "Elvanoglu", 1995);
        Console.WriteLine($"   {person.firstName} {person.lastName} ({DateTime.Now.Year - person.birthYear} yaşında)");
    }

    // ValueTuple döndüren metot (Modern)
    static (string, int, string) GetUserInfoValueTuple()
    {
        return ("Ahmet", 25, "ahmet@email.com");
    }

    // Tuple döndüren metot (Eski)
    static Tuple<string, int, string> GetUserInfoTuple()
    {
        return new Tuple<string, int, string>("Mehmet", 30, "mehmet@email.com");
    }

    // Named ValueTuple (En iyisi)
    static (string FirstName, int Age, string Email) GetUserWithNames()
    {
        return ("Fatma", 28, "fatma@email.com");
    }

    // Bölme işlemi
    static (int Quotient, int Remainder) DivideNumbers(int dividend, int divisor)
    {
        return (dividend / divisor, dividend % divisor);
    }

    // Try-Catch benzeri dönüş
    static (bool Success, string Message, int Value) TryParseNumber(string input)
    {
        if (int.TryParse(input, out int result))
        {
            return (true, "Başarıyla dönüştürüldü", result);
        }
        return (false, "Dönüştürme başarısız", 0);
    }
}