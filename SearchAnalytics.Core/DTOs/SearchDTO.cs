namespace SearchAnalytics.Core.DTOs;

public class SearchDto
{
    public int Id { get; set; }
    public int SearchEngineId { get; set; }
    public string TargetUrl { get; set; } 
    public string Keyword { get; set; }
    public int TopNResult { get; set; } 
    public DateTime Date { get; set; } = DateTime.UtcNow;
}