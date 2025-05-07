namespace VC.Tenants.Api.Models.Request.Create;

public record CreateDayScheduleDto(DayOfWeek Day, DateTime StartWork, DateTime EndWork);