param(
    [string]$DataPath = "$PSScriptRoot/../MockData",
    [string]$ConnectionString
)

$ErrorActionPreference = "Stop"

if (-not $ConnectionString) {
    $appSettings = Get-Content (Join-Path $DataPath "../appsettings.json") -Raw | ConvertFrom-Json
    $ConnectionString = $appSettings.ConnectionStrings.FxDatabase
}

Import-Module SqlServer -ErrorAction Stop

function Format-SqlValue($value, $type) {
    if ($null -eq $value) { return "NULL" }
    switch ($type) {
        "int"       { return [int]$value }
        "decimal"   { return [decimal]$value }
        "bool"      { return $(if ($value) { 1 } else { 0 }) }
        "string"    { return "N'$([string]$value -replace "'","''")'" }
        "datetime"  { return "'$(([DateTime]$value).ToString('yyyy-MM-ddTHH:mm:ss'))'" }
        "datetime?" {
            if ($null -eq $value -or [string]::IsNullOrEmpty($value)) { return "NULL" }
            return "'$(([DateTime]$value).ToString('yyyy-MM-ddTHH:mm:ss'))'"
        }
    }
}

$tableConfigs = @(
    @{
        File = "customers.json"; Table = "Customers"
        Columns = @(
            @{ J="id"; S="Id"; T="int" }, @{ J="name"; S="Name"; T="string" },
            @{ J="email"; S="Email"; T="string" }, @{ J="phone"; S="Phone"; T="string" },
            @{ J="company"; S="Company"; T="string" }, @{ J="createdAt"; S="CreatedAt"; T="datetime" }
        )
    },
    @{
        File = "research-articles.json"; Table = "ResearchArticles"
        Columns = @(
            @{ J="id"; S="Id"; T="int" }, @{ J="title"; S="Title"; T="string" },
            @{ J="summary"; S="Summary"; T="string" }, @{ J="content"; S="Content"; T="string" },
            @{ J="category"; S="Category"; T="string" }, @{ J="author"; S="Author"; T="string" },
            @{ J="publishedDate"; S="PublishedDate"; T="datetime" }, @{ J="status"; S="Status"; T="string" },
            @{ J="tags"; S="Tags"; T="string" }, @{ J="sentiment"; S="Sentiment"; T="string" }
        )
    },
    @{
        File = "traders.json"; Table = "Traders"
        Columns = @(
            @{ J="id"; S="Id"; T="int" }, @{ J="name"; S="Name"; T="string" },
            @{ J="email"; S="Email"; T="string" }, @{ J="desk"; S="Desk"; T="string" },
            @{ J="specialization"; S="Specialization"; T="string" }, @{ J="region"; S="Region"; T="string" },
            @{ J="isActive"; S="IsActive"; T="bool" }, @{ J="joinedAt"; S="JoinedAt"; T="datetime" }
        )
    },
    @{
        File = "customer-preferences.json"; Table = "CustomerPreferences"
        Columns = @(
            @{ J="id"; S="Id"; T="int" }, @{ J="customerId"; S="CustomerId"; T="int" },
            @{ J="preferredCurrencyPairs"; S="PreferredCurrencyPairs"; T="string" },
            @{ J="riskTolerance"; S="RiskTolerance"; T="string" },
            @{ J="maxPositionSize"; S="MaxPositionSize"; T="decimal" },
            @{ J="stopLossPercent"; S="StopLossPercent"; T="decimal" },
            @{ J="takeProfitPercent"; S="TakeProfitPercent"; T="decimal" },
            @{ J="tradingStyle"; S="TradingStyle"; T="string" },
            @{ J="tradingObjective"; S="TradingObjective"; T="string" },
            @{ J="enableNotifications"; S="EnableNotifications"; T="bool" },
            @{ J="notificationChannels"; S="NotificationChannels"; T="string" },
            @{ J="updatedAt"; S="UpdatedAt"; T="datetime" }
        )
    },
    @{
        File = "customer-portfolios.json"; Table = "CustomerPortfolios"
        Columns = @(
            @{ J="id"; S="Id"; T="int" }, @{ J="customerId"; S="CustomerId"; T="int" },
            @{ J="currencyPair"; S="CurrencyPair"; T="string" }, @{ J="direction"; S="Direction"; T="string" },
            @{ J="amount"; S="Amount"; T="decimal" }, @{ J="entryRate"; S="EntryRate"; T="decimal" },
            @{ J="openedAt"; S="OpenedAt"; T="datetime" }, @{ J="status"; S="Status"; T="string" }
        )
    },
    @{
        File = "customer-histories.json"; Table = "CustomerHistories"
        Columns = @(
            @{ J="id"; S="Id"; T="int" }, @{ J="customerId"; S="CustomerId"; T="int" },
            @{ J="currencyPair"; S="CurrencyPair"; T="string" }, @{ J="direction"; S="Direction"; T="string" },
            @{ J="amount"; S="Amount"; T="decimal" }, @{ J="entryRate"; S="EntryRate"; T="decimal" },
            @{ J="exitRate"; S="ExitRate"; T="decimal" }, @{ J="pnL"; S="PnL"; T="decimal" },
            @{ J="openedAt"; S="OpenedAt"; T="datetime" }, @{ J="closedAt"; S="ClosedAt"; T="datetime" },
            @{ J="notes"; S="Notes"; T="string" }
        )
    },
    @{
        File = "research-drafts.json"; Table = "ResearchDrafts"
        Columns = @(
            @{ J="id"; S="Id"; T="int" }, @{ J="title"; S="Title"; T="string" },
            @{ J="content"; S="Content"; T="string" }, @{ J="author"; S="Author"; T="string" },
            @{ J="category"; S="Category"; T="string" }, @{ J="tags"; S="Tags"; T="string" },
            @{ J="status"; S="Status"; T="string" }, @{ J="version"; S="Version"; T="int" },
            @{ J="reviewerNotes"; S="ReviewerNotes"; T="string" },
            @{ J="createdAt"; S="CreatedAt"; T="datetime" }, @{ J="updatedAt"; S="UpdatedAt"; T="datetime?" }
        )
    },
    @{
        File = "research-patterns.json"; Table = "ResearchPatterns"
        Columns = @(
            @{ J="id"; S="Id"; T="int" }, @{ J="currencyPair"; S="CurrencyPair"; T="string" },
            @{ J="patternName"; S="PatternName"; T="string" }, @{ J="timeframe"; S="Timeframe"; T="string" },
            @{ J="direction"; S="Direction"; T="string" }, @{ J="confidenceScore"; S="ConfidenceScore"; T="decimal" },
            @{ J="description"; S="Description"; T="string" }, @{ J="detectedBy"; S="DetectedBy"; T="string" },
            @{ J="detectedAt"; S="DetectedAt"; T="datetime" }, @{ J="expiresAt"; S="ExpiresAt"; T="datetime?" },
            @{ J="status"; S="Status"; T="string" }
        )
    },
    @{
        File = "trader-recommendations.json"; Table = "TraderRecommendations"
        Columns = @(
            @{ J="id"; S="Id"; T="int" }, @{ J="traderId"; S="TraderId"; T="int" },
            @{ J="currencyPair"; S="CurrencyPair"; T="string" }, @{ J="direction"; S="Direction"; T="string" },
            @{ J="targetRate"; S="TargetRate"; T="decimal" }, @{ J="stopLoss"; S="StopLoss"; T="decimal" },
            @{ J="confidence"; S="Confidence"; T="string" }, @{ J="rationale"; S="Rationale"; T="string" },
            @{ J="status"; S="Status"; T="string" }, @{ J="createdAt"; S="CreatedAt"; T="datetime" },
            @{ J="expiresAt"; S="ExpiresAt"; T="datetime?" }
        )
    },
    @{
        File = "trader-newsfeeds.json"; Table = "TraderNewsFeeds"
        Columns = @(
            @{ J="id"; S="Id"; T="int" }, @{ J="traderId"; S="TraderId"; T="int" },
            @{ J="headline"; S="Headline"; T="string" }, @{ J="source"; S="Source"; T="string" },
            @{ J="category"; S="Category"; T="string" }, @{ J="currencyPairs"; S="CurrencyPairs"; T="string" },
            @{ J="sentiment"; S="Sentiment"; T="string" }, @{ J="summary"; S="Summary"; T="string" },
            @{ J="isRead"; S="IsRead"; T="bool" }, @{ J="publishedAt"; S="PublishedAt"; T="datetime" }
        )
    },
    @{
        File = "trader-suggestions.json"; Table = "TraderSuggestions"
        Columns = @(
            @{ J="id"; S="Id"; T="int" }, @{ J="traderId"; S="TraderId"; T="int" },
            @{ J="customerId"; S="CustomerId"; T="int" }, @{ J="researchArticleId"; S="ResearchArticleId"; T="int" },
            @{ J="reasoning"; S="Reasoning"; T="string" }, @{ J="relevanceScore"; S="RelevanceScore"; T="string" },
            @{ J="status"; S="Status"; T="string" }, @{ J="createdAt"; S="CreatedAt"; T="datetime" }
        )
    }
)

# Delete in reverse FK order
$deleteOrder = @(
    "TraderSuggestions", "TraderNewsFeeds", "TraderRecommendations",
    "CustomerHistories", "CustomerPreferences", "CustomerPortfolios",
    "Traders", "ResearchPatterns", "ResearchDrafts",
    "Customers", "ResearchArticles"
)

Write-Host "Running seed.sql..."
$seedSqlPath = Join-Path $DataPath "seed.sql"
Invoke-Sqlcmd -ConnectionString $ConnectionString -InputFile $seedSqlPath
Write-Host "  seed.sql completed."

# Insert data from JSON files
foreach ($config in $tableConfigs) {
    $filePath = Join-Path $DataPath $config.File
    if (-not (Test-Path $filePath)) {
        Write-Warning "Skipping $($config.Table) - file not found: $($config.File)"
        continue
    }

    $data = Get-Content $filePath -Raw | ConvertFrom-Json
    if ($data.Count -eq 0) {
        Write-Warning "Skipping $($config.Table) - no records in $($config.File)"
        continue
    }

    $colNames = ($config.Columns | ForEach-Object { $_.S }) -join ", "
    Write-Host "Inserting $($data.Count) records into $($config.Table)..."
    $inserted = 0
    $sb = [System.Text.StringBuilder]::new()
    $sb.AppendLine("SET IDENTITY_INSERT $($config.Table) ON;") | Out-Null

    foreach ($row in $data) {
        $vals = foreach ($col in $config.Columns) {
            Format-SqlValue $row.($col.J) $col.T
        }
        $sb.AppendLine("BEGIN TRY") | Out-Null
        $sb.AppendLine("  INSERT INTO $($config.Table) ($colNames) VALUES ($($vals -join ', '));") | Out-Null
        $sb.AppendLine("END TRY BEGIN CATCH END CATCH") | Out-Null
    }

    $sb.AppendLine("SET IDENTITY_INSERT $($config.Table) OFF;") | Out-Null

    Invoke-Sqlcmd -ConnectionString $ConnectionString -Query $sb.ToString()
    Write-Host "  Done."
}

Write-Host "Seed completed."
