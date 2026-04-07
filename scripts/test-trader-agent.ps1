$url = "http://localhost:5003/api/agent/trader"
$body = "What is the current EUR/USD trend and should I buy or sell?"

$response = Invoke-RestMethod -Uri $url -Method Post -Body $body -ContentType "text/plain"
Write-Output $response
