# $url = "http://localhost:5001/insight"
$url = "https://fxag-research.azurewebsites.net/api/chat/ask"
$body = @{ "message" = "What is the current market outlook for AUD/USD?"; "temperature" = 0.7 } | ConvertTo-Json

$response = Invoke-RestMethod -Uri $url -Method Post -Body $body -ContentType "application/json"
Write-Output $response

