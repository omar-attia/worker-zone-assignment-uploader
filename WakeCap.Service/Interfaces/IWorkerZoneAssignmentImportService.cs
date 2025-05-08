using WakeCap.Models.Results;

namespace WakeCap.Service.Interfaces;

public interface IWorkerZoneAssignmentImportService
{
    Task<AssignmentImportResult> ImportWorkerZoneAssignmentsAsync(Stream fileStream, string fileName);
}
