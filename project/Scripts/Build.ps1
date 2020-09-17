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

$msbuild = & $vsWhere -nologo -latest -find "MSBuild\Current\bin" | Out-String

# if no releases are found, check prerelease versions
if ($msbuild -eq "")
{
    $msbuild = & $vsWhere -nologo -prerelease -find "MSBuild\Current\bin" | Out-String
}

$msbuild = $msbuild -replace "`n","" -replace "`r",""
$msbuild = "$($msbuild)\MSBuild.exe"

# make sure msbuild ins't empty and that the path exists, otherwise warn and exit.
if (($msbuild -ne "") -and(Test-Path $msbuild))
{
    Write-Host "Found MSBuild.exe, building project"
    Start-Process -FilePath $msbuild -NoNewWindow -Wait -ArgumentList "-nologo /verbosity:minimal -consoleloggerparameters:Summary -t:restore -p:Configuration=Release Modules.sln"
    Start-Process -FilePath $msbuild -NoNewWindow -Wait -ArgumentList "-nologo /verbosity:minimal -consoleloggerparameters:Summary -t:rebuild -p:Configuration=Release Modules.sln"
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
