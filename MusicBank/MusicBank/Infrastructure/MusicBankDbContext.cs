using MusicBank.Models;
using Microsoft.EntityFrameworkCore;

namespace MusicBank.Data;

public interface IMusicBankDbContext
{
    public DbSet<UserDTO> Users { get; set; }
    public DbSet<EventDTO> Events { get; set; }
    public DbSet<TicketReservationDTO> TicketReservations { get; set; }
}

public partial class MusicBankDbContext : DbContext, IMusicBankDbContext
{
    public MusicBankDbContext(DbContextOptions<MusicBankDbContext> options) : base(options) { }
    
    public virtual DbSet<UserDTO> Users { get; set; }
    public virtual DbSet<EventDTO> Events { get; set; }
    public virtual DbSet<TicketReservationDTO> TicketReservations { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //UserDTO mapping
        modelBuilder.Entity<UserDTO>(entity =>
        {
            entity.ToTable("user");
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Id).HasColumnName("user_id");
            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.PhoneNumber).HasColumnName("email");
            entity.Property(e => e.Email).HasColumnName("phone_number");
            entity.HasMany(e => e.TicketReservations)
                .WithOne(tr => tr.User)
                .HasForeignKey(tr => tr.User);

        });
        
        //EventDTO mapping
        modelBuilder.Entity<EventDTO>(entity =>
        {
            entity.ToTable("event");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id).HasColumnName("event_id");
            entity.Property(e => e.EventName).HasColumnName("event_name");
            entity.Property(e => e.EventVenue).HasColumnName("event_venue");
            entity.Property(e => e.EventDate).HasColumnName("event_date");
            
        });
        
        //TicketReservationDTO mapping
        modelBuilder.Entity<TicketReservationDTO>(entity =>
        {
            entity.ToTable("ticket_reservation");
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Id)
                .HasColumnName("reservation_id");

            entity.Property(e => e.ReservationDate)
                .HasColumnName("reservation_date");
            
            entity.HasOne(e => e.User)
                .WithMany(u => u.TicketReservations)
                .HasForeignKey("user_id");
            
            entity.HasOne(e => e.Event)
                .WithMany() 
                .HasForeignKey("event_id");
        });
        
        OnModelCreatingPartial(modelBuilder);

    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}