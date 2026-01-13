# ===================== CHECK ADMIN =====================
if (-not ([Security.Principal.WindowsPrincipal] `
    [Security.Principal.WindowsIdentity]::GetCurrent()
).IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)) {

    Write-Host "Richiesta privilegi SYSTEM/ADMIN..." -ForegroundColor Red
    Start-Process powershell.exe `
        "-NoProfile -ExecutionPolicy Bypass -File `"$PSCommandPath`"" `
        -Verb RunAs
    exit
}

# ===================== INIT =====================
$ErrorActionPreference = "SilentlyContinue"
Clear-Host
$host.UI.RawUI.WindowTitle = "FEDERAZIONE GOD MODE - LIVE FORENSICS"

$timestamp = Get-Date -Format 'yyyyMMdd_HHmmss'
$evidencePath = "$env:USERPROFILE\Desktop\FEDERAZIONE_EVIDENCE_$timestamp"
New-Item -ItemType Directory -Path $evidencePath -Force | Out-Null
$logFile = "$evidencePath\FULL_REPORT.txt"

# ===================== LOGGER =====================
function Log-Write {
    param (
        [string]$Msg,
        [string]$Color = "White",
        [bool]$Header = $false
    )

    if ($Header) {
        Write-Host "`n========================================================" -ForegroundColor Cyan
        Write-Host " $Msg" -ForegroundColor Cyan
        Write-Host "========================================================" -ForegroundColor Cyan
        "========================================================`n $Msg`n========================================================" | Out-File $logFile -Append
    } else {
        Write-Host $Msg -ForegroundColor $Color
        $Msg | Out-File $logFile -Append
    }
}

# ===================== MOD DUMP =====================
function Dump-ModContent {
    param ($jarPath, $jarName)

    Log-Write "[DUMP] Estrazione codice sorgente/stringhe per: $jarName" Yellow

    $dumpFile = "$evidencePath\$jarName.DUMP.txt"
    $tempExtract = "$env:TEMP\FedAnalyzer_$((Get-Random))"

    try {
        Add-Type -AssemblyName System.IO.Compression.FileSystem
        [System.IO.Compression.ZipFile]::ExtractToDirectory($jarPath, $tempExtract)

        "--- DUMP ANALISI PER $jarName ---" | Out-File $dumpFile
        "--- HASH: $((Get-FileHash $jarPath).Hash) ---`n" | Out-File $dumpFile -Append

        Get-ChildItem $tempExtract -Recurse -Include *.class,*.yml,*.json,*.txt | ForEach-Object {
            $bytes = Get-Content $_.FullName -Encoding Byte -ReadCount 0
            $text = ($bytes | Where-Object { $_ -ge 32 -and $_ -le 126 } | ForEach-Object { [char]$_ }) -join ""

            $keywords = $text | Select-String "[a-zA-Z0-9_]{4,}" -AllMatches |
                Select-Object -ExpandProperty Matches |
                Select-Object -ExpandProperty Value

            if ($keywords) {
                "`n[FILE: $($_.Name)]" | Out-File $dumpFile -Append
                ($keywords -join " ") | Out-File $dumpFile -Append
            }
        }

        Log-Write "Dump salvato: $dumpFile" Green
    }
    catch {
        Log-Write "Errore dump: $($_.Exception.Message)" Red
    }
    finally {
        Remove-Item $tempExtract -Recurse -Force -ErrorAction SilentlyContinue
    }
}

# ===================== INFO =====================
Log-Write "FEDERAZIONE GOD MODE - AVVIATO" -Header $true
Log-Write "Target User: $env:USERNAME"
Log-Write "OS Version: $((Get-CimInstance Win32_OperatingSystem).Caption)"
Log-Write "Analisi salvata in: $evidencePath" Magenta

# ===================== BAM =====================
Log-Write "ANALISI BAM/DAM" -Header $true
$bamPath = "HKLM:\SYSTEM\CurrentControlSet\Services\bam\State\UserSettings"

if (Test-Path $bamPath) {
    Get-ChildItem $bamPath | ForEach-Object {
        (Get-ItemProperty $_.PSPath).PSObject.Properties.Name |
        Where-Object { $_ -match "\.exe|javaw" } |
        Where-Object { $_ -match "Clicker|Vape|AnyDesk|ProcessHacker|Echo|drip" } |
        ForEach-Object { Log-Write "[CRITICO BAM] $_" Red }
    }
}

# ===================== DRIVERS =====================
Log-Write "ANALISI DRIVER" -Header $true
try {
    Get-WinEvent -FilterHashtable @{LogName='System'; ID=7045} -MaxEvents 50 |
    Where-Object { $_.Message -match "mhyprot|VBox|kprocesshacker|Echo" } |
    ForEach-Object { Log-Write "[CRITICO DRIVER] $($_.Message)" Red }
}
catch {
    Log-Write "Errore lettura Event Log" Red
}

# ===================== USB =====================
Log-Write "ANALISI USB" -Header $true
Get-ItemProperty "HKLM:\SYSTEM\CurrentControlSet\Enum\USBSTOR\*" |
Where-Object FriendlyName |
ForEach-Object { Log-Write "USB: $($_.FriendlyName)" DarkGray }

# ===================== MRU =====================
Log-Write "ANALISI MRU" -Header $true
$mru = "HKCU:\Software\Microsoft\Windows\CurrentVersion\Explorer\ComDlg32\OpenSavePidlMRU"
if (Test-Path $mru) {
    Get-ChildItem $mru | Where-Object { $_.PSChildName -match "exe|jar|dll" } |
    ForEach-Object { Log-Write "Estensione: .$($_.PSChildName)" Yellow }
}

# ===================== MODS =====================
Log-Write "ANALISI MODS" -Header $true
$modsDir = "$env:APPDATA\.minecraft\mods"
$input = Read-Host "Path mods (INVIO default)"
if ($input) { $modsDir = $input }

if (Test-Path $modsDir) {
    $cheats = "AimAssist","AutoClicker","KillAura","Reach","Velocity","Hitboxes","Wurst","Vape","Meteor","Aristois"

    Get-ChildItem $modsDir -Filter *.jar | ForEach-Object {
        $raw = Get-Content $_.FullName -Raw
        if ($cheats | Where-Object { $raw -match $_ }) {
            Log-Write "[CHEAT] $($_.Name)" Red
            Dump-ModContent $_.FullName $_.Name
        }
    }
}

# ===================== USN =====================
Log-Write "ANALISI USN" -Header $true
$usn = "$env:TEMP\usn.txt"
fsutil usn readjournal C: csv | Select-Object -Last 3000 > $usn
Get-Content $usn | Where-Object { $_ -match "FileDelete|FileCreate" -and $_ -match "exe|jar|Vape|Clicker" } |
ForEach-Object { Log-Write "[USN] $_" Red }
Remove-Item $usn -Force

# ===================== RECYCLE BIN =====================
Log-Write "ANALISI CESTINO" -Header $true
$shell = New-Object -ComObject Shell.Application
$shell.NameSpace(0xA).Items() |
Where-Object { $_.Name -match "\.exe|\.jar|\.dll" } |
ForEach-Object { Log-Write "[CESTINO] $($_.Name)" Red }

# ===================== END =====================
Log-Write "ANALISI COMPLETATA" -Header $true
Invoke-Item $evidencePath
Read-Host "Premi INVIO per terminare"

