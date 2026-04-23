# Magnifier — Firefox Extension

> Hover over any text, image, or PDF to magnify it with a customizable lens. Choose circle or rectangle shape, zoom 1×–10×, three lens sizes, and five cursor positions.

---

## Features

- 🔍 **Text Magnification** — Hover over any text on a webpage to see it enlarged in a floating lens
- 🖼️ **Image Magnification** — Works on images and inline graphics
- 📄 **PDF Support** — Built-in PDF viewer with magnifier support for local PDF files
- ⭕ **Circle or Rectangle** — Choose your preferred lens shape
- 🔎 **Zoom 1×–10×** — Fine-grained zoom control from 1× up to 10×
- 📐 **Three Lens Sizes** — Small, medium, and large lens options
- 🖱️ **Five Cursor Positions** — Position the lens relative to your cursor
- 💾 **Persistent Settings** — All preferences are saved across sessions
- 🔘 **On/Off Toggle** — Enable or disable the magnifier with one click

---

## Installation

### From Firefox Add-ons (AMO)
Search **"Magnifier"** on [addons.mozilla.org](https://addons.mozilla.org)

### Developer Install
1. Open `about:debugging#/runtime/this-firefox`
2. Click **Load Temporary Add-on**
3. Select `manifest.json` from this folder

---

## How to Use

1. Click the **Magnifier** icon in the Firefox toolbar to open settings
2. Toggle the magnifier **ON**
3. Choose your preferred **zoom level**, **lens shape**, and **size**
4. Hover over any text or image on the page — a magnified view appears
5. For PDFs, use the built-in viewer for best results

---

## Project Structure

```
├── manifest.json          # MV2 manifest
├── background.js          # Background script — state management
├── content.js             # Content script — lens rendering and mouse tracking
├── content.css            # Lens styles and animations
├── popup/
│   ├── popup.html         # Settings popup UI
│   ├── popup.css          # Popup styles
│   └── popup.js           # Settings controls
├── pdf-viewer/
│   ├── viewer.html        # Built-in PDF viewer
│   ├── viewer.css         # PDF viewer styles
│   ├── viewer.js          # PDF viewer logic with magnifier integration
│   └── lib/
│       ├── pdf.min.js     # PDF.js library
│       └── pdf.worker.min.js
└── icons/                 # Extension icons (48, 96px)
```

---

## Third-Party Libraries

| Library | License | Purpose |
|---------|---------|---------|
| [PDF.js](https://github.com/mozilla/pdf.js) | Apache 2.0 | PDF rendering in the built-in viewer |

---

## Privacy

**No data leaves your device.** Settings are stored locally using `browser.storage.local`. No external network requests are made.

---

## Developer

**YuvaTech**

---

## License

All Rights Reserved.
