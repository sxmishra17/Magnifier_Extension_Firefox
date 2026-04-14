# Magnifier — Firefox Extension

Hover over any content on a web page to instantly magnify it in a floating lens overlay. Works on text, images, and PDFs — no configuration required.

---

## Features

| Feature | Details |
|---|---|
| **Text magnification** | Reads font, weight, colour, and background from the hovered element and renders it enlarged inside the lens |
| **Image magnification** | Zooms into `<img>` elements and CSS background images with pixel-accurate centering |
| **PDF magnification** | Built-in PDF viewer renders PDF pages to canvas and magnifies them without CORS restrictions |
| **Circle & Rectangle lens** | Switch between a circular loupe and a wide rectangular banner lens |
| **Zoom 1× – 10×** | Smooth 0.5-step slider |
| **Three lens sizes** | Small / Medium / Large |
| **Five cursor positions** | Left · Right · Above · Below · Centered |
| **Per-tab enable/disable** | Magnifier state is local to each tab; other tabs are never affected |
| **Keyboard shortcuts** | `Ctrl+M` toggles the magnifier; `Escape` turns it off |
| **Settings sync** | Zoom, size, shape, and position are synced across devices via Firefox Sync |

---

## Installation

### From Firefox Add-ons (AMO)
*(Link will appear here after the extension is published.)*

### Manual / Developer Installation
1. Clone or download this repository.
2. Open Firefox and navigate to `about:debugging`.
3. Click **This Firefox** → **Load Temporary Add-on**.
4. Select the `manifest.json` file.

---

## Usage

1. Click the **Magnifier** icon in the toolbar to open the settings popup.
2. Toggle the switch **ON** to activate the magnifier for the current tab.
3. Move the mouse over any text, image, or element — the lens appears automatically.
4. Adjust **Zoom Level**, **Lens Size**, **Lens Shape**, and **Position** in the popup; changes apply instantly.

### PDF Viewer
Click **Open PDF Viewer** in the popup to open the built-in viewer. Drop any PDF file in, enable the magnifier, then hover over the rendered pages.

---

## Permissions

| Permission | Reason |
|---|---|
| `storage` | Saves zoom, size, shape, and position settings via Firefox Sync |
| `tabs` | Lets the background script relay enable/disable state to specific tabs |
| `activeTab` | Reads the active tab when the popup is open |

---

## File Structure

```
manifest.json          — Extension manifest (MV2)
background.js          — Background service: tracks per-tab state, relays messages
content.js             — Injected into every page: builds lens, handles magnification
content.css            — Lens overlay styles
icons/
  icon-48.png
  icon-96.png
popup/
  popup.html           — Settings panel markup
  popup.css            — Settings panel styles
  popup.js             — Settings panel logic
pdf-viewer/
  viewer.html          — Built-in PDF viewer page
  viewer.css           — PDF viewer styles
  viewer.js            — PDF.js integration (renders pages to canvas)
```

---

## Architecture

```
Popup ──(storage.sync.set + sendMessage)──► Content Script
  │                                              │
  └──(runtime.sendMessage)──► Background ───────┘
                                   │
                             tabEnabled Map
                             (in-memory, per-tab)
```

- **Settings** (zoom, size, shape, position) are shared via `browser.storage.sync` and reflected to content scripts through both storage change events and direct messages.
- **Enabled state** is tab-local and never persisted; it lives only in the background script's `Map` and is relayed to each tab's content script via `sendMessage`.

---

## Browser Compatibility

| Browser | Support |
|---|---|
| Firefox 109+ | ✅ Full support (Manifest V2) |
| Firefox for Android | ✅ (popup layout adapts) |
| Chrome / Edge | ❌ Uses `browser.*` API (not `chrome.*`) |

---

## Version History

| Version | Changes |
|---|---|
| 1.1.0 | Fixed image magnification centering in rectangle lens mode; refactored shared dimension logic; tightened Content Security Policy |
| 1.0.0 | Initial release |

---

## License

MIT © Magnifier contributors
