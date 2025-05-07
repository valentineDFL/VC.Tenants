namespace VC.Tenants.Api.Models.Request.Update;

/// <summary>
/// Dto принимает дату только в формате UTC
/// </summary>
/// <param name="StartWork">Дата должна быть только в формате UTC</param>
/// <param name="EndWork">Дата должна быть только в формате UTC</param>
public record UpdateDayScheduleDto
    (Guid Id,
     DayOfWeek Day,
     DateTime StartWork,
     DateTime EndWork);