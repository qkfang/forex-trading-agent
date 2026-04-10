# $url = "http://localhost:5001/insight"
$url = "https://fxag-agent.azurewebsites.net/insight"
$body = @{ Message = "china market" } | ConvertTo-Json

$response = Invoke-RestMethod -Uri $url -Method Post -Body $body -ContentType "application/json"
Write-Output $response

