### PowerShell 7 or later

$outFile = Join-Path $(Get-Location) "syukujitsu.csv.header"
$inFile = $outFile + ".orig"
# BOM無しUTF-8
$utf8n = [System.Text.UTF8Encoding]::new($false)
try
{
    $r = Get-Content -Path $inFile -Encoding UTF8 -ReadCount 0
    $w = [System.IO.StreamWriter]::new($outFile, $false, $utf8n)
    $w.NewLine = "`n" # 改行コードをLFに変更
    foreach ($line in $r)
    {
        $entry = $line.Split(":", 2, [System.StringSplitOptions]::TrimEntries)
        switch -CaseSensitive ($entry[0])
        {
            "Last-Modified"  { $w.WriteLine($line) }
            "Content-Length" { $w.WriteLine($line) }
        }
    }
    Write-Host "[INFO] ヘッダから抜粋した情報を $outFile に保存しました。"
}
finally
{
    if ($w) { $w.Close() }
}
