namespace SearchAnalytics.Core.DTOs;

public class SearchResultDto
{
    public int Id { get; set; }
    public int SearchId { get; set; }
    public int Position { get; set; }     
    public string Url { get; set; } 
    public DateTime CreatedAt { get; set; }
}