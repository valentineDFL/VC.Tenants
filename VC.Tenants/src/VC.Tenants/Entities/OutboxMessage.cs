namespace VC.Tenants.Entities;

public sealed class OutboxMessage
{
    private OutboxMessage(Guid id, string content, string type, DateTime occuredOnUtc, DateTime? processedOnUtc = null, string? error = null)
    {
        Id = id;
        Content = content;
        Type = type;
        OccuredOnUtc = occuredOnUtc;
        ProcessedOnUtc = processedOnUtc;
        Error = error;
    }

    public Guid Id { get; private set; }

    /// <summary>
    /// Json
    /// </summary>
    public string Content { get; private set; }

    /// <summary>
    /// Json Type
    /// </summary>
    public string Type { get; private set; }

    public DateTime OccuredOnUtc { get; private set; }

    public DateTime? ProcessedOnUtc { get; private set; }

    public string? Error { get; private set; }

    public static OutboxMessage Create(Guid id, string content, string type, DateTime occuredOnUtc)
    {
        return new OutboxMessage(id, content, type, occuredOnUtc);
    }

    public void Update(DateTime? processedOnUtc = null, string? error = null)
    {
        if(ProcessedOnUtc is null)
            ProcessedOnUtc = processedOnUtc;

        if(Error is null)
            Error = error;
    }
}