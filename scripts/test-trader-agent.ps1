$url = "http://localhost:5003/api/agent/trader"
$body = "accout =1 buy, rate 1.22, for aud/usd, trading 10000"

$response = Invoke-RestMethod -Uri $url -Method Post -Body $body -ContentType "text/plain"
Write-Output $response
