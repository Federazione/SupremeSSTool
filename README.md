# Supreme ScreenShare Pro

**Supreme ScreenShare Pro** is an advanced, automated cheat detection tool designed specifically for Minecraft screensharing sessions. It provides a comprehensive suite of scanners to detect hidden mods, injected DLLs, suspicious processes, and registry traces associated with known cheat clients.

## 🚀 Key Features

*   **Memory Scanning**: Scans system memory (RAM) for strings associated with known cheats (e.g., Vape, generic cheat strings).
*   **Process Analysis**: Analyzes running processes for suspicious names, paths, and internal strings (including specific checks for `javaw.exe` and other executables).
*   **Injection Detection**: Detects unsigned or suspicious DLLs injected into processes.
*   **Registry Analysis**:
    *   **BAM (Background Activity Moderator)**: Checks for evidence of recently executed cheat programs.
    *   **UserAssist**: Decodes ROT13-encrypted registry entries to find execution traces.
*   **File System Scanning**:
    *   **Mod Folder Scan**: Checks `.minecraft/mods` for known illegal modifications.
    *   **Recycle Bin**: Scans deleted files for cheat remnants.
    *   **Downloads Folder**: Checks for recently downloaded cheat clients.
    *   **USB/External Drive**: Scans connected removable drives for suspicious files.
*   **Deep String Inspection**: Recursively scans executables and process memory for hidden cheat signatures.
*   **Self-Destruct**: Includes a feature to safely remove the tool and its traces after use.
*   **Professional Reporting**: Generates a detailed, styled HTML report of all findings.

## 📋 Requirements

*   **Operating System**: Windows 10 / 11 (x86/x64)
*   **Permissions**: Must be run as **Administrator** to access system memory and registry.
*   **Prerequisites**: Minecraft (`javaw.exe` or `java.exe`) should be running for optimal process-specific scanning (though the tool can run without it).

## 📥 Installation

Supreme ScreenShare Pro is a portable application. No installation is required.

1.  Download the latest release (`SupremeScreenSharePro.exe`).
2.  Place it on the Desktop or a USB drive.

## 🛠️ Usage

1.  **Run as Administrator**: Right-click `SupremeScreenSharePro.exe` and select "Run as administrator".
2.  **Select Scan Modules**:
    *   Click individual buttons (e.g., "Memory Scan", "Process Scan") to run specific checks.
    *   Click **ULTRA SCAN** to run all modules sequentially for a full system audit.
3.  **Review Results**:
    *   Real-time logs will appear in the application window.
    *   A "Red Flags" counter tracks detected threats.
4.  **Generate Report**:
    *   After a scan, click **Generate Report** (or wait for Ultra Scan to finish) to open a detailed HTML report in your browser.
5.  **Cleanup**:
    *   Use the **Self Destruct** button to close the application and delete the executable automatically.

---
*Built with .NET 6 & WPF*
