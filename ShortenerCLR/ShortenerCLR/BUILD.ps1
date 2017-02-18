$data = Get-Content ShortenerCLR.dll -Encoding byte -ReadCount 0
$hex = ''
foreach ($byte in $data){ 
    $hex += "{0:X2}" -f $byte
}

(Get-Content Deploy.sql) -replace "'<binary data>'", ('0x' + $hex) | Set-Content Deploy.sql

