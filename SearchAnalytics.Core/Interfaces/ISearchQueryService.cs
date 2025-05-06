using SearchAnalytics.Core.Models;

namespace SearchAnalytics.Core.Interfaces;

public interface ISearchQueryService
{
    Task Search(Search newSearch);
    Task<List<Search>> GetSearchHistoryAsync();
}