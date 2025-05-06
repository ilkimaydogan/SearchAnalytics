using System.ComponentModel.DataAnnotations;

namespace SearchAnalytics.Core.Models;
public class SearchResult
{
    [Key]
    public int Id { get; set; }
    public int SearchId { get; set; }
    public int Position { get; set; }     
    public string Url { get; set; }       
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public virtual Search Search { get; set; } = null!;

}