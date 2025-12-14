
using AllulExpressDriverApi;
using AllulExpressDriverApi.Data;
using AllulExpressDriverApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


[ApiController]
[Route("api/[controller]")]
public class LoginController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly TokenService _tokenService;

    public LoginController(AppDbContext db, TokenService tokenService)
    {
        _db = db;
        _tokenService = tokenService;
    }

    [AllowAnonymous]
    [HttpPost("loginDriver")]
    public async Task<IActionResult> LoginDriver([FromBody] LoginRequest request)
    {


        // 1 fetch user from DB
        var driver = await _db.Drivers
     .FirstOrDefaultAsync(u => u.Phonenum1 == request.Phonenum1);

        if (driver == null)
        {
            // user not found
            return Unauthorized(new { message = "Invalid phone or password" });
        }

        //  Verify the password (hashed)
        bool isValidPassword = BCrypt.Net.BCrypt.Verify(request.Password, driver.Password);

        if (!isValidPassword)
        {
            // password incorrect
            return Unauthorized(new { message = "Invalid phone or password" });
        }


        //  generate JWT
        var token = _tokenService.GenerateToken(driver);

        // 3️⃣ save in ValidTokens
        _db.ValidTokenDrivers.Add(new ValidTokenDrivers
        {
            driverid = driver.Id,
            Token = token,
            ExpiresAt = DateTime.UtcNow.AddDays(7)
        });
        await _db.SaveChangesAsync();

        // 4️ return token
        return Ok(new { Token = token, driver.Id, driver.Name, driver.Phonenum1, driver.Email, driver.Language });
    }

    [HttpGet("validate-token")]
    public async Task<IActionResult> ValidateToken([FromQuery] string token)
    {
        var tokenRecord = await _db.ValidTokenDrivers
            .FirstOrDefaultAsync(t => t.Token == token);

        if (tokenRecord == null)
            return Unauthorized(new { valid = false, reason = "Token not found" });

        if (tokenRecord.ExpiresAt < DateTime.UtcNow)
            return Unauthorized(new { valid = false, reason = "Token expired" });

        return Ok(new { valid = true });
    }

}
