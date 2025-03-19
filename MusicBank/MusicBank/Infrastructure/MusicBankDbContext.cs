using MusicBank.Models;
using MusicBank.Domain;
using Microsoft.EntityFrameworkCore;

namespace MusicBank.Data;

public interface IMusicBankDbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Event> Events { get; set; }
    public DbSet<TicketReservation> TicketReservations { get; set; }
}

public partial class MusicBankDbContext : DbContext, IMusicBankDbContext
{
    public MusicBankDbContext(DbContextOptions<MusicBankDbContext> options) : base(options) { }
    
    public virtual DbSet<User> Users { get; set; }
    public virtual DbSet<Event> Events { get; set; }
    public virtual DbSet<TicketReservation> TicketReservations { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //UserDTO mapping
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("user");
            entity.HasKey(e => e.UserId);
            
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.PhoneNumber).HasColumnName("phone_number");
            entity.Property(e => e.Email).HasColumnName("email");
            entity.HasMany(e => e.TicketReservations)
                .WithOne(tr => tr.User)
                .HasForeignKey(tr => tr.UserId);

        });
        
        //EventDTO mapping
        modelBuilder.Entity<Event>(entity =>
        {
            entity.ToTable("event");
            entity.HasKey(e => e.EventId);

            entity.Property(e => e.EventId).HasColumnName("event_id");
            entity.Property(e => e.EventName).HasColumnName("event_name");
            entity.Property(e => e.EventVenue).HasColumnName("event_venue");
            entity.Property(e => e.EventDate).HasColumnName("event_date");
            
        });
        
        //TicketReservationDTO mapping
        modelBuilder.Entity<TicketReservation>(entity =>
        {
            entity.ToTable("ticket_reservation");
            entity.HasKey(e => e.TicketReservationId);

            entity.Property(e => e.TicketReservationId).HasColumnName("ticket_reservation_id");
            entity.Property(e => e.ReservationDate).HasColumnName("reservation_date");

            // Configure relationships
            entity.HasOne(tr => tr.User)
                    .WithMany(u => u.TicketReservations)
                    .HasForeignKey(tr => tr.UserId);

            entity.HasOne(tr => tr.Event)
                    .WithMany() 
                    .HasForeignKey(tr => tr.EventId);
        });
        
        OnModelCreatingPartial(modelBuilder);

    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}