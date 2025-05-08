using CsvHelper;
using System.Globalization;
using WakeCap.Models.DTOs;
using WakeCap.Models.Results;
using WakeCap.DAL.Entities;
using WakeCap.DAL.Repositories.Interfaces;
using WakeCap.Models.Enums;
using WakeCap.Service.Interfaces;


namespace WakeCap.Service;

public class WorkerZoneAssignmentImportService(IWorkerZoneAssignmentRepository workerZoneAssignmentRepository, IWorkerRepository workerRepository, IZoneRepository zoneRepository, IUploadLogRepository uploadLogRepository) : IWorkerZoneAssignmentImportService
{
    private const int MaxRows = 50000;
    private const int MaxCodeLength = 10;

    public async Task<AssignmentImportResult> ImportWorkerZoneAssignmentsAsync(Stream fileStream, string fileName)
    {
        var rows = ReadCsvRows(fileStream);
        if (rows.Count > MaxRows)
        {
            throw new InvalidOperationException($"File exceeds maximum row limit of {MaxRows}");
        }

        var (uniqueWorkerCodes, uniqueZoneCodes, assignmentKeys, rowOccurrences, workerDateZones) = CollectDataFromRows(rows);

        var existingWorkerCodesAndIds = await workerRepository.GetWorkerCodesAndIdsAsync(uniqueWorkerCodes);
        var existingZoneCodesAndIds = await zoneRepository.GetZoneCodesAndIdsAsync(uniqueZoneCodes);
        var existingAssignments = await workerZoneAssignmentRepository.GetExistingAssignmentsAsync(assignmentKeys);

        var result = ValidateRows(rows, existingWorkerCodesAndIds, existingZoneCodesAndIds, existingAssignments, rowOccurrences, workerDateZones);

        if (!result.Errors.Any())
        {
            var assignments = result.ValidRows.Select(r => new WorkerZoneAssignment
            {
                WorkerId = existingWorkerCodesAndIds[r.WorkerCode],
                ZoneId = existingZoneCodesAndIds[r.ZoneCode],
                EffectiveDate = DateTime.Parse(r.AssignmentDate).Date
            }).ToList();

            await workerZoneAssignmentRepository.BulkInsertAssignmentsAsync(assignments);
        }

        await uploadLogRepository.SaveLogAsync(new UploadLog
        {
            FileName = fileName,
            TotalRows = rows.Count,
            ValidRows = result.ValidRows.Count,
            ErrorRows = result.Errors.Count,
            Status = result.Errors.Any() ? UploadStatus.rejected.ToString() : UploadStatus.saved.ToString()
        });

        return result;
    }

    private List<BulkUploadDto> ReadCsvRows(Stream fileStream)
    {
        using var reader = new StreamReader(fileStream);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        csv.Context.TypeConverterOptionsCache.GetOptions<string>().NullValues.Add(string.Empty);
        csv.Read();
        csv.ReadHeader();
        return csv.GetRecords<BulkUploadDto>().ToList();
    }

    private static (HashSet<string> uniqueWorkerCodes,
        HashSet<string> uniqueZoneCodes, 
        HashSet<(string, DateTime)> assignmentKeys, 
        Dictionary<(string, string, string), int> rowOccurrences, 
        Dictionary<(string, DateTime), 
        HashSet<string>> workerDateZones)
        CollectDataFromRows(List<BulkUploadDto> rows)
    {
        var uniqueWorkerCodes = new HashSet<string>();
        var uniqueZoneCodes = new HashSet<string>();
        var assignmentKeys = new HashSet<(string, DateTime)>();
        var rowOccurrences = new Dictionary<(string, string, string), int>();
        var workerDateZones = new Dictionary<(string, DateTime), HashSet<string>>();

        foreach (var row in rows)
        {
            var workerCode = row.WorkerCode?.Trim();
            var zoneCode = row.ZoneCode?.Trim();
            var assignmentDate = row.AssignmentDate?.Trim();

            if (!string.IsNullOrWhiteSpace(workerCode)) uniqueWorkerCodes.Add(workerCode);
            if (!string.IsNullOrWhiteSpace(zoneCode)) uniqueZoneCodes.Add(zoneCode);

            if (!string.IsNullOrWhiteSpace(workerCode) && !string.IsNullOrWhiteSpace(assignmentDate))
            {
                if (DateTime.TryParse(assignmentDate, out DateTime parsedDate))
                {
                    var key = (workerCode, parsedDate);
                    assignmentKeys.Add(key);

                    if (!workerDateZones.ContainsKey(key))
                    {
                        workerDateZones[key] = new HashSet<string>();
                    }

                    workerDateZones[key].Add(zoneCode);
                }
            }

            var rowKey = (workerCode, zoneCode, assignmentDate);
            rowOccurrences[rowKey] = rowOccurrences.GetValueOrDefault(rowKey) + 1;
        }
        return (uniqueWorkerCodes, uniqueZoneCodes, assignmentKeys, rowOccurrences, workerDateZones);
    }

