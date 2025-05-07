
namespace VC.Tenants.Entities;

public class DaySchedule : ValueObject
{
    private DaySchedule(Guid id, Guid tenantId, DayOfWeek day, DateTime startWork, DateTime endWork)
    {
        Id = id;
        TenantId = tenantId;
        Day = day;
        StartWork = startWork;
        EndWork = endWork;
    }

    private DaySchedule() { }

    public Guid Id { get; private set; }

    public Guid TenantId { get; private set; }

    public DayOfWeek Day { get; private set; }

    public DateTime StartWork { get; private set; }

    public DateTime EndWork { get; private set; }

    public static DaySchedule Create(Guid id, Guid tenantId, DayOfWeek day, DateTime startWork, DateTime endWork)
    {
        if (startWork > endWork)
            throw new ArgumentException("StartWork time cannot be highest than EndWork time");

        if(startWork == endWork)
            throw new ArgumentException("StartWork time cannot be equals EndWork time");

        return new DaySchedule(id, tenantId, day, startWork, endWork);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Id;
        yield return TenantId;
        yield return Day;
        yield return StartWork;
        yield return EndWork;
    }
}