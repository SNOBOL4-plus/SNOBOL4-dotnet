# build_native.ps1 — build all C fixture DLLs for Windows x64.
# Run from the solution root in any PowerShell prompt:
#   .\build_native.ps1
#
# Requires Visual Studio with the C++ workload installed.

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

# ── Locate vcvars64.bat via vswhere ─────────────────────────────────────────
$vswhere = "${env:ProgramFiles(x86)}\Microsoft Visual Studio\Installer\vswhere.exe"
if (-not (Test-Path $vswhere)) {
    Write-Error "vswhere.exe not found at: $vswhere`nIs Visual Studio installed?"
}

$vcvars = & $vswhere -latest -products * `
    -requires Microsoft.VisualCpp.Tools.HostX64.TargetX64 `
    -find "VC\Auxiliary\Build\vcvars64.bat"

if (-not $vcvars) {
    Write-Error "Could not find vcvars64.bat. Install the 'Desktop development with C++' workload."
}

Write-Host "Using toolchain: $vcvars" -ForegroundColor Cyan

# ── Library definitions ──────────────────────────────────────────────────────
$root = $PSScriptRoot

$libs = @(
    @{
        Dir    = "$root\CustomFunction\SpitbolCLib"
        Src    = "spitbol_math.c"
        Out    = "libspitbol_math.dll"
        Pdb    = "libspitbol_math.pdb"
    },
    @{
        Dir    = "$root\CustomFunction\SpitbolCreateLib"
        Src    = "spitbol_create.c"
        Out    = "libspitbol_create.dll"
        Pdb    = "libspitbol_create.pdb"
    },
    @{
        Dir    = "$root\CustomFunction\SpitbolNoconvLib"
        Src    = "spitbol_noconv.c"
        Out    = "libspitbol_noconv.dll"
        Pdb    = "libspitbol_noconv.pdb"
    },
    @{
        Dir    = "$root\CustomFunction\SpitbolXnLib"
        Src    = "spitbol_xn.c"
        Out    = "libspitbol_xn.dll"
        Pdb    = "libspitbol_xn.pdb"
    }
)

# ── Build each library ───────────────────────────────────────────────────────
$failed = @()

foreach ($lib in $libs) {
    Write-Host ""
    Write-Host "── Building $($lib.Out) ──" -ForegroundColor Yellow

    $cl = "cl.exe /nologo /Od /Zi /MDd /D_DEBUG " +
          "/Fe:`"$($lib.Dir)\$($lib.Out)`" " +
          "`"$($lib.Dir)\$($lib.Src)`" " +
          "/link /DLL /MACHINE:X64 /DEBUG " +
          "/PDB:`"$($lib.Dir)\$($lib.Pdb)`""

    $cmd = "`"$vcvars`" && cd /d `"$($lib.Dir)`" && $cl"
    cmd /c $cmd

    if ($LASTEXITCODE -ne 0) {
        Write-Host "FAILED: $($lib.Out)" -ForegroundColor Red
        $failed += $lib.Out
    } else {
        Write-Host "OK: $($lib.Out)" -ForegroundColor Green
    }
}

# ── Summary ──────────────────────────────────────────────────────────────────
Write-Host ""
if ($failed.Count -eq 0) {
    Write-Host "All native libraries built successfully." -ForegroundColor Green
} else {
    Write-Host "Failed: $($failed -join ', ')" -ForegroundColor Red
    exit 1
}