    private static AssignmentImportResult ValidateRows(
        List<BulkUploadDto> rows,
        Dictionary<string, int> existingWorkerCodesAndIds,
        Dictionary<string, int> existingZoneCodesAndIds,
        Dictionary<(string, DateTime), string> existingAssignments,
        Dictionary<(string, string, string), int> rowOccurrences,
        Dictionary<(string, DateTime), HashSet<string>> workerDateZones)
    {
        var result = new AssignmentImportResult();
        var multipleZoneKeys = workerDateZones
            .Where(kv => kv.Value.Count > 1)
            .Select(kv => kv.Key)
            .ToHashSet();

        foreach (var row in rows)
        {
            var rowData = new Dictionary<string, string>
            {
                ["worker_code"] = row.WorkerCode,
                ["zone_code"] = row.ZoneCode,
                ["assignment_date"] = row.AssignmentDate
            };
            var rowErrors = new RowError { Data = rowData, Error = new Dictionary<string, string>() };

            // Validate worker code
            ValidateWorkerCode(existingWorkerCodesAndIds, row, rowErrors);

            // Validate zone code
            ValidateZoneCode(existingZoneCodesAndIds, row, rowErrors);

            // Validate assignment date if in valid format or not

            if (!DateTime.TryParse(row.AssignmentDate, out DateTime assignmentDate))
            {
                AddError(rowErrors, "assignment_date", "Invalid date format");
            }

            // Check for exact duplicates in the file
            var rowKey = (row.WorkerCode, row.ZoneCode, row.AssignmentDate);
            if (rowOccurrences.TryGetValue(rowKey, out int count) && count > 1)
                AddError(rowErrors, "rowError", "Duplicate row in file");

            // Check for multiple zone assignments in the file
            if (assignmentDate.Date > DateTime.MinValue.Date)
            {
                var assignmentKey = (row.WorkerCode, assignmentDate);
                if (multipleZoneKeys.Contains(assignmentKey))
                    AddError(rowErrors, "rowError", "Worker assigned to multiple zones on the same date");

                // Check for conflicts with existing assignments
                if (existingAssignments.TryGetValue(assignmentKey, out var existingZoneCode))
                {
                    if (existingZoneCode == row.ZoneCode)
                        AddError(rowErrors, "rowError", "Assignment already exists in worker_zone_assignment table");
                    else
                        AddError(rowErrors, "rowError", "If inserted, it would create a conflicting assignment for the same worker on the same date");
                }
            }

            if (rowErrors.Error.Any())
                result.Errors.Add(rowErrors);
            else
                result.ValidRows.Add(row);
        }
        result.ErrorRowsCount = result.Errors.Count;
        result.ValidRowsCount = result.ValidRows.Count;
        result.TotalRowsCount = rows.Count;
        return result;
    }

    private static void ValidateZoneCode(Dictionary<string, int> existingZoneCodes, BulkUploadDto row, RowError rowErrors)
    {
        if (string.IsNullOrWhiteSpace(row.ZoneCode))
            AddError(rowErrors, "zone_code", "Zone code cannot be null or empty");
        else if (row.ZoneCode.Length > MaxCodeLength)
            AddError(rowErrors, "zone_code", "Zone code exceeds 10 characters");
        else if (!existingZoneCodes.ContainsKey(row.ZoneCode))
            AddError(rowErrors, "zone_code", "Zone does not exist");
    }

    private static void ValidateWorkerCode(Dictionary<string, int> existingWorkerCodes, BulkUploadDto row, RowError rowErrors)
    {
        if (string.IsNullOrWhiteSpace(row.WorkerCode))
            AddError(rowErrors, "worker_code", "Worker code cannot be null or empty");
        else if (row.WorkerCode.Length > MaxCodeLength)
            AddError(rowErrors, "worker_code", "Worker code exceeds 10 characters");
        else if (!existingWorkerCodes.ContainsKey(row.WorkerCode))
            AddError(rowErrors, "worker_code", "Worker does not exist");
    }

    private static void AddError(RowError result, string key, string message)
    {
        if (result.Error.ContainsKey(key))
            result.Error[key] = $"{result.Error[key]}, {message}";
        else
            result.Error[key] = message;
    }
}
