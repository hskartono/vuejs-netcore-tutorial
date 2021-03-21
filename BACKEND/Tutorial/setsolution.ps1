$s = "Tutorial."
$r = "Tutorial."

Get-ChildItem "D:\Kantor\AMN\Training-Net-Core\Generated\BACKEND\Tutorial" -Recurse -Filter *.* | ForEach-Object {
    (Get-Content $_.FullName) |
        ForEach-Object { $_ -replace [regex]::Escape($s), $r } |
        Set-Content $_.FullName
}

Rename-Item D:\Kantor\AMN\Training-Net-Core\Generated\BACKEND\Tutorial\Tutorial.sln Tutorial.sln
