param(
    [Parameter(Mandatory=$true)]
    [string]$directory
)

$outputFile = Join-Path -Path $PSScriptRoot -ChildPath "output.csv"

$processedFiles = @{}

Get-ChildItem -Path $directory -Include *.dll,*.exe -Recurse |
ForEach-Object {
    if ($processedFiles.ContainsKey($_.Name) -or $_.Name.StartsWith("Demant") -or $_.Name.StartsWith("WDH") -or $_.Name.StartsWith("Cosmos") -or $_.Name.StartsWith("Wdh") -or $_.Name.StartsWith("Phoenix")) {
        return
    }

    $processedFiles[$_.Name] = $true

    $sig = Get-AuthenticodeSignature -FilePath $_.FullName
    [PSCustomObject]@{
        'File' = $_.Name
        'Status' = if ($sig.Status -eq 'NotSigned') { "NOT-SIGNED" } else { $sig.Status }
        'SignerCertificate' = if ($sig.SignerCertificate) { 
            $o = ($sig.SignerCertificate.Subject -split ',' | Where-Object { $_.Trim().StartsWith("O=") }) -replace "O=",""
            "$o"
        } else { "NOT-SIGNED" }
        'DigestAlgorithm' = if ($sig.SignerCertificate) { $sig.SignerCertificate.SignatureAlgorithm.FriendlyName } else { "NOT-SIGNED" }
        'TimeStamperCertificate' = if ($sig.TimeStamperCertificate) { 
            $ou = ($sig.TimeStamperCertificate.Subject -split ',' | Where-Object { $_.Trim().StartsWith("OU=") }) -replace "OU=",""
            $ou -join ', '
        } else { "NOT-SIGNED" }
    }
} | Export-Csv -Path $outputFile -NoTypeInformation