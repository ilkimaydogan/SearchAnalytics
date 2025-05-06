using SearchAnalytics.Core.Models;

namespace SearchAnalytics.Core.Interfaces;

public interface ISearchEngineRepository
{
    Task<SearchEngine?> GetByIdAsync(int id);
    Task<List<SearchEngine>> GetAllAsync();
}