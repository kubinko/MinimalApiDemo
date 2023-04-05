using Microsoft.EntityFrameworkCore;

namespace MinimalApi.Database
{
    public class AttendanceDb : DbContext
    {
        public AttendanceDb(DbContextOptions<AttendanceDb> context) : base(context)
        {

        }

        public DbSet<Attendee> Attendees { get; set; } = default!;
    }
}
