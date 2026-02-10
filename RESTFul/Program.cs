using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddControllers();

        var app = builder.Build();

        app.MapControllers();
        app.Run();
    }
}

// Örnek Controller
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private static List<User> users = new()
    {
        new User { Id = 1, Name = "Ahmet", Email = "ahmet@example.com" },
        new User { Id = 2, Name = "Fatma", Email = "fatma@example.com" }
    };

    // GET: api/users - Tüm kullanıcıları al
    [HttpGet]
    public ActionResult<IEnumerable<User>> GetAllUsers()
    {
        return Ok(users);
    }

    // GET: api/users/1 - Belirli bir kullanıcıyı al
    [HttpGet("{id}")]
    public ActionResult<User> GetUserById(int id)
    {
        var user = users.FirstOrDefault(u => u.Id == id);
        if (user == null)
            return NotFound();
        return Ok(user);
    }

    // POST: api/users - Yeni kullanıcı oluştur
    [HttpPost]
    public ActionResult<User> CreateUser([FromBody] User newUser)
    {
        newUser.Id = users.Max(u => u.Id) + 1;
        users.Add(newUser);
        return CreatedAtAction(nameof(GetUserById), new { id = newUser.Id }, newUser);
    }

    // PUT: api/users/1 - Kullanıcıyı tamamen güncelle
    [HttpPut("{id}")]
    public ActionResult UpdateUser(int id, [FromBody] User updatedUser)
    {
        var user = users.FirstOrDefault(u => u.Id == id);
        if (user == null)
            return NotFound();
        
        user.Name = updatedUser.Name;
        user.Email = updatedUser.Email;
        return Ok(user);
    }

    // PATCH: api/users/1 - Kullanıcıyı kısmen güncelle
    [HttpPatch("{id}")]
    public ActionResult PatchUser(int id, [FromBody] JsonPatchDocument<User> patchDoc)
    {
        var user = users.FirstOrDefault(u => u.Id == id);
        if (user == null)
            return NotFound();
        
        patchDoc.ApplyTo(user);
        return Ok(user);
    }

    // DELETE: api/users/1 - Kullanıcıyı sil
    [HttpDelete("{id}")]
    public ActionResult DeleteUser(int id)
    {
        var user = users.FirstOrDefault(u => u.Id == id);
        if (user == null)
            return NotFound();
        
        users.Remove(user);
        return NoContent();
    }
}

public class User
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
}