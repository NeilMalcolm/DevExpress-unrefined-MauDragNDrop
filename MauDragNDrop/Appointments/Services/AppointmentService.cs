using MauDragNDrop.Appointments.Models;

namespace MauDragNDrop.Appointments.Services;

public class AppointmentService : IAppointmentService
{
    private DateTime GetBaseDate() => DateTime.UtcNow.Date;

    public async Task<List<Appointment>> GetAppointmentsForUser(int userId)
    {
        await Task.Delay(0);

        return new List<Appointment>
        {
            new Appointment
            {
                Id = 5,
                Start = GetBaseDate().AddHours(10).AddMinutes(30),
                End = GetBaseDate().AddHours(11).AddMinutes(30),
                Type = "Dinner"
            },
            new Appointment
            {
                Id = 2,
                Start = GetBaseDate().AddDays(1).AddHours(9),
                End = GetBaseDate().AddDays(1).AddHours(10),
                Type = "Check up",
                Patient = new Patient(1, "Damon Albarn")
            },
            new Appointment
            {
                Id = 3,
                Start = GetBaseDate().AddDays(-1).AddHours(23),
                End = GetBaseDate().AddHours(1),
                Type = "Inoculation",
                Patient = new Patient(1, "Alex Turner")
            },
            new Appointment
            {
                Id = 3,
                Start = GetBaseDate().AddDays(-1).AddHours(16),
                End = GetBaseDate().AddDays(-1).AddHours(17).AddMinutes(30),
                Type = "Check up",
                Patient = new Patient(1, "Rina Sawayama")
            },
            new Appointment
            {
                Id = 4,
                Start = GetBaseDate().AddDays(2).AddHours(17).AddMinutes(0),
                End = GetBaseDate().AddDays(2).AddHours(17).AddMinutes(30),
                Type = "Therapy",
                Patient = new Patient(1, "Joe Keery")
            }
        };
    }
}
