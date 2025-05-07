namespace VC.Tenants.Api.Models.Response;

public record ResponseScheduleDto
    (Guid Id,
     DayOfWeek Day,
     DateTime StartWork,
     DateTime EndWork);