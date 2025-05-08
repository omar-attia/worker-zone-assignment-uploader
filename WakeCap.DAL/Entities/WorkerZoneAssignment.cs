namespace WakeCap.DAL.Entities;

public class WorkerZoneAssignment
{
    public int Id { get; set; }
    public required int WorkerId { get; set; }
    public required int ZoneId { get; set; }
    public required DateTime EffectiveDate { get; set; }
    public Worker Worker { get; set; }
    public Zone Zone { get; set; }
}
