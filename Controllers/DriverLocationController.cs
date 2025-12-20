using AllulExpressDriverApi.Data;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/driver")]
public class DriverLocationController : ControllerBase
{
    private readonly AppDbContext _db;

    public DriverLocationController(AppDbContext db)
    {
        _db = db;
    }

    [HttpPost("location")]
    public async Task<IActionResult> UpdateLocation(
        [FromBody] DriverLocationDto dto)
    {
        var driver = await _db.Drivers.FindAsync(dto.Id);
        if (driver == null)
            return NotFound();

        driver.Latitude = dto.Latitude;
        driver.Longitude = dto.Longitude;
        driver.LastUpdated = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return Ok();
    }
}
