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
            
            try
            {
                await _searchQueryService.Search(search);
            }
            catch (Exception searchEx)
            {
                Console.WriteLine($"Search error: {searchEx.Message}");
            }
            
            var results = await _searchResultRepository.GetBySearchIdAsync(searchId);
            foreach(SearchResult r in results)
            {
                Console.WriteLine(r.Url);
            }
            return Ok(new { 
                message = "Search added successfully", 
                searchId = searchId,
                results = results 
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
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
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpGet("history")]
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
