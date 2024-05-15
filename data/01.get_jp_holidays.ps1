### PowerShell 7 or later

$uri = "https://www8.cao.go.jp/chosei/shukujitsu/syukujitsu.csv"
$outFile = Join-Path $(Get-Location) "syukujitsu.csv"
$outHeader = $outFile + ".header.orig"

try
{
    $request = [Net.WebRequest]::Create($uri)
    $response = $request.GetResponse()
    $response.Headers.ToString() | Out-File -FilePath $outHeader
    Write-Host "[INFO] ヘッダを $outHeader に保存しました。"
    $fs = [System.IO.File]::Create($outFile)
    $response.GetResponseStream().CopyTo($fs)
    Write-Host "[INFO] CSVを $outFile に保存しました。"
}
finally
{
    if ($fs) { $fs.Close() }
    if ($response) { $response.Close() }
}
