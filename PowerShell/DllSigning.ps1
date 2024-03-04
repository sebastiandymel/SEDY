param(
    [Parameter(Mandatory=$true)]
    [string]$directory
)

$outputFile = Join-Path -Path $PSScriptRoot -ChildPath "output.txt"

$processedFiles = @{}

Get-ChildItem -Path $directory -Include *.exe -Recurse |
ForEach-Object {
    if ($processedFiles.ContainsKey($_.Name)) {
        return
    }

    $processedFiles[$_.Name] = $true

    $sig = Get-AuthenticodeSignature -FilePath $_.FullName
    $output = @"
File: $($_.Name)  # The name of the file
Status: $(if ($sig.Status -eq 'NotSigned') { "NOT-SIGNED" } else { $sig.Status })  # The status of the signature. If it's 'NotSigned', the file is not signed. Otherwise, it shows the status of the signature.
Signer Certificate: $(if ($sig.SignerCertificate) { ($sig.SignerCertificate.Subject -split ',' | Where-Object { $_.Trim().StartsWith("O=") }) -replace "O=","" } else { "NOT-SIGNED" })  # The signer certificate. This is the certificate used to sign the file.
Digest Algorithm: $(if ($sig.SignerCertificate) { $sig.SignerCertificate.SignatureAlgorithm.FriendlyName } else { "NOT-SIGNED" })  # The digest algorithm used for the signature. This is the algorithm used to create the hash of the file that is signed.
Time Stamper Certificate: $(if ($sig.TimeStamperCertificate) { ($sig.TimeStamperCertificate.Subject -split ',' | Where-Object { $_.Trim().StartsWith("OU=") }) -replace "OU=","" -join ', ' } else { "NOT-SIGNED" })  # The time stamper certificate. This is the certificate of the authority that timestamped the signature.
Issuer: $(if ($sig.SignerCertificate) { $sig.SignerCertificate.Issuer } else { "NOT-SIGNED" })  # The issuer of the signer certificate. This is the authority that issued the signer certificate.
Subject: $(if ($sig.SignerCertificate) { $sig.SignerCertificate.Subject } else { "NOT-SIGNED" })  # The subject of the signer certificate. This is the entity that the signer certificate was issued to.
Signature Algorithm: $(if ($sig.SignerCertificate) { $sig.SignerCertificate.SignatureAlgorithm.FriendlyName } else { "NOT-SIGNED" })  # The signature algorithm used for the signer certificate. This is the algorithm used to create the signature.
Valid From: $(if ($sig.SignerCertificate) { $sig.SignerCertificate.NotBefore } else { "NOT-SIGNED" })  # The validity start date of the signer certificate. This is the date from which the signer certificate is valid.
Valid To: $(if ($sig.SignerCertificate) { $sig.SignerCertificate.NotAfter } else { "NOT-SIGNED" })  # The validity end date of the signer certificate. This is the date until which the signer certificate is valid.
"@
    Add-Content -Path $outputFile -Value $output
}