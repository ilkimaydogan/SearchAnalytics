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
        else
        {
            List<SearchResult> searchResults = ExtractSearchResults(htmlContent, newSearch.Id, pattern:""); 
            List<SearchResult> matches = FindMatches(searchResults, newSearch.TargetUrl);

            foreach (SearchResult match in matches)
            {
                Console.WriteLine(match.Url);
            }
            // Save all matches to db
            _searchResultRepository.AddRangeAsync(matches);
        }
        
        
    }
    
    // Function for getting HTML content of a Google Search
    private async Task<string> GetGoogleHtmlContent(string searchUrl)
    {
        // Get initial search results
        string htmlContent = await GetSearchHtmlAsync(searchUrl);
        
        // File.WriteAllText("webpagefirst.html", htmlContent); // delete

        // Extract the "click here" URL and follow it
        string followUpUrl = SearchUtilities.ExtractClickHereUrl(htmlContent);

        int maxTryOut = 0;
        
        while (!string.IsNullOrEmpty(followUpUrl) && maxTryOut < 16)
        {
            htmlContent = await GetSearchHtmlAsync(followUpUrl);
            followUpUrl = SearchUtilities.ExtractClickHereUrl(htmlContent);
            Console.WriteLine(followUpUrl + "    max try out is " + maxTryOut);
            maxTryOut++;
        }

        if (maxTryOut > 16)
        {
            Console.WriteLine("YOU HAVE REACHED MAX TRY");
            return "";
        }
    
        
        return htmlContent;
    }
    
    private static async Task<string> GetSearchHtmlAsync(string url)
    {
        // Create a custom HttpClient with a proper User-Agent to avoid being blocked
        using (HttpClient client = new HttpClient())
        {
            // Set a user agent that looks like a regular browser
            client.DefaultRequestHeaders.Add("User-Agent",
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/98.0.4758.102 Safari/537.36");

            // Make the request and get the HTML response
            HttpResponseMessage response = await client.GetAsync(url);

            // Check if the request was successful
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }
            else
            {
                throw new Exception($"HTTP request failed with status code: {response.StatusCode}");
            }
        }
    }
    
    // Function to find url from the search and list
    static List<SearchResult> ExtractSearchResults(string htmlContent, int searchId, string pattern)
    {
        List<SearchResult> searchResults = new List<SearchResult>();
        
        string realPattern = @"<a\s+jsname=""UWckNb""\s+class=""zReHs""\s+href=""(?<hrefContent>[^""]+)""\s+[^>]*>.*?<h3\s+class=""LC20lb[^""]*""[^>]*>(.*?)</h3>";
        Regex linkRegex = new Regex(realPattern, RegexOptions.Singleline);
        
        // Find all matches
        MatchCollection matches = linkRegex.Matches(htmlContent);
        
        // for position info
        int position = 1;
        
        foreach (Match match in matches)
        {
            if (match.Groups["hrefContent"].Success)
            {
                string url = match.Groups["hrefContent"].Value;
                Console.WriteLine("Url is: " + url);

                // Create new object and add the resultsto list
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
    
    
    
    // Get search history
    public async Task<List<Search>> GetSearchHistoryAsync()
    {
        return await _searchRepository.GetHistoryAsync();
    }
    
}