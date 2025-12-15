using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using AllulExpressDriverApi.Data;
using AllulExpressDriverApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DriverController : ControllerBase
{

    private readonly AppDbContext _db;
    public DriverController(AppDbContext db)
    {
        _db = db;

    }

    [HttpGet("getdriver/{id}")]
    public async Task<ActionResult<Drivers>> GetDriver(int id)
    {
        var driver = await _db.Drivers
            .Include(d => d.Cities)
            .FirstOrDefaultAsync(d => d.Id == id);

        if (driver == null)
            return NotFound(new { message = "Driver not found" });

        return Ok(driver);
    }



    [HttpPatch("Driver/{id}/changelanguage")]
    public async Task<IActionResult> UpdateLanguage(int id, [FromBody] LanguageUpdateRequest request)
    {
        var user = await _db.Drivers.FindAsync(id);

        if (user == null)
            return NotFound(new { message = "Driver not found" });

        user.Language = request.Language; // assuming "Language" column exists
        _db.Entry(user).State = EntityState.Modified;

        try
        {
            await _db.SaveChangesAsync();
            return Ok(new { message = "Language updated successfully" });
        }
        catch (DbUpdateException e)
        {
            return StatusCode(500, new { message = "Failed to update language", error = e.Message });
        }
    }


    public class LanguageUpdateRequest
    {
        public string Language { get; set; }
    }
}
