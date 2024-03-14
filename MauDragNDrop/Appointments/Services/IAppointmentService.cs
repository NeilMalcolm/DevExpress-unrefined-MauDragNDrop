using MauDragNDrop.Appointments.Models;

namespace MauDragNDrop.Appointments.Services;

public interface IAppointmentService
{
    Task<List<Appointment>> GetAppointmentsForUser(int userId);
}
