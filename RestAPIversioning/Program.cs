using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiVersioningDemo
{
    // ==================== VERİ MODELLERİ ====================
    // V1: Basit kullanıcı modeli
    public class UserV1
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    // V2: Genişletilmiş kullanıcı modeli
    public class UserV2
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    // ==================== MOCK VERİTABANI ====================
    public static class MockDatabase
    {
        public static List<UserV2> Users = new()
        {
            new UserV2 { Id = 1, FirstName = "Ahmet", LastName = "Yılmaz", Email = "ahmet@mail.com", CreatedDate = DateTime.Now.AddDays(-30) },
            new UserV2 { Id = 2, FirstName = "Fatma", LastName = "Kaya", Email = "fatma@mail.com", CreatedDate = DateTime.Now.AddDays(-15) },
            new UserV2 { Id = 3, FirstName = "Mehmet", LastName = "Demir", Email = "mehmet@mail.com", CreatedDate = DateTime.Now.AddDays(-7) }
        };
    }

    // ==================== API SERVİSLERİ ====================
    // V1 API Servisi
    public class UserApiV1
    {
        public UserV1 GetUser(int id)
        {
            var user = MockDatabase.Users.FirstOrDefault(u => u.Id == id);
            if (user == null)
                return null;

            return new UserV1
            {
                Id = user.Id,
                Name = $"{user.FirstName} {user.LastName}"
            };
        }

        public List<UserV1> GetAllUsers()
        {
            return MockDatabase.Users.Select(u => new UserV1
            {
                Id = u.Id,
                Name = $"{u.FirstName} {u.LastName}"
            }).ToList();
        }
    }

    // V2 API Servisi
    public class UserApiV2
    {
        public UserV2 GetUser(int id)
        {
            return MockDatabase.Users.FirstOrDefault(u => u.Id == id);
        }

        public List<UserV2> GetAllUsers()
        {
            return MockDatabase.Users.ToList();
        }

        public List<UserV2> GetUsersByDate(DateTime since)
        {
            return MockDatabase.Users.Where(u => u.CreatedDate >= since).ToList();
        }
    }

    // ==================== API ROUTER (Sürüm Kontrolü) ====================
    public class ApiRouter
    {
        private readonly UserApiV1 _apiV1 = new();
        private readonly UserApiV2 _apiV2 = new();

        public object HandleRequest(string path, Dictionary<string, string> queryParams)
        {
            // Strateji 1: URI Path Versioning (api/v1/users vs api/v2/users)
            if (path.StartsWith("/api/v1/users"))
                return HandleV1Request(path, queryParams);

            if (path.StartsWith("/api/v2/users"))
                return HandleV2Request(path, queryParams);

            // Strateji 2: Query String Versioning (api/users?version=1)
            if (path == "/api/users" && queryParams.ContainsKey("version"))
            {
                string version = queryParams["version"];
                if (version == "1")
                    return HandleV1Request(path, queryParams);
                if (version == "2")
                    return HandleV2Request(path, queryParams);
            }

            // Strateji 3: Header Versioning (Accept: application/json; version=1)
            if (path == "/api/users" && queryParams.ContainsKey("Accept-Version"))
            {
                string version = queryParams["Accept-Version"];
                if (version == "1")
                    return HandleV1Request(path, queryParams);
                if (version == "2")
                    return HandleV2Request(path, queryParams);
            }

            return new { error = "Bilinmeyen endpoint veya sürüm" };
        }

        private object HandleV1Request(string path, Dictionary<string, string> queryParams)
        {
            if (path.Contains("/users/") && int.TryParse(path.Split('/').Last(), out int id))
            {
                var user = _apiV1.GetUser(id);
                if (user == null)
                    return new { error = "Kullanıcı bulunamadı" };
                return user;
            }

            return new
            {
                version = "1.0",
                data = _apiV1.GetAllUsers(),
                info = "Basit kullanıcı bilgisi (Name alanı)"
            };
        }

        private object HandleV2Request(string path, Dictionary<string, string> queryParams)
        {
            if (path.Contains("/users/") && int.TryParse(path.Split('/').Last(), out int id))
            {
                var user = _apiV2.GetUser(id);
                if (user == null)
                    return new { error = "Kullanıcı bulunamadı" };
                return user; ;
            }

            // V2: Yeni filtreleme yetenekeri
            if (queryParams.ContainsKey("since") && DateTime.TryParse(queryParams["since"], out DateTime date))
            {
                return new
                {
                    version = "2.0",
                    data = _apiV2.GetUsersByDate(date),
                    info = "Belirtilen tarihten sonra oluşturulan kullanıcılar"
                };
            }

            return new
            {
                version = "2.0",
                data = _apiV2.GetAllUsers(),
                info = "Detaylı kullanıcı bilgisi (FirstName, LastName, Email, CreatedDate)"
            };
        }
    }

    // ==================== MAIN - DEMO ====================
    class Program
    {
        static void Main()
        {
            Console.WriteLine("╔════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║          REST API VERSIONING ÖRNEĞİ - CONSOLE             ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════════╝\n");

            var router = new ApiRouter();

            // Test 1: URI Path Versioning - V1
            Console.WriteLine("📌 [Test 1] URI Versioning: /api/v1/users");
            var result1 = router.HandleRequest("/api/v1/users", new());
            PrintJson(result1);
            Console.WriteLine();

            // Test 2: URI Path Versioning - V2
            Console.WriteLine("📌 [Test 2] URI Versioning: /api/v2/users");
            var result2 = router.HandleRequest("/api/v2/users", new());
            PrintJson(result2);
            Console.WriteLine();

            // Test 3: Query String Versioning
            Console.WriteLine("📌 [Test 3] Query String: /api/users?version=2");
            var result3 = router.HandleRequest("/api/users", new Dictionary<string, string> { { "version", "2" } });
            PrintJson(result3);
            Console.WriteLine();

            // Test 4: Header Versioning
            Console.WriteLine("📌 [Test 4] Header: /api/users (Accept-Version: 1)");
            var result4 = router.HandleRequest("/api/users", new Dictionary<string, string> { { "Accept-Version", "1" } });
            PrintJson(result4);
            Console.WriteLine();

            // Test 5: V2 Özel Özellik - Tarih Filtresi
            Console.WriteLine("📌 [Test 5] V2 Özel: /api/v2/users?since=2026-02-01");
            var result5 = router.HandleRequest("/api/v2/users", new Dictionary<string, string> { { "since", "2026-02-01" } });
            PrintJson(result5);
            Console.WriteLine();

            // Test 6: Belirli Kullanıcı - V1
            Console.WriteLine("📌 [Test 6] Belirli Kullanıcı V1: /api/v1/users/1");
            var result6 = router.HandleRequest("/api/v1/users/1", new());
            PrintJson(result6);
            Console.WriteLine();

            // Test 7: Belirli Kullanıcı - V2
            Console.WriteLine("📌 [Test 7] Belirli Kullanıcı V2: /api/v2/users/2");
            var result7 = router.HandleRequest("/api/v2/users/2", new());
            PrintJson(result7);

            Console.WriteLine("\n╔════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║          VERSIONING STRATEJİLERİ ÖZET                     ║");
            Console.WriteLine("╠════════════════════════════════════════════════════════════╣");
            Console.WriteLine("║ 1. URI Path:  /api/v1/users (En yaygın, açık ve net)     ║");
            Console.WriteLine("║ 2. Query:     /api/users?version=1 (Basit, flexible)    ║");
            Console.WriteLine("║ 3. Header:    Accept-Version: 1 (Temiz URL)              ║");
            Console.WriteLine("║ 4. Content:   Accept: application/vnd.api.v1+json       ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════════╝");
        }

        static void PrintJson(object obj)
        {
            var json = System.Text.Json.JsonSerializer.Serialize(obj,
                new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
            Console.WriteLine(json);
        }
    }
}