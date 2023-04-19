using Microsoft.EntityFrameworkCore;

namespace MinimalApi.Database
{
    public class AttendanceDb : DbContext
    {
        public AttendanceDb(DbContextOptions<AttendanceDb> context) : base(context)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            if (!this.Database.IsInMemory())
            {
                modelBuilder.Entity<Attendee>()
                    .Property(x => x.Id).ValueGeneratedOnAdd();
            }
        }

        public DbSet<Attendee> Attendees { get; set; } = default!;
    }
}
