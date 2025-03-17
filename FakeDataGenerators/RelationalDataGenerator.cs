using System;
using System.Collections.Generic;

//TO-DO: dotnet add package Bogus
using Bogus;

public static class FakeDataGenerator
{
    public static List<UserDTO> GenerateUsers(int count)
    {
        var userFaker = new Faker<UserDTO>()
            .RuleFor(u => u.Id, f => f.IndexFaker + 1)
            .RuleFor(u => u.Name, f => f.Name.FullName())
            .RuleFor(u => u.Email, f => f.Internet.Email())
            .RuleFor(u => u.PhoneNumber, f => f.Phone.PhoneNumber());

        return userFaker.Generate(count);
    }

    public static List<TicketDTO> GenerateTickets(int count)
    {
        var ticketFaker = new Faker<TicketDTO>()
            .RuleFor(t => t.Id, f => f.IndexFaker + 1)
            .RuleFor(t => t.EventName, f => f.Company.CatchPhrase())
            .RuleFor(t => t.Venue, f => f.Address.City())
            .RuleFor(t => t.EventDate, f => f.Date.Future());

        return ticketFaker.Generate(count);
    }

    public static List<TicketReservationDTO> GenerateReservations(int count, List<UserDTO> users, List<TicketDTO> tickets)
    {
        var reservationFaker = new Faker<TicketReservationDTO>()
            .RuleFor(r => r.Id, f => f.IndexFaker + 1)
            .RuleFor(r => r.User, f => f.PickRandom(users))
            .RuleFor(r => r.Ticket, f => f.PickRandom(tickets))
            .RuleFor(r => r.ReservationDate, f => f.Date.Recent());

        return reservationFaker.Generate(count);
    }
}

//HOW TO GENERATE DATA:
//var users = FakeDataGenerator.GenerateUsers(1000);
//var tickets = FakeDataGenerator.GenerateTickets(500);
//var reservations = FakeDataGenerator.GenerateReservations(2000, users, tickets);




