namespace WakeCap.Models.DTOs;
public class AssignmentImportResultDto
{
    
    public int TotalRows { get; set; }
    public int ValidRows { get; set; }
    public int InvalidRows => Errors.Count;
    public List<AssignmentImportErrorDto> Errors { get; set; } = [];
}
