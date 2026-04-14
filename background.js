// background.js - Sets default settings on first install

// Per-tab enabled state (in-memory; never persisted — tab-local only)
const tabEnabled = new Map();

browser.runtime.onInstalled.addListener((details) => {
  if (details.reason === "install") {
    // "enabled" is intentionally omitted — it is tab-local, not shared
    browser.storage.sync.set({ zoom: 1.5, lensSize: "medium", lensPosition: "right", lensShape: "rect" });
  }
});

// Clean up when a tab is closed
browser.tabs.onRemoved.addListener((tabId) => {
  tabEnabled.delete(tabId);
});

// Message hub for popup ↔ background ↔ content-script coordination
browser.runtime.onMessage.addListener((msg, sender) => {
  if (!msg) return;

  // Popup requests the enabled state for a specific tab
  if (msg.type === "get-tab-enabled") {
    return Promise.resolve({ enabled: tabEnabled.get(msg.tabId) === true });
  }

  // Popup sets the enabled state for a specific tab and relays to content script
  if (msg.type === "set-tab-enabled") {
    const { tabId, enabled } = msg;
    tabEnabled.set(tabId, enabled);
    browser.tabs
      .sendMessage(tabId, { type: "settings-update", patch: { enabled } })
      .catch(() => {});
    return Promise.resolve({ ok: true });
  }

  // Content script toggled via Ctrl+M — sync background map (no relay needed)
  if (msg.type === "set-enabled" && sender && sender.tab) {
    tabEnabled.set(sender.tab.id, msg.enabled);
    return Promise.resolve({ ok: true });
  }
});
