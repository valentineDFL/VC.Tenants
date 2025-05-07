namespace VC.Tenants.Api.Models.Request.Update;

public record UpdateWorkScheduleDto(IReadOnlyCollection<UpdateDayScheduleDto> WeekSchedule);