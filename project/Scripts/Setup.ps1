# Setup.ps1
# Copyright: © SPT-AKI 2020
# License: NCSA
# Authors:
# - Merijn Hendriks
# - waffle.lord

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

# make sure msbuild ins't empty and that the path exists, otherwise warn and exit.
if (($msbuild -ne "") -and(Test-Path $msbuild))
{
    $userPath = [System.Environment]::GetEnvironmentVariable("Path", "Machine")

    if ($userPath.Contains($msbuild))
    {
        Write-Host "Path Contains MSBuild already"
    }
    else
    {
        $pathLength = $userPath.Length + $msbuild.Length

        # max directory length is somewhere around 246 characters, max path variable length is 2047 characters
        if (($msbuild.Length -gt 240) -or($pathLength -gt 2047))
        {
            Write-Warning "Path is too long"
        }
        else
        {
            $userPath += $msbuild

            # for testing to check the path looks ok
            Write-Host ""
            Write-Host "Added MSBuild to path"
            Write-Host $userPath -ForegroundColor Cyan

            # commented out for testing
            [System.Environment]::SetEnvironmentVariable("Path", $userPath, "Machine")
        }
    }
}
else
{
    Write-Warning "Could not find Microsoft Buildtools"
}