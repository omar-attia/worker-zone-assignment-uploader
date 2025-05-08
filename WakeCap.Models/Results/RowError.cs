namespace WakeCap.Models.Results;

public class RowError
{
    public Dictionary<string, string> Data { get; set; } = [];
    public Dictionary<string, string> Error { get; set; } = [];
}
