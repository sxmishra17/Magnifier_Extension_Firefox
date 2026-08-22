# Magnifier — Windows Desktop Application

> A standalone, native Windows screen magnifier application that provides a real-time, customizable floating lens over any app, text, browser, image, or PDF document. Built as a click-and-play single executable (`Magnifier.exe`).

---

## Features

- ⚡ **Click and Play Single Executable (`Magnifier.exe`)** — Zero installation, zero external runtimes needed. Starts instantly on any Windows 10 / 11 PC.
- ⌨️ **System-Wide Global Hotkey (`Ctrl + M` default)** — Start and stop the magnifier anywhere in Windows, even inside full-screen apps, browsers, or games.
- 🔘 **System Tray Toolbar Integration** — Sits quietly in the Windows notification area (system tray). Left-click opens Settings; right-click opens the context menu.
- 🔍 **Real-Time 60 FPS Magnifier Lens** — Transparent, click-through overlay that follows your cursor with zero lag.
- ⭕ **Circle & Rectangle Shapes** — Choose between circular or rounded rectangular lens with green glowing ring (`#3cdc64`) and center crosshairs.
- 🔎 **Zoom 1×–10×** — Fine-grained zoom adjustment in 0.5× increments.
- 📐 **Three Lens Sizes** — Small, Medium, and Large lens presets.
- 🖱️ **Five Cursor Positions** — Position the lens relative to your cursor (`Right`, `Left`, `Up`, `Down`, `Center`) with screen edge clamping.
- ⚙️ **Customizable Global Hotkey** — Change your hotkey shortcut anytime directly inside Settings using an interactive key recorder.
- ⎋ **Quick Escape** — Press `Escape` anytime to immediately hide the magnifier.
- 📄 **Built-in PDF Viewer** — View local PDF files with toolbar controls and live magnifier support.
- 💾 **Persistent Settings** — All your preferences are automatically remembered across sessions.

---

## How to Use

1. **Launch**: Double-click `Magnifier.exe`.
2. **Tray Icon**: The Magnifier icon will appear in your Windows system tray (taskbar notification area).
3. **Toggle Magnifier**:
   - Press **`Ctrl + M`** on your keyboard (or your custom configured hotkey) to toggle ON / OFF.
   - Or right-click the tray icon and click **Toggle Magnifier**.
   - Or open **Settings** and flip the toggle switch.
4. **Settings & Customization**:
   - Right-click or left-click the tray icon and select **Settings...**.
   - Adjust Zoom (1× to 10×), Lens Size, Shape, Position, or Change Hotkey.
5. **PDF Viewer**:
   - Right-click the tray icon and select **PDF Viewer...** (or click "Open PDF Viewer" in Settings).
   - Click **Choose PDF** to load any document.
6. **Exit**: Right-click the tray icon and click **Exit**.

---

## Build from Source

To recompile `Magnifier.exe` at any time:
```cmd
build.bat
```
*(Uses the native Windows C# compiler `csc.exe` built into Windows).*
