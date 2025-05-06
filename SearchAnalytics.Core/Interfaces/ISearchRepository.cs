// SearchAnalytics.DB/Interfaces/ISearchRepository.cs
using SearchAnalytics.Core.Models;

namespace SearchAnalytics.Core.Interfaces;

public interface ISearchRepository
{
    Task<int> AddAsync(Search search);
    Task<List<Search>> GetHistoryAsync();

    Task<Search?> GetLatestSearchAsync();

}