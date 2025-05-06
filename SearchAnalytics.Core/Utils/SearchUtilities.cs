using System.Text.RegularExpressions;

namespace SearchAnalytics.Core.Utils;

public static class SearchUtilities
{
    
    public static string BuildSearchUrl(string keyword, int topNResults)
    {
        string encodedKeyword = Uri.EscapeDataString(keyword).Replace("%20", "+");
        
        // Default to Google
        return $"https://www.google.com/search?num={topNResults}&q={encodedKeyword}";
    }
    
    public static string StripUrlToDomain(string url)
    {
        var domain = new Uri(url).Host;
        
        return domain; 
    }
    
    // Helper method for google search routing
    public static string ExtractClickHereUrl(string html)
    {
        // Use regex to find the specific "click here" link in the given pattern
        var match = Regex.Match(html, @"If you're having trouble accessing Google Search, please&nbsp;<a href=""([^""]+)"">click here</a>");
        if (match.Success && match.Groups.Count > 1)
        {
            string href = match.Groups[1].Value;
            
            href = href.Replace("&amp;", "&");
            
            if (href.StartsWith("/"))
            {
                href = "https://www.google.co.uk" + href;
            }
            
            return href;
        }
        
        return string.Empty;
    }

}