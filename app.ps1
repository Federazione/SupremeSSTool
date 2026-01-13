# Nasconde la console
Add-Type -Name Window -Namespace Console -MemberDefinition '
[DllImport("kernel32.dll")] public static extern IntPtr GetConsoleWindow();
[DllImport("user32.dll")] public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
'
[Console.Window]::ShowWindow([Console.Window]::GetConsoleWindow(), 0)

Add-Type -AssemblyName PresentationFramework

$base = "https://raw.githubusercontent.com/Federazione/FederazioneModAnalyzer/main"
$xaml = irm "$base/ui.xaml"
$reader = New-Object System.Xml.XmlNodeReader $xaml
$Window = [Windows.Markup.XamlReader]::Load($reader)

. "$PSScriptRoot/core/helpers.ps1"

$Log = $Window.FindName("LogBox")

function Log($msg) {
    $Log.AppendText("$msg`n")
    $Log.ScrollToEnd()
}

$Window.FindName("BtnSystem").Add_Click({
    Log "== SYSTEM INFO =="
    . "$PSScriptRoot/core/system.ps1"
})

$Window.FindName("BtnBam").Add_Click({
    Log "== BAM EXECUTIONS =="
    . "$PSScriptRoot/core/bam.ps1"
})

$Window.FindName("BtnUSB").Add_Click({
    Log "== USB HISTORY =="
    . "$PSScriptRoot/core/usb.ps1"
})

$Window.FindName("BtnMods").Add_Click({
    Log "== MODS SCAN =="
    . "$PSScriptRoot/core/mods.ps1"
})

$Window.FindName("BtnRecycle").Add_Click({
    Log "== RECYCLE BIN =="
    . "$PSScriptRoot/core/recycle.ps1"
})

$Window.ShowDialog() | Out-Null
