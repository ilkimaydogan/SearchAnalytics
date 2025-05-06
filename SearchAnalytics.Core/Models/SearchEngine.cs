using System.ComponentModel.DataAnnotations;

namespace SearchAnalytics.Core.Models;

public class SearchEngine
{

    [Key]
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public string Pattern { get; set; } = string.Empty;
    public virtual ICollection<Search> Searches { get; set; } = null!;
    
}