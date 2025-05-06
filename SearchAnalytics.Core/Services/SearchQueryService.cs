using System.Text.RegularExpressions;
using SearchAnalytics.Core.Interfaces;
using SearchAnalytics.Core.Models;
using SearchAnalytics.Core.Utils;

namespace SearchAnalytics.Core.Services;
public class SearchQueryService : ISearchQueryService
{
    private readonly ISearchRepository _searchRepository;
    private readonly ISearchResultRepository _searchResultRepository;
    private readonly ISearchEngineRepository _searchEngineRepository;

    public SearchQueryService(
        ISearchRepository searchRepository,
        ISearchResultRepository searchResultRepository,
        ISearchEngineRepository searchEngineRepository)
    {
        _searchRepository = searchRepository;
        _searchResultRepository = searchResultRepository;
        _searchEngineRepository = searchEngineRepository;
    }
    
    public async Task Search(Search newSearch)
    {
        // Potential improvement: Get the serach engine object from DB
        
        string searchUrl = SearchUtilities.BuildSearchUrl(newSearch.Keyword, newSearch.TopNResult);
        
        // if search is google get google html
        bool isGoogle = true;
        string htmlContent;
        if (isGoogle)
        {
            htmlContent = await GetGoogleHtmlContent(searchUrl);
        }
        else
        {
            htmlContent = await GetSearchHtmlAsync(searchUrl);
        }

        if (htmlContent == "")
        {
            throw new Exception("No search results found");
        }
        List<SearchResult> searchResults = ExtractSearchResults(htmlContent, newSearch.Id); 
        List<SearchResult> matches = FindMatches(searchResults, newSearch.TargetUrl);
        foreach (SearchResult match in matches)
        {
            Console.WriteLine(match.Url);
        }
        // Save all matches to db
        await _searchResultRepository.AddRangeAsync(matches);
    }
    
    // Function for getting HTML content of a Google Search
    private async Task<string> GetGoogleHtmlContent(string searchUrl)
    {
        string htmlContent = await GetSearchHtmlAsync(searchUrl);
        
        // Extract the "click here" URL and follow it
        string followUpUrl = SearchUtilities.ExtractClickHereUrl(htmlContent);

        int maxTryOut = 23;
        int tryOutCount = 0;
        
        while (!string.IsNullOrEmpty(followUpUrl) && tryOutCount < maxTryOut)
        {
            htmlContent = await GetSearchHtmlAsync(followUpUrl);
            followUpUrl = SearchUtilities.ExtractClickHereUrl(htmlContent);
            Console.WriteLine(followUpUrl + " try out count is " + tryOutCount);
            tryOutCount++;
        }

        if (tryOutCount == maxTryOut)
        {
            throw new Exception("You have reached max try limit");
        }
    
        
        return htmlContent;
    }
    
    private static async Task<string> GetSearchHtmlAsync(string url)
    {
        using (HttpClient client = new HttpClient())
        {
            // Set a user agent that looks like a regular browser
            client.DefaultRequestHeaders.Add("User-Agent",
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/98.0.4758.102 Safari/537.36");

            HttpResponseMessage response = await client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }
            throw new Exception($"HTTP request failed with status code: {response.StatusCode}");
        }
    }
    static List<SearchResult> ExtractSearchResults(string htmlContent, int searchId)
    {
        List<SearchResult> searchResults = new List<SearchResult>();
        
        // Potential improvement: Fetch this from DB alongside query engine object
        string googleRegexPattern = @"<a\s+jsname=""UWckNb""\s+class=""zReHs""\s+href=""(?<hrefContent>[^""]+)""\s+[^>]*>.*?<h3\s+class=""LC20lb[^""]*""[^>]*>(.*?)</h3>";
        Regex linkRegex = new Regex(googleRegexPattern, RegexOptions.Singleline);
        
        MatchCollection matches = linkRegex.Matches(htmlContent);
        
        int position = 1;
        
        foreach (Match match in matches)
        {
            if (match.Groups["hrefContent"].Success)
            {
                string url = match.Groups["hrefContent"].Value;
                Console.WriteLine("Url is: " + url);

                searchResults.Add(new SearchResult 
                { 
                    SearchId = searchId,
                    Position = position,
                    Url = url,
                    CreatedAt = DateTime.UtcNow
                });
                
                position++;
            }
        }
        Console.WriteLine("Exited the function extract search results");
        return searchResults;
    }

    
    // Function to match target url with searchResults
    List<SearchResult> FindMatches(List<SearchResult> searchResults, string targetUrl)
    {
        Console.WriteLine($"trying to find matches for the results for {targetUrl}");
        
        List<SearchResult> matchedResults = new List<SearchResult>();
        string targetDomain = SearchUtilities.StripUrlToDomain(targetUrl);
        
        foreach (SearchResult searchResult in searchResults)
        {
            if (string.Equals(SearchUtilities.StripUrlToDomain(searchResult.Url), targetDomain, StringComparison.OrdinalIgnoreCase))
            {
                matchedResults.Add(searchResult);
            }
        }
        
        return matchedResults;
    }
    public async Task<List<Search>> GetSearchHistoryAsync()
    {
        return await _searchRepository.GetHistoryAsync();
    }
    
}