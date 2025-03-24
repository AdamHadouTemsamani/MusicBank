using System.Collections.Generic;
using Bogus;
using MusicBank.Domain;

public static class SqlFakeDataGenerator
{
    [assembly: CollectionBehavior(DisableTestParallelization = true)]
    public static List<User> GenerateUsers(int count)
    {
        var userFaker = new Faker<User>()
            .RuleFor(u => u.UserId, _ => 0) // EF Core/PostgreSQL will auto-generate
            .RuleFor(u => u.Name, f => f.Name.FullName())
            .RuleFor(u => u.Email, f => f.Internet.Email())
            .RuleFor(u => u.PhoneNumber, f => f.Phone.PhoneNumber());

        return userFaker.Generate(count);
    }

    public static List<Event> GenerateEvents(int count)
    {
        var eventFaker = new Faker<Event>()
            .RuleFor(e => e.EventId, _ => 0)
            .RuleFor(e => e.EventName, f => f.Company.CompanyName())
            .RuleFor(e => e.EventVenue, f => f.Address.City())
            .RuleFor(e => e.EventDate, f => f.Date.Future().ToUniversalTime());

        return eventFaker.Generate(count);
    }

    public static List<TicketReservation> GenerateReservations(int count, List<User> users, List<Event> events)
    {
        var reservationFaker = new Faker<TicketReservation>()
            .RuleFor(r => r.TicketReservationId, _ => 0)
            .RuleFor(r => r.UserId, f => f.PickRandom(users).UserId)
            .RuleFor(r => r.EventId, f => f.PickRandom(events).EventId)
            .RuleFor(r => r.ReservationDate, f => f.Date.Recent().ToUniversalTime());

        return reservationFaker.Generate(count);
    }
}
