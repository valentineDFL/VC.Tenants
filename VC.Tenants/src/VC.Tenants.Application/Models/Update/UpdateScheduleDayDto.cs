namespace VC.Tenants.Application.Models.Update;

public record UpdateScheduleDayDto(Guid Id, DayOfWeek Day, DateTime StartWork, DateTime EndWork);