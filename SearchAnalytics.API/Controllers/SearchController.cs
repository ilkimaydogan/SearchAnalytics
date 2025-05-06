// SearchAnalytics.API/Controllers/SearchController.cs

using Microsoft.AspNetCore.Mvc;
using SearchAnalytics.Core.DTOs;
using SearchAnalytics.Core.Interfaces;
using SearchAnalytics.Core.Models;

namespace SearchAnalytics.Controllers;

[ApiController]
[Route("[controller]")]
public class SearchController : ControllerBase
{
    private readonly ISearchRepository _searchRepository;
    private readonly ISearchResultRepository _searchResultRepository;
    private readonly ISearchQueryService _searchQueryService;

    public SearchController(
        ISearchRepository searchRepository, 
        ISearchResultRepository searchResultRepository,
        ISearchQueryService searchQueryService)
    {
        _searchRepository = searchRepository;
        _searchResultRepository = searchResultRepository;
        _searchQueryService = searchQueryService;
    }

    [HttpPost("/search")]
    public async Task<IActionResult> Search([FromBody] SearchRequestDTO request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var search = new Search
            {
                SearchEngineId = 1, // Google's ID
                TargetUrl = request.TargetUrl,
                Keyword = request.Keyword,
                TopNResult = request.TopNResult,
                Date = DateTime.UtcNow
            };
            int searchId = await _searchRepository.AddAsync(search);
            await _searchQueryService.Search(search);
            var results = await _searchResultRepository.GetBySearchIdAsync(searchId);

            
            return Ok(new { 
                message = "Search added successfully", 
                 results = results.Select(x => new SearchResultDto(){
                    Id = x.Id, 
                    Url = x.Url, 
                    CreatedAt = x.CreatedAt,
                    Position = x.Position, 
                    SearchId = x.SearchId
                    
                } ) 
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Search error: {ex.Message}");
            return Problem(
                detail: ex.Message,
                title: "An unexpected error occurred during search", 
                statusCode: StatusCodes.Status500InternalServerError);
        }
    }
    
    [HttpGet("/search/results/{searchId}")]
    public async Task<IActionResult> GetSearchResults(int searchId)
    {
        try
        {
            var results = await _searchResultRepository.GetBySearchIdAsync(searchId);
            return Ok(results);
        }
        catch (Exception ex)
        {
            return Problem(
                detail: ex.Message,
                title: "Error retrieving search results", 
                statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    [HttpGet("/search/history")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetSearchHistory()
    {
        try
        {
            var history = await _searchQueryService.GetSearchHistoryAsync();
            return Ok(history);
        }
        catch (Exception ex)
        {
            return Problem(
                detail: ex.Message,
                title: "Error retrieving search history", 
                statusCode: StatusCodes.Status500InternalServerError);
        }
    }
}
