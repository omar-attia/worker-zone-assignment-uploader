namespace WakeCap.Models.DTOs;

public record BulkUploadDto(
    string WorkerCode,
    string ZoneCode,
    string AssignmentDate);
