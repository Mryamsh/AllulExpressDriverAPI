using System;

namespace AllulExpressDriverApi.Models
{
    public class ValidTokenDrivers
    {
        public int Id { get; set; }              // Primary Key
        public int driverid { get; set; }          // Link to Users table
        public required string Token { get; set; }        // JWT string
        public DateTime ExpiresAt { get; set; }  // Expiry time
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Default now

        // Navigation property (if you have a User entity)

    }
}
