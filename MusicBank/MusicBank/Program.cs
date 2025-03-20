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

// **Configure MongoDB settings**
builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection("MongoDB"));
builder.Services.AddSingleton<MongoDbContext>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// **Drop and recreate the MongoDB database on startup**
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<MongoDbContext>();
    dbContext.RecreateDatabase();  // Deletes and recreates the database
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Map endpoints
app.MapGetUserEndpoints();
app.MapPostUserEndpoints();
app.MapGetEventEndpoints();
app.MapPostEventEndpoints();
app.MapGetTicketReservationEndpoints();
app.MapPostTicketReservationEndpoints();
app.MapDeleteTicketReservationEndpoints();

app.Run();

public partial class Program { }
