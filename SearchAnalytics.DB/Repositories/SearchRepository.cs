using Microsoft.EntityFrameworkCore;
using SearchAnalytics.Core.Interfaces;
using SearchAnalytics.Core.Models;

namespace SearchAnalytics.DB.Repositories;

public class SearchRepository : ISearchRepository
{
    private readonly SearchAnalyticsDbContext _dbContext;

    public SearchRepository(SearchAnalyticsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<int> AddAsync(Search search)
    {
        _dbContext.Searches.Add(search);
        await _dbContext.SaveChangesAsync();
        return search.Id;
    }

    public async Task<List<Search>> GetHistoryAsync()
    {
        return await _dbContext.Searches
            .Include(s => s.SearchEngine)
            .OrderByDescending(s => s.Date)
            .ToListAsync();
    }
}