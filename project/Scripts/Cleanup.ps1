# Cleanup.ps1
# Copyright: © SPT-AKI 2020
# License: NCSA
# Authors:
# - Merijn Hendriks
# - waffle.lord

$outputFolder = "../Build/"
$rootPath = Resolve-Path -path "../"
$outputPath = Join-Path -Path $rootPath -ChildPath $outputFolder

# get directories
$debugPaths = Get-ChildItem -Recurse -Path $rootPath | where {$_.FullName -like "*\bin\debug"} | select -ExpandProperty FullName
$releasePaths = Get-ChildItem -Recurse -Path $rootPath | where {$_.FullName -like "*\bin\Release"} | select -ExpandProperty FullName
$binPaths = Get-ChildItem -Recurse -Path $rootPath | where {$_.FullName -like "*\bin"} | select -ExpandProperty FullName
$objectPaths = Get-ChildItem -Recurse -Path $rootPath | where {$_.FullName -like "*\obj"} | select -ExpandProperty FullName

# delete the old output folder
if (Test-Path $outputPath)
{
    Remove-Item $outputPath -Force -Recurse
}

# create a new output folder
New-Item -Path $rootPath -Name $outputFolder -ItemType "directory"

# copy from debug
foreach ($path in $debugPaths)
{
    Write-Host "Copy: $($path)"
    Copy-Item -Path "$($path)\*" -Destination $outputPath -Recurse
}

# copy from release
foreach ($path in $releasePaths)
{
    Write-Host "Copy: $($path)"
    Copy-Item -Path "$($path)\*" -Destination $outputPath -Recurse
}

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