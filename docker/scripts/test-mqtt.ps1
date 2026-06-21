# Send a single test telemetry reading to ThingsBoard via MQTT.
# Usage (from repo root): .\docker\scripts\test-mqtt.ps1

$Token = "jxO93T5o6cCmLkOt0NVc"
$Payload = '{"heartRate":72,"spo2":98}'

Write-Host "Sending test telemetry to ThingsBoard..."
docker run --rm eclipse-mosquitto mosquitto_pub `
    -d -q 1 `
    -h host.docker.internal `
    -p 1883 `
    -t v1/devices/me/telemetry `
    -u $Token `
    -m $Payload

if ($LASTEXITCODE -eq 0) {
    Write-Host "Success. Refresh device page -> En son telemetri tab."
} else {
    Write-Host "Failed. Is Docker / ThingsBoard running?"
}
