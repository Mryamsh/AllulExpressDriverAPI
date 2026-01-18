
using AllulExpressDriverApi.Data;
using AllulExpressDriverApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class PostsController : ControllerBase
{
    private readonly AppDbContext _db;

    public PostsController(AppDbContext db)
    {
        _db = db;
    }
    [Authorize]
    [HttpGet("{driverId}/posts")]
    public async Task<IActionResult> GetPostsByDriver(
        int driverId,
        int pageNumber = 1,
        int pageSize = 10)
    {
        if (pageNumber <= 0 || pageSize <= 0)
            return BadRequest("Invalid pagination values.");

        var query = _db.Posts
            .AsNoTracking()
            .Where(p => p.DriverID == driverId);

        var totalCount = await query.CountAsync();

        if (totalCount == 0)
            return NotFound(new { message = "No posts found for this driver." });

        var posts = await query
            .OrderByDescending(p => p.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(p => new
            {
                Id = p.Id,
                p.Businessname,
                p.City,
                p.Phonenum2,
                p.Postnum,
                p.Numberofpieces,
                p.Price,
                p.Shipmentfee
            })
            .ToListAsync();

        return Ok(new
        {
            data = posts,
            pagination = new
            {
                totalItems = totalCount,
                pageNumber,
                pageSize,
                totalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            }
        });
    }


    // [HttpGet("by-qr")]
    // [Authorize] // Ensure client is authenticated
    // public async Task<IActionResult> GetPostByQrCode([FromQuery] string qrCode)
    // {
    //     // 1️⃣ Get the ClientId from JWT
    //     var clientIdClaim = User.FindFirst("Id")?.Value;
    //     if (clientIdClaim == null)
    //         return Unauthorized(new { message = "Invalid token" });

    //     int clientId = int.Parse(clientIdClaim);

    //     // 2️⃣ Find the post with this QR code for this client
    //     var post = await _db.Posts
    //         .FirstOrDefaultAsync(p => p.QrCode == qrCode && p.ClientId == clientId);

    //     if (post == null)
    //         return NotFound(new { message = "Post not found for this client" });

    //     return Ok(post);
    // }

    [HttpGet("by-qrcode")]
    public async Task<IActionResult> GetPostByQr([FromQuery] string qr, [FromQuery] int driverId)
    {
        Console.WriteLine("mtqrtext" + qr);
        var post = await _db.Posts.FirstOrDefaultAsync(p => p.Qrcodetext == qr);
        Console.WriteLine("mypost" + post);
        if (post == null)
            return NotFound("Post not found");

        if (post.DriverID != driverId)
            return BadRequest("This post does not belong to this driver");

        return Ok(post);
    }

    [HttpGet("{postId}/driver/{driverId}")]
    public async Task<IActionResult> GetPostById(int postId, int driverId)
    {
        // Fetch the post including the driver info
        var post = await _db.Posts
            .Include(p => p.Client) // Make sure you have a navigation property "Driver" in Post model
            .FirstOrDefaultAsync(p => p.Id == postId && p.DriverID == driverId);

        if (post == null)
        {
            return NotFound(new { message = "Post not found or not associated with this client." });
        }

        // Optional: return custom object instead of full EF entities
        var result = new
        {
            PostId = post.Id,
            post.Businessname,
            post.City,
            post.Phonenum1,
            post.Phonenum2,
            post.Price,
            post.Shipmentfee,
            post.Postnum,
            post.ChangeOrReturn,
            post.Numberofpieces,
            post.Exactaddress,
            post.Poststatus,
            post.Note,
            post.Savedate,


            Client = post.Client != null ? new
            {
                post.Client.Id,
                post.Client.Name,
                post.Client.Phonenum1,
                post.Client.Phonenum2,
                post.Client.Email,
                post.Client.Note,


            } : null


        };

        return Ok(result);
    }
    [HttpPatch("{postId}/status")]
    public async Task<IActionResult> TogglePostStatus(int postId, [FromBody] PostStatusUpdateRequest request)
    {
        var post = await _db.Posts.FindAsync(postId);
        if (post == null)
        {
            return NotFound(new { message = "Post not found" });
        }

        post.Poststatus = request.Status; // Update status
        await _db.SaveChangesAsync();

        return Ok(new
        {
            message = $"Post status updated to {post.Poststatus}",
            post.Id,
            post.Poststatus
        });
    }

}
