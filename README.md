# Magnifier — Firefox Extension & Windows Desktop Application v1.3.0

![Version](https://img.shields.io/badge/version-1.3.0-blue.svg)
![Platform](https://img.shields.io/badge/platform-Firefox%20%7C%20Windows-brightgreen.svg)
![Developer](https://img.shields.io/badge/developer-Yuvatech%20Solution%20USA%2C%20LLC-orange.svg)

> Hover over any text, image, screen region, or PDF to magnify it with a customizable lens. Includes both a **Firefox Web Extension** and a standalone **Windows Desktop Executable (`Magnifier.exe`)**!

---

## 🚀 Quick Download

- 📦 **[Download Windows Desktop App (`Magnifier-Desktop.zip`)](https://raw.githubusercontent.com/sxmishra17/Magnifier_Extension_Firefox/main/Magnifier-Desktop.zip)** — *Contains Magnifier.exe standalone executable*
- 💻 **[Download Direct Executable (`Magnifier.exe`)](https://raw.githubusercontent.com/sxmishra17/Magnifier_Extension_Firefox/main/Magnifier.exe)** — *Click-and-play portable Windows app*
- 🧩 **[Download Firefox Add-on (`magnifier-1.3.0.xpi`)](https://raw.githubusercontent.com/sxmishra17/Magnifier_Extension_Firefox/main/magnifier-1.3.0.xpi)** — *Firefox extension package*

---

## 🌟 Key Features

### 🖥️ Windows Standalone App (`Magnifier.exe`)
- ⚡ **Zero Installation & Click-and-Play** — Lightweight (~100 KB), single-file portable Windows executable.
- 🎯 **Flicker-Free 60 FPS Real-Time Screen Lens** — Powered by GDI+ layered rendering with native `WDA_EXCLUDEFROMCAPTURE` exclusion.
- ⌨️ **System-Wide Global Hotkey (`Ctrl+M`)** — Toggle the lens anywhere across Windows. Includes an interactive Hotkey Recorder dialog.
- 📌 **System Tray Toolbar Integration** — Minimizes cleanly to the system tray taskbar menu.
- 🏢 **About Yuvatech Solution USA, LLC Modal** — Dedicated company info dialog in system tray menu.
- 🌐 **Multi-Language Auto-Detection** — System language default on initial startup.

### 🧩 Firefox Web Extension
- 🔍 **Text & Image Magnification** — Hover over any webpage text or image for real-time magnified preview.
- 📄 **PDF Support** — Built-in localized PDF viewer with magnifier integration.
- 🏢 **Yuvatech Logo & Branding** — Company logo and branding at the bottom of the extension GUI.
- ⭕ **Circle or Rectangle Shapes** — Choose circle or rectangle lens shapes with 3 lens sizes (Small, Medium, Large) and 5 cursor positions.
- 🌐 **10+ Supported Languages** — Auto-detects browser/system language by default.
- 📱 **Anti-Clipping Warm Theme GUI** — Compact dark glassmorphism layout with smooth scrollbar failsafe.

---

## 🌐 Supported Languages

Both the Windows app and the Firefox extension feature complete localization:

| Code | Language | Native Name |
|---|---|---|
| `auto` | Auto-detect (Default) | System / Browser Default |
| `en` | English | English |
| `es` | Spanish | Español |
| `fr` | French | Français |
| `de` | German | Deutsch |
| `ja` | Japanese | 日本語 |
| `zh` | Chinese | 中文 |
| `hi` | Hindi | हिन्दी |
| `pt` | Portuguese | Português |
| `it` | Italian | Italiano |
| `ru` | Russian | Русский |

---

## 📁 Repository Structure

```
├── Magnifier.exe               # Standalone Windows Desktop Executable
├── Magnifier-Desktop.zip       # Windows Desktop Package (Magnifier.exe inside)
├── magnifier-1.3.0.xpi         # Packaged Firefox Extension (v1.3.0)
├── magnifier-1.3.0.zip         # GitHub Release Archive
├── package-extension.bat       # Firefox extension packager script
├── build.bat                   # Windows application C# compiler script
├── manifest.json               # Firefox WebExtension MV2 Manifest
├── background.js               # Background script state management
├── content.js                  # Content script lens rendering
├── content.css                 # Extension lens overlay styles
├── locales.js                  # Multi-language translation engine
├── popup/
│   ├── popup.html              # Settings popup UI with Yuvatech logo & download button
│   ├── popup.css               # Compact warm dark settings layout
│   └── popup.js                # Live popup settings logic & localization
├── pdf-viewer/
│   ├── viewer.html             # Localized PDF viewer
│   ├── viewer.css              # PDF viewer styles
│   └── viewer.js               # PDF rendering with magnifier integration
├── icons/                      # Extension icons & yuvatechlogo.png
└── src/                        # C# Desktop App Source Code
    ├── Program.cs              # Entry point & Tray NotifyIcon
    ├── MagnifierLens.cs        # Layered window GDI+ screen capture lens
    ├── SettingsForm.cs         # Settings panel GUI
    ├── AboutDialog.cs          # Yuvatech Solution USA, LLC company modal
    ├── Localization.cs        # C# Multi-language engine
    └── NativeMethods.cs        # Windows API P/Invoke declarations
```

---

## 👨‍💻 Developer & Company

Developed by **Satish Mishra**  
**Yuvatech Solution USA, LLC**  
All Rights Reserved.
