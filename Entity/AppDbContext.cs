using Microsoft.EntityFrameworkCore;

namespace EventEase_Management.Entity
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<Venue> VenueManager { get; set; }
        public DbSet<Event> EventManager { get; set; }
         public DbSet<Booking> BookingManager { get; set; }
        public DbSet<EventType> EventTypes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // keep EF base config

            modelBuilder.Entity<EventType>().HasData(
                new EventType { EventTypeID = 1, Name = "Conference" },
                new EventType { EventTypeID = 2, Name = "Workshop" },
                new EventType { EventTypeID = 3, Name = "Seminar" },
                new EventType { EventTypeID = 4, Name = "Webinar" }
            );
            modelBuilder.Entity<Venue>()
              .HasKey(v => v.VenueID);

            modelBuilder.Entity<Event>()
                .HasKey(e => e.EventID);

            modelBuilder.Entity<Event>()
              .HasOne(e => e.Venue)
              .WithMany()
              .HasForeignKey(e => e.VenueID)
              .OnDelete(DeleteBehavior.Cascade);
           modelBuilder.Entity<Event>()
             .HasOne(e => e.EventType)
             .WithMany(et => et.Events)
             .HasForeignKey(e => e.EventTypeID)
             .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Booking>()
            .HasOne(b => b.Venuess)
            .WithMany()
            .HasForeignKey(b => b.VenueBooking)
            .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Eventss)
                .WithMany()
                .HasForeignKey(b => b.EventBooking)
                .OnDelete(DeleteBehavior.Restrict);



        }
    }
}
