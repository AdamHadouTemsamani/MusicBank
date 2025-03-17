using System;
using System.Collections.Generic;

//TO-DO: dotnet add package Bogus and MongoDB
using Bogus;
using MongoDB.Bson;

public static class FakeDataGenerator
{
    public static List<UserDTO> GenerateUsers(int count, IDatabaseStrategy dbStrategy)
    {
        var userFaker = new Faker<UserDTO>()
            .RuleFor(u => u.Id, f => dbStrategy.GenerateId())  // Uses dynamic ID generation
            .RuleFor(u => u.Name, f => f.Name.FullName())
            .RuleFor(u => u.Email, f => f.Internet.Email())
            .RuleFor(u => u.PhoneNumber, f => f.Phone.PhoneNumber());

        return userFaker.Generate(count);
    }

    public static List<TicketDTO> GenerateTickets(int count, IDatabaseStrategy dbStrategy)
    {
        var ticketFaker = new Faker<TicketDTO>()
            .RuleFor(t => t.Id, f => dbStrategy.GenerateId())
            .RuleFor(t => t.EventName, f => f.Company.CatchPhrase())
            .RuleFor(t => t.Venue, f => f.Address.City())
            .RuleFor(t => t.EventDate, f => f.Date.Future());

        return ticketFaker.Generate(count);
    }

    public static List<TicketReservationDTO> GenerateReservations(int count, List<UserDTO> users, List<TicketDTO> tickets, IDatabaseStrategy dbStrategy)
    {
        var reservationFaker = new Faker<TicketReservationDTO>()
            .RuleFor(r => r.Id, f => dbStrategy.GenerateId())
            .RuleFor(r => r.UserId, f => users[f.Random.Int(0, users.Count - 1)].Id)
            .RuleFor(r => r.TicketId, f => tickets[f.Random.Int(0, tickets.Count - 1)].Id)
            .RuleFor(r => r.ReservationDate, f => f.Date.Recent());

        return reservationFaker.Generate(count);
    }
}

/*
# HOW TO GENERATE DATA:

var sqlStrategy = new RelationalDatabaseStrategy();

var users = FakeDataGenerator.GenerateUsers(1000, sqlStrategy);
var tickets = FakeDataGenerator.GenerateTickets(500, sqlStrategy);
var reservations = FakeDataGenerator.GenerateReservations(2000, users, tickets, sqlStrategy);

var noSqlStrategy = new NoSqlDatabaseStrategy();

var users = FakeDataGenerator.GenerateUsers(1000, noSqlStrategy);
var tickets = FakeDataGenerator.GenerateTickets(500, noSqlStrategy);
var reservations = FakeDataGenerator.GenerateReservations(2000, users, tickets, noSqlStrategy);
*/

