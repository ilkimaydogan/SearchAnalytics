namespace SearchAnalytics.Core.DTOs;

public class SearchRequestDTO
{
    public int SearchEngineId { get; set; } 
    public string TargetUrl { get; set; } 
    public string Keyword { get; set; } 
    public int TopNResult { get; set; } 
}