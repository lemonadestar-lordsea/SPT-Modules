# Cleanup.ps1
# Copyright: © SPT-AKI 2020
# License: NCSA
# Authors:
# - Merijn Hendriks
# - waffle.lord

$rootPath = Resolve-Path -path "."

# get directories
$binPaths = Get-ChildItem -Recurse -Path $rootPath | where {$_.FullName -like "*\bin"} | select -ExpandProperty FullName
$objectPaths = Get-ChildItem -Recurse -Path $rootPath | where {$_.FullName -like "*\obj"} | select -ExpandProperty FullName

# delete build waste
foreach ($path in $binPaths)
{
    Write-Host "Delete: $($path)"
    Remove-Item $path -Force -Recurse
}

foreach ($path in $objectPaths)
{
    Write-Host "Delete: $($path)"
    Remove-Item $path -Force -Recurse
}