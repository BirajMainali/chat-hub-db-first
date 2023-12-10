namespace ChatHub.Aggregate;

public class FullAuditLog<T>
{
    public T Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public Status Status { get; set; }
    public string Remarks { get; set; }
}

public enum Status
{
    Active,
    Pending,
    Deleted,
    Deactivated
}