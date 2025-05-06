using Microsoft.EntityFrameworkCore;
using SearchAnalytics.Core.Interfaces;
using SearchAnalytics.Core.Models;

namespace SearchAnalytics.DB.Repositories;

public class SearchEngineRepository: ISearchEngineRepository
{
    private readonly SearchAnalyticsDbContext _dbContext;

    public SearchEngineRepository(SearchAnalyticsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<SearchEngine?> GetByIdAsync(int id)
    {
        return await _dbContext.SearchEngines.FindAsync(id);
    }

    public async Task<List<SearchEngine>> GetAllAsync()
    {
        return await _dbContext.SearchEngines.ToListAsync();
    }
}