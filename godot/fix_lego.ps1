param(
    [string]$GodotExe = ""
)

$ErrorActionPreference = "Stop"

$projectPath = Join-Path $PSScriptRoot "AstroLifeGodot"
$scriptPath = "res://Scripts/Tools/GenerateStationTileset.gd"

function Resolve-GodotExe {
    param([string]$Candidate)

    if ($Candidate -and (Test-Path $Candidate)) {
        return (Resolve-Path $Candidate).Path
    }

    $known = @(
        "C:\Users\mahaseias.carvalho\Downloads\Godot_v4.6.1-stable_mono_win64\Godot_v4.6.1-stable_mono_win64\Godot_v4.6.1-stable_mono_win64_console.exe",
        "C:\Users\mahaseias.carvalho\Downloads\Godot_v4.6.1-stable_mono_win64\Godot_v4.6.1-stable_mono_win64\Godot_v4.6.1-stable_mono_win64.exe"
    )

    foreach ($path in $known) {
        if (Test-Path $path) {
            return (Resolve-Path $path).Path
        }
    }

    $fromPath = Get-Command godot -ErrorAction SilentlyContinue
    if ($fromPath) {
        return $fromPath.Path
    }

    throw "Godot executable not found. Passe -GodotExe ""C:\caminho\Godot_v4.6.1-stable_mono_win64_console.exe""."
}

$exe = Resolve-GodotExe -Candidate $GodotExe

Write-Host "[fix_lego] Godot: $exe"
Write-Host "[fix_lego] Project: $projectPath"
Write-Host "[fix_lego] Script: $scriptPath"

& $exe --headless --path $projectPath --script $scriptPath
$exitCode = $LASTEXITCODE

if ($exitCode -ne 0) {
    throw "[fix_lego] Falhou com exit code $exitCode"
}

Write-Host "[fix_lego] OK. TileSet e TestLevel atualizados."
