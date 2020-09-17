# Build.ps1
# Copyright: © SPT-AKI 2020
# License: NCSA
# Authors:
# - Merijn Hendriks
# - waffle.lord

$rootPath = Resolve-Path -path "."
$vsWhere = "${env:ProgramFiles(x86)}\Microsoft Visual Studio\Installer\vswhere.exe"

# make sure vsWhere.exe exists, otherwise warn and exit script
if (-not(Test-Path $vsWhere))
{
    Write-Warning "Could not find Microsoft Buildtools"
    return
}

$msbuildPath = & $vsWhere -nologo -latest -find "MSBuild\Current\bin" | Out-String

# if no releases are found, check prerelease versions
if ($msbuildPath -eq "")
{
    $msbuildpath = & $vsWhere -nologo -prerelease -find "MSBuild\Current\bin" | Out-String
}

$msbuildPath = $msbuildPath -replace "`n","" -replace "`r",""

# make sure msbuild ins't empty and that the path exists, otherwise warn and exit.
if (($msbuildPath -ne "") -and(Test-Path $msbuildPath))
{
    $msbuild = "$($msbuildPath)\MSBuild.exe"

    Start-Process -FilePath $msbuild -ArgumentList "/property:GenerateFullPaths=true /consoleloggerparameters:NoSummary /t:restore /p:Configuration=Release Modules.sln" -NoNewWindow -Wait
    Start-Process -FilePath $msbuild -ArgumentList "/property:GenerateFullPaths=true /consoleloggerparameters:NoSummary /t:rebuild /p:Configuration=Release Modules.sln" -NoNewWindow -Wait
}

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
