namespace MauDragNDrop.Appointments.Models;

public class Appointment
{
    public int Id { get; set; }
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public Patient Patient { get; set; }
    public string Type { get; set; }

    public bool IsWithPatient => Patient is not null;
}

public record Patient(int Id, string Name);