namespace SearchAnalytics.Core.DTOs;

public class SearchEngineDto
{
    public int Id { get; set; }
    public string Name { get; set; } 
    public string Pattern { get; set; }
    public bool IsActive { get; set; } = true;
}
