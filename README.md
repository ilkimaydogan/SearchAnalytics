# SearchAnalytics

SearchAnalytics is a web application designed to help track a website's position in Google search results for specific keywords. The application automatically searches Google for the specified keyword and identifies where your target website appears in the results.

## Features

- Track where a target URL appears in Google search results for specific keywords
- View historical search data to track SEO performance over time
- Results are limited to the top N results (configurable, default 100)
- Clean separation of concerns with repository pattern and service-based architecture

## Prerequisites

- .NET 8.0 SDK
- SQL Server Express
- A code editor (Visual Studio, Visual Studio Code, Rider, etc.)

## Setup Instructions

### 1. Database Setup

1. Open SQL Server Management Studio or another SQL client
2. Connect to your SQL Server Express instance
3. Run the provided `script.sql` file to create the database schema and initial data

### 2. Configuration

1. Open the `appsettings.json` file in the `SearchAnalytics.API` project
2. Verify that the database connection string matches your SQL Server Express instance:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=SearchAnalyticsDB;Trusted_Connection=True;Encrypt=False"
   }
   ```
3. Update the connection string if necessary to match your SQL Server configuration

### 3. Running the Application

#### Using Visual Studio:

1. Open the `SearchAnalytics.sln` file in Visual Studio
2. Set `SearchAnalytics.API` as the startup project
3. Press F5 or click the Run button

#### Using .NET CLI:

1. Open a command prompt or terminal
2. Navigate to the `SearchAnalytics.API` directory
3. Run the following command:
   ```
   dotnet run
   ```

4. The API will be available at:
   - HTTPS: https://localhost:7282

5. Swagger UI will be available at:
   - HTTPS: https://localhost:7282/swagger


#### Important SSL Certificate Requirements:

The application runs exclusively on HTTPS for security purposes. You must verify that the .NET Developer Certificate is installed and trusted on your machine. For more information, see: https://aka.ms/aspnet/https-trust-dev-cert

If the .NET Developer Certificate is not trusted, you will see the following warning in the console:
```
warn: Microsoft.AspNetCore.Server.Kestrel.Core.KestrelServer[8]
The ASP.NET Core developer certificate is not trusted. For information about trusting the ASP.NET Core developer certificate, see https://aka.ms/aspnet/https-trust-dev-cert.
```

Quick Solution: Run the following command in your terminal or command prompt:
```
dotnet dev-certs https --trust
```

#### Frontend:

The application is configured to work with a frontend running on `http://localhost:3000`. This is specified in the CORS configuration in `Program.cs`:

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});
```

⚠️ **Important**: If you're running your frontend on a different port than 3000, you will encounter CORS errors. To fix this:

1. Update the CORS policy in `Program.cs` to include your frontend's URL
2. Rebuild and restart the API

## API Endpoints

- `POST /search` - Perform a new search with specified keyword and target URL
  - Request body example:
    ```json
    {
      "searchEngineId": 1,
      "targetUrl": "www.infotrack.co.uk",
      "keyword": "land registry searches",
      "topNResult": 100
    }
    ```

- `GET /search/results/{searchId}` - Get results for a specific search

- `GET /search/history` - Get search history

## Project Structure

- **SearchAnalytics.API**: Web API controllers and application startup configuration
- **SearchAnalytics.Core**: Business logic, models, DTOs, interfaces, and services
- **SearchAnalytics.DB**: Database context, entity configurations, and repository implementations

## Troubleshooting

- If searches consistently fail, wait at least 1 hour before trying again to allow Google's rate limiting to reset
- Verify that your firewall or antivirus is not blocking the application from making HTTP requests
- Check the console output for error messages

## Important Considerations

⚠️ **Rate Limiting Warning**: Google implements rate limiting for consecutive searches. If you make too many searches in a short period (especially with num=100 parameter), Google may temporarily block your IP address for approximately 1 hour, resulting in search errors.

⚠️ **Google Redirects**: When scraping Google search results, the system may encounter multiple redirects (currently set to handle up to 23 redirect attempts). If searches consistently fail:
- Try reducing the `TopNResult` value (e.g., search for top 50 instead of 100)
- You can adjust the `maxTryOut` value in the `SearchQueryService.cs` file if needed
- Wait longer between searches to avoid triggering Google's rate limiting


## Potential Improvements

- Add support for other search engines
- Implement caching to reduce the number of requests to Google
- Add authentication and user management
- Implement more detailed analytics and visualization of ranking trends
- Add automated scheduled searches
