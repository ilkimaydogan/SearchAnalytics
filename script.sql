
-- Create database
CREATE DATABASE SearchAnalyticsDB;
GO

-- Use the SearchAnalytics database
USE SearchAnalyticsDB;
GO

-- Create SearchEngines table
CREATE TABLE SearchEngines (
                               Id INT PRIMARY KEY IDENTITY(1,1),
                               Name NVARCHAR(50) NOT NULL,
                               Pattern NVARCHAR(MAX) NOT NULL,
                               IsActive BIT NOT NULL DEFAULT 1
);
GO

-- Create Searches table
CREATE TABLE Searches (
                          Id INT PRIMARY KEY IDENTITY(1,1),
                          SearchEngineId INT NOT NULL,
                          TargetUrl NVARCHAR(255) NOT NULL,
                          Keyword NVARCHAR(255) NOT NULL,
                          TopNResult INT NOT NULL DEFAULT 100,
                          Date DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
                          FOREIGN KEY (SearchEngineId) REFERENCES SearchEngines(Id)
);
GO

-- Create SearchResults table
CREATE TABLE SearchResults (
                               Id INT PRIMARY KEY IDENTITY(1,1),
                               SearchId INT NOT NULL,
                               Position INT NOT NULL,
                               Url NVARCHAR(500) NOT NULL,
                               CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
                               FOREIGN KEY (SearchId) REFERENCES Searches(Id)
);
GO

-- Insert initial data for SearchEngines
INSERT INTO SearchEngines (Name, Pattern, IsActive)
VALUES 
(
    'Google', 
    '<a\s+jsname="UWckNb"\s+class="zReHs"\s+href="(?<hrefContent>[^"]+)"\s+[^>]*>.*?<h3\s+class="LC20lb[^"]*"[^>]*>(.*?)</h3>', 
    1
);
GO

-- Create indexes for better performance
CREATE INDEX IX_Searches_SearchEngineId ON Searches(SearchEngineId);
CREATE INDEX IX_Searches_Date ON Searches(Date);
CREATE INDEX IX_Searches_Keyword ON Searches(Keyword);
CREATE INDEX IX_SearchResults_SearchId ON SearchResults(SearchId);
CREATE INDEX IX_SearchResults_Position ON SearchResults(Position);
GO

