using Microsoft.EntityFrameworkCore;
using MusicBank.Data;
using MusicBank.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<MusicBankDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("defaultConnection")));

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

//Events endpoints

//TicketReservation endpints

app.MapGet("/", () => "Hello World!");

app.Run();