using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Cineplex.Models;

namespace Cineplex.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);


            builder.Entity<Show>()
                .HasOne(s => s.Movie)
                .WithMany(m => m.Shows);

            builder.Entity<Show>()
                .HasOne(s => s.Cinema)
                .WithMany(c => c.Shows);

            builder.Entity<Booking>()
                .HasOne(b => b.Show)
                .WithMany(s => s.Bookings);

            builder.Entity<Booking>()
                .HasOne(b => b.Order)
                .WithMany(o => o.Bookings);

            builder.Entity<Ticket>().HasOne(t => t.PricingType);

            builder.Entity<Movie>().HasMany(m => m.Images);
            builder.Entity<Event>().HasMany(e => e.Images);
            builder.Entity<Cinema>().HasMany(c => c.Images);
            builder.Entity<Seat>().HasKey(s => new { s.BookingId, s.SeatNumber });

            builder.Entity<Movie>().HasOne(m => m.Rating);

            builder.Entity<Booking>().HasMany(b => b.Seats);
            builder.Entity<Booking>().HasMany(b => b.Tickets);

            builder.Entity<Order>()
                .HasOne(o => o.OrderedBy)
                .WithMany(o => o.Orders);

            builder.Entity<Enquiry>()
                .HasOne(e => e.Event)
                .WithMany(en => en.Enquiries);

            builder.Entity<Order>().Property<DateTime>("CreatedAt");

        }

        public DbSet<Movie> Movies { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<Cinema> Cinemas { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Show> Shows { get; set; }
        public DbSet<PricingType> PricingTypes { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Enquiry> Enquiries { get; set; }
        public DbSet<Rating> Ratings { get; set; }
        public DbSet<Seat> Seats { get; set; }

    }
}
