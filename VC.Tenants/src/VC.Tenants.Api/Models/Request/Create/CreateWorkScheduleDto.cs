namespace VC.Tenants.Api.Models.Request.Create;

public record CreateWorkScheduleDto(IReadOnlyList<CreateDayScheduleDto> WeekSchedule);