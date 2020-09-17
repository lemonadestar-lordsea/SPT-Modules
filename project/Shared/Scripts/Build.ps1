# Build.ps1
# Copyright: © SPT-AKI 2020
# License: NCSA
# Authors:
# - Merijn Hendriks
# - waffle.lord

$rootPath = Resolve-Path -path "."

# locate msbuild
Write-Host "Scanning for build tools..."

$vsWhere = "${env:ProgramFiles(x86)}\Microsoft Visual Studio\Installer\vswhere.exe"

if (-not(Test-Path $vsWhere))
{
    Write-Warning "  Could not find VSWhere.exe, please install BuildTools 2017 or newer"
    return
}

$msbuild = & $vsWhere -nologo -latest -find "MSBuild\Current\bin" | Out-String

if ($msbuild -eq "")
{
    # if no releases are found, check prerelease versions
    $msbuild = & $vsWhere -nologo -prerelease -find "MSBuild\Current\bin" | Out-String
}

$msbuild = $msbuild -replace "`n","" -replace "`r",""
$msbuild = "$($msbuild)\MSBuild.exe"

if (($msbuild -eq("")) -or(-Not(Test-Path $msbuild)))
{
    # make sure msbuild ins't empty and that the path exists, otherwise warn and exit.
    Write-Warning "  Could not find Microsoft Buildtools"
    return
}

Write-Host "  Found MSBuild.exe"
Write-Host ""

# restore nuget packages
Write-Host "Restore nuget packages..."
Start-Process -FilePath $msbuild -NoNewWindow -Wait -ArgumentList "-nologo /verbosity:minimal -consoleloggerparameters:Summary -t:restore -p:Configuration=Release Modules.sln"
Write-Host ""

# build the project
Write-Host "Building project..."
Start-Process -FilePath $msbuild -NoNewWindow -Wait -ArgumentList "-nologo /verbosity:minimal -consoleloggerparameters:Summary -t:rebuild -p:Configuration=Release Modules.sln"
Write-Host ""

# delete build waste
Write-Host "Cleaning garbage produced by build..."

$binPaths = Get-ChildItem -Recurse -Path $rootPath | where {$_.FullName -like "*\bin"} | select -ExpandProperty FullName
$objectPaths = Get-ChildItem -Recurse -Path $rootPath | where {$_.FullName -like "*\obj"} | select -ExpandProperty FullName

foreach ($path in $binPaths)
{
    Write-Host "  Delete: $($path)"
    Remove-Item $path -Force -Recurse
}

foreach ($path in $objectPaths)
{
    Write-Host "  Delete: $($path)"
    Remove-Item $path -Force -Recurse
}

Write-Host ""
Write-Host "Done building"
