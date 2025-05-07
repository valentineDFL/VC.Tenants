namespace VC.Tenants.Application.Models.Update;

public record UpdateWorkScheduleDto(IReadOnlyList<UpdateScheduleDayDto> WeekSchedule);