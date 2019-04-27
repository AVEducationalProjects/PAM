param (
	[Parameter(Mandatory=$true)][string]$password,
	[Parameter(Mandatory=$true)][string]$filename
)

function Generate-SelfSigned-Certificate {
	param (
		[Parameter(Mandatory=$true)][string]$path,
		[Parameter(Mandatory=$true)][string]$certificatename,
		[Parameter(Mandatory=$true)][SecureString]$certificatepassword
	)

	# setup certificate properties including the commonName (DNSName) property for Chrome 58+
	$certificate = New-SelfSignedCertificate `
		-Subject localhost `
		-DnsName localhost `
		-KeyAlgorithm RSA `
		-KeyLength 2048 `
	    -NotBefore (Get-Date) `
		-NotAfter (Get-Date).AddYears(1) `
		-CertStoreLocation "cert:CurrentUser\My" `
		-FriendlyName "Localhost Certificate for .NET Core" `
		-HashAlgorithm SHA256 `
	    -KeyUsage DigitalSignature, KeyEncipherment, DataEncipherment `
		-TextExtension @("2.5.29.37={text}1.3.6.1.5.5.7.3.1") 

	$certificatePath = 'Cert:\CurrentUser\My\' + ($certificate.ThumbPrint)

	# set certificate password here
	$pfxPassword = $certificatepassword
	$pfxFilePath = $path + "\" + $certificatename + ".pfx"
	$cerFilePath = $path + "\" + $certificatename + ".cer"
	# create pfx certificate
	Export-PfxCertificate -Cert $certificatePath -FilePath $pfxFilePath -Password $pfxPassword
	Export-Certificate -Cert $certificatePath -FilePath $cerFilePath
	# import the pfx certificate
	Import-PfxCertificate -FilePath $pfxFilePath Cert:\LocalMachine\My -Password $pfxPassword -Exportable
	# trust the certificate by importing the pfx certificate into your trusted root
	Import-Certificate -FilePath $cerFilePath -CertStoreLocation Cert:\CurrentUser\Root
}


$pwd = $password | ConvertTo-SecureString -AsPlainText -Force
Generate-SelfSigned-Certificate ".\" $filename $pwd