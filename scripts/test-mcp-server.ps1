# Test MCP Server Endpoint

$baseUrl = "http://localhost:5005"

Write-Host "Testing MCP Server Endpoint..." -ForegroundColor Cyan

# Test 1: Initialize
Write-Host "`n1. Testing Initialize..." -ForegroundColor Yellow
$initBody = @{
    jsonrpc = "2.0"
    method = "initialize"
    id = "1"
} | ConvertTo-Json

try {
    $response = Invoke-RestMethod -Uri "$baseUrl/mcp" -Method Post -Body $initBody -ContentType "application/json"
    Write-Host "✓ Initialize successful" -ForegroundColor Green
    $response.result | ConvertTo-Json -Depth 3
} catch {
    Write-Host "✗ Initialize failed: $_" -ForegroundColor Red
}

# Test 2: List Tools
Write-Host "`n2. Testing List Tools..." -ForegroundColor Yellow
$listBody = @{
    jsonrpc = "2.0"
    method = "tools/list"
    id = "2"
} | ConvertTo-Json

try {
    $response = Invoke-RestMethod -Uri "$baseUrl/mcp" -Method Post -Body $listBody -ContentType "application/json"
    Write-Host "✓ List Tools successful" -ForegroundColor Green
    Write-Host "Found $($response.result.tools.Count) tools:" -ForegroundColor Cyan
    $response.result.tools | ForEach-Object { Write-Host "  - $($_.name): $($_.description)" }
} catch {
    Write-Host "✗ List Tools failed: $_" -ForegroundColor Red
}

# Test 3: Call a Tool (get_all_customers)
Write-Host "`n3. Testing Tool Call (get_all_customers)..." -ForegroundColor Yellow
$callBody = @{
    jsonrpc = "2.0"
    method = "tools/call"
    params = @{
        name = "get_all_customers"
        arguments = @{}
    }
    id = "3"
} | ConvertTo-Json -Depth 3

try {
    $response = Invoke-RestMethod -Uri "$baseUrl/mcp" -Method Post -Body $callBody -ContentType "application/json"
    Write-Host "✓ Tool call successful" -ForegroundColor Green
    $customers = $response.result.content[0].text | ConvertFrom-Json
    Write-Host "Retrieved $($customers.Count) customers" -ForegroundColor Cyan
} catch {
    Write-Host "✗ Tool call failed: $_" -ForegroundColor Red
}

# Test 4: Compare with REST API
Write-Host "`n4. Testing REST API (for comparison)..." -ForegroundColor Yellow
try {
    $restResponse = Invoke-RestMethod -Uri "$baseUrl/api/customers" -Method Get
    Write-Host "✓ REST API successful" -ForegroundColor Green
    Write-Host "Retrieved $($restResponse.Count) customers via REST" -ForegroundColor Cyan
} catch {
    Write-Host "✗ REST API failed: $_" -ForegroundColor Red
}

Write-Host "`n" -NoNewline
Write-Host "Testing complete!" -ForegroundColor Cyan
