namespace WakeCap.DAL.Entities;

public class Zone
{
    public int Id { get; set; }
    public required string Code { get; set; }
    public required string Name { get; set; }
    public ICollection<WorkerZoneAssignment> Assignments { get; set; } = [];
}