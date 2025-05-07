namespace VC.Tenants.Entities;

public class WorkSchedule : ValueObject
{
    private List<DaySchedule> _weekSchedule;

    private WorkSchedule(List<DaySchedule> weekSchedule)
    {
        _weekSchedule = weekSchedule;
    }

    private WorkSchedule() { }

    public IReadOnlyList<DaySchedule> WeekSchedule => _weekSchedule.AsReadOnly();

    public static WorkSchedule Create(List<DaySchedule> weekSchedule)
    {
        if (weekSchedule.Count != Enum.GetValues(typeof(DayOfWeek)).Length)
            throw new ArgumentException("Schedule must have 7 days");

        if (weekSchedule.DistinctBy(d => d.Day).Count() != weekSchedule.Count)
            throw new ArgumentException("Schedule Days must be uniqie");

        return new WorkSchedule(weekSchedule);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        foreach(var item in _weekSchedule.OrderBy(x => x.Day))
            yield return item;
    }
}