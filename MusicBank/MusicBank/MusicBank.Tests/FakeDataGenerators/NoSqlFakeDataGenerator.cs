using System.Collections.Generic;
using Bogus;
using MusicBank.Domain;
using MongoDB.Bson;

public static class NoSqlFakeDataGenerator
{
    public static List<User> GenerateUsers(int count)
    {
        var userFaker = new Faker<User>()
            // Generate a new MongoDB ObjectId as a string
            .RuleFor(u => u.Id, _ => ObjectId.GenerateNewId().ToString())
            .RuleFor(u => u.Name, f => f.Name.FullName())
            .RuleFor(u => u.Email, f => f.Internet.Email())
            .RuleFor(u => u.PhoneNumber, f => f.Phone.PhoneNumber());

        return userFaker.Generate(count);
    }

    public static List<Event> GenerateEvents(int count)
    {
        var eventFaker = new Faker<Event>()
            .RuleFor(e => e.Id, _ => ObjectId.GenerateNewId().ToString())
            .RuleFor(e => e.EventName, f => f.Company.CompanyName())
            .RuleFor(e => e.EventVenue, f => f.Address.City())
            .RuleFor(e => e.EventDate, f => f.Date.Future().ToUniversalTime());

        return eventFaker.Generate(count);
    }

    public static List<TicketReservation> GenerateReservations(int count, List<User> users, List<Event> events)
    {
        var reservationFaker = new Faker<TicketReservation>()
            .RuleFor(r => r.Id, _ => ObjectId.GenerateNewId().ToString())
            .RuleFor(r => r.UserId, f => f.PickRandom(users).Id)
            .RuleFor(r => r.EventId, f => f.PickRandom(events).Id)
            .RuleFor(r => r.ReservationDate, f => f.Date.Recent().ToUniversalTime());

        return reservationFaker.Generate(count);
    }
}