using WakeCap.Models.DTOs;

namespace WakeCap.Models.Results;

public record AssignmentImportResult
{
    public int TotalRowsCount{ get; set; }
    public int ValidRowsCount{ get; set; } 
    public int ErrorRowsCount{ get; set; }
    public List<BulkUploadDto> ValidRows { get; set; } = [];
    public List<RowError> Errors { get; set; } = [];
}
