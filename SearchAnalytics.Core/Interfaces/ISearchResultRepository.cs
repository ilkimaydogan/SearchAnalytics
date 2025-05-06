using SearchAnalytics.Core.Models;

namespace SearchAnalytics.Core.Interfaces;

public interface ISearchResultRepository
{
    Task AddRangeAsync(List<SearchResult> searchResults);
    Task<List<SearchResult>> GetBySearchIdAsync(int searchId);
}