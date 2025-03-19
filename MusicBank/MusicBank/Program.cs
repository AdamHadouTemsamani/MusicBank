using Microsoft.EntityFrameworkCore;
using MusicBank.Data;
using MusicBank.Features.Events.GetEvent;
using MusicBank.Features.Users.GetUser;
using MusicBank.Features.Users.PostUser;
using MusicBank.Features.Events.PostEvent;
using MusicBank.Features.TicketReservations.GetTicketReservations;
using MusicBank.Features.TicketReservations.PostTicketReservation;
using MusicBank.Features.TicketReservations.DeleteTicketReservation;
using MusicBank.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<MusicBankDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("MusicBankDb")));

builder.Services.AddScoped<IMusicBankDbContext>(provider =>
    provider.GetRequiredService<MusicBankDbContext>()
);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<MusicBankDbContext>();
    db.Database.Migrate();
}

//Users endpoints
app.MapGetUserEndpoints();
app.MapPostUserEndpoints();

//Events endpoints
app.MapGetEventEndpoints();
app.MapPostEventEndpoints();

//TicketReservation endpints
app.MapGetTicketReservationEndpoints();
app.MapPostTicketReservationEndpoints();
app.MapDeleteTicketReservationEndpoints();


app.Run();

public partial class Program{ }