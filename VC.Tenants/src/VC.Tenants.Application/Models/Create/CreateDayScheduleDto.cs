namespace VC.Tenants.Application.Models.Create;

public record CreateDayScheduleDto(DayOfWeek Day, DateTime StartWork, DateTime EndWork);