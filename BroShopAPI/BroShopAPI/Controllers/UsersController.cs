using BroShopAPI.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly AppDbContext _context;

    public UsersController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
    {
        if (loginRequest == null || string.IsNullOrEmpty(loginRequest.Identifier))
            return BadRequest("Пустой запрос");

        // Ищем пользователя: проверяем совпадение Identifier с Login ИЛИ с Email
        var user = await _context.Users
            .FirstOrDefaultAsync(u => (u.Login == loginRequest.Identifier || u.Email == loginRequest.Identifier)
                                       && u.Password == loginRequest.Password);

        if (user == null)
        {
            return Unauthorized(new { message = "Неверные данные для входа" });
        }

        // Возвращаем объект, который MAUI сможет прочитать
        return Ok(new
        {
            user.UserId,
            user.FullName,
            user.Login,
            user.Email,
            user.RoleId
        });
    }

    public class LoginRequest
    {
        // Называем поле Identifier, так как там может быть и то, и другое
        public string Identifier { get; set; }
        public string Password { get; set; }
    }
}