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
    [HttpGet("{id}")]
    public async Task<IActionResult> GetDriverById(int id)
    {
        var driver = await _db.Drivers
            .Include(d => d.Cities)
            .Where(d => d.Id == id)
            .Select(d => new DriverDto
            {
                Id = d.Id,
                Name = d.Name,
                Email = d.Email,
                Phonenum1 = d.Phonenum1,
                Phonenum2 = d.Phonenum2,
                Paymentpayed = d.Paymentpayed,
                Paymentremained = d.Paymentremained,
                Arrivedpost = d.Arrivedpost,
                Remainedpost = d.Remainedpost,
                Vehicledetail = d.Vehicledetail,
                Cities = d.Cities
                    .Select(c => new CityDto { Id = c.Id, City = c.City })
                    .ToList(),
                IsActive = d.IsActive,
                IDimagefront = d.IDimagefront,
                IDimageback = d.IDimageback,
                Savedate = d.Savedate,
                Note = d.Note ?? ""
            })
            .FirstOrDefaultAsync();

        if (driver == null)
            return NotFound("Driver not found");

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
