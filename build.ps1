param($Configuration = "Debug", [switch]$NoRebuild)

$SourceRoot = Split-Path -Parent $MyInvocation.MyCommand.Path

# Check for the Bot directory and create it if it does not exist
if(!(Test-Path "Bot")) {
    mkdir Bot | Out-Null
}
$BotDir = Resolve-Path "Bot"
Write-Host "Bot = $BotDir"

# Build the project
if(!$NoRebuild) {
    msbuild "$SourceRoot\NuBot.sln" /p:Configuration="$Configuration"
}

function Get-Outputs {
    param($ProjectRoot)
    dir "$ProjectRoot\bin\$Configuration" | where {
        ($_.Extension -eq ".exe") -or ($_.Extension -eq ".dll") -or ($_.Extension -eq ".pdb")
    }
}

# Catalog Files to copy for the runner
$BotAppOutputs = Get-Outputs "$SourceRoot\Source\NuBot"

Write-Host "Updating Bot Runner Files"
$BotAppOutputs | Foreach { Copy-Item $_.FullName (Join-Path $BotDir $_.Name) }

$PartsRoot = Join-Path $BotDir "Parts"
if(!(Test-Path $PartsRoot)) {
    mkdir $PartsRoot | Out-Null
}

# Catalog Parts
$Parts = @();
dir "$SourceRoot\Source\Plugins" | Where { $_.PSIsContainer } | ForEach {
    Write-Host "Copying $($_.Name) Plugin"
    $ThisPartRoot = Join-Path $PartsRoot $_.Name
    if(!(Test-Path $ThisPartRoot)) {
        mkdir $ThisPartRoot | Out-Null
    }

    Get-Outputs "$SourceRoot\Source\Plugins\$($_.Name)" | ForEach {
        Copy-Item $_.FullName (Join-Path $ThisPartRoot $_.Name)
    }
}