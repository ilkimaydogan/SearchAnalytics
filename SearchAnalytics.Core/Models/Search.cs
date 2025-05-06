using System.ComponentModel.DataAnnotations;

namespace SearchAnalytics.Core.Models;

public class Search
{
    [Key]
    public int Id { get; set; }
    public int SearchEngineId { get; set; }
    public string? TargetUrl { get; set; }
    public string? Keyword { get; set; }
    public int TopNResult { get; set; } 
    public DateTime? Date { get; set; } = DateTime.UtcNow;
    public virtual SearchEngine SearchEngine { get; set; } = null!;
    public virtual ICollection<SearchResult> SearchResults { get; set; } = null!;
}