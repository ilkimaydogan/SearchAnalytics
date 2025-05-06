// SearchAnalytics.DB/Repositories/SearchResultRepository.cs
using Microsoft.EntityFrameworkCore;
using SearchAnalytics.Core.Interfaces;
using SearchAnalytics.Core.Models;

namespace SearchAnalytics.DB.Repositories;

public class SearchResultRepository : ISearchResultRepository
{
    private readonly SearchAnalyticsDbContext _dbContext;

    public SearchResultRepository(SearchAnalyticsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddRangeAsync(List<SearchResult> searchResults)
    {
        _dbContext.SearchResults.AddRange(searchResults);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<List<SearchResult>> GetBySearchIdAsync(int searchId)
    {
        return await _dbContext.SearchResults
            .Where(sr => sr.SearchId == searchId)
            .OrderBy(sr => sr.Position)
            .ToListAsync();
    }
}