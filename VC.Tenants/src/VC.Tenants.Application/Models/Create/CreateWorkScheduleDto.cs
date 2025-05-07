namespace VC.Tenants.Application.Models.Create;

public record CreateWorkScheduleDto(IReadOnlyList<CreateDayScheduleDto> WeekSchedule);