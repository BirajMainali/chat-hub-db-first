namespace ChatHub.Migrations.ValueObject;

public class Migration
{
    public string Name { get; set; }
    public MigrationType Type { get; set; }
    public DateTime? AppliedDate { get; set; }
}

public enum MigrationType
{
    Pending,
    Applied
}