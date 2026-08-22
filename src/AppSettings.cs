using System;
using System.IO;
using System.Windows.Forms;

namespace MagnifierApp
{
    public class AppSettings
    {
        public bool Enabled { get; set; }
        public float Zoom { get; set; }
        public string LensSize { get; set; }      // "small", "medium", "large"
        public string LensShape { get; set; }     // "circle", "rect"
        public string LensPosition { get; set; }  // "up", "down", "left", "right", "center"
        public uint HotkeyModifiers { get; set; } // 2 = MOD_CONTROL
        public Keys HotkeyKey { get; set; }       // Keys.M
        public string HotkeyDisplay { get; set; } // "Ctrl + M"
        public string Language { get; set; }      // "auto", "en", "es", "fr", "de", "ja", "zh", "hi", etc.

        public AppSettings()
        {
            // Default settings matching Firefox extension
            Enabled = false;
            Zoom = 3.0f;
            LensSize = "medium";
            LensShape = "rect";
            LensPosition = "right";
            HotkeyModifiers = 2; // MOD_CONTROL
            HotkeyKey = Keys.M;
            HotkeyDisplay = "Ctrl + M";
            Language = "auto";
        }

        private static string GetConfigPath()
        {
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string dir = Path.Combine(appData, "MagnifierWindowsApp");
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            return Path.Combine(dir, "settings.ini");
        }

        public void Save()
        {
            try
            {
                string path = GetConfigPath();
                using (StreamWriter sw = new StreamWriter(path, false))
                {
                    sw.WriteLine(string.Format("Zoom={0}", Zoom));
                    sw.WriteLine(string.Format("LensSize={0}", LensSize));
                    sw.WriteLine(string.Format("LensShape={0}", LensShape));
                    sw.WriteLine(string.Format("LensPosition={0}", LensPosition));
                    sw.WriteLine(string.Format("HotkeyModifiers={0}", HotkeyModifiers));
                    sw.WriteLine(string.Format("HotkeyKey={0}", (int)HotkeyKey));
                    sw.WriteLine(string.Format("HotkeyDisplay={0}", HotkeyDisplay));
                    sw.WriteLine(string.Format("Language={0}", Language));
                }
            }
            catch { }
        }

        public static AppSettings Load()
        {
            AppSettings settings = new AppSettings();
            try
            {
                string path = GetConfigPath();
                if (File.Exists(path))
                {
                    string[] lines = File.ReadAllLines(path);
                    foreach (string line in lines)
                    {
                        if (string.IsNullOrEmpty(line) || !line.Contains("=")) continue;
                        string[] parts = line.Split(new char[] { '=' }, 2);
                        string key = parts[0].Trim();
                        string val = parts[1].Trim();

                        if (key.Equals("Zoom", StringComparison.OrdinalIgnoreCase))
                        {
                            float z;
                            if (float.TryParse(val, out z)) settings.Zoom = Math.Max(1.0f, Math.Min(10.0f, z));
                        }
                        else if (key.Equals("LensSize", StringComparison.OrdinalIgnoreCase))
                        {
                            if (val == "small" || val == "medium" || val == "large") settings.LensSize = val;
                        }
                        else if (key.Equals("LensShape", StringComparison.OrdinalIgnoreCase))
                        {
                            if (val == "circle" || val == "rect") settings.LensShape = val;
                        }
                        else if (key.Equals("LensPosition", StringComparison.OrdinalIgnoreCase))
                        {
                            if (val == "up" || val == "down" || val == "left" || val == "right" || val == "center")
                                settings.LensPosition = val;
                        }
                        else if (key.Equals("HotkeyModifiers", StringComparison.OrdinalIgnoreCase))
                        {
                            uint mods;
                            if (uint.TryParse(val, out mods)) settings.HotkeyModifiers = mods;
                        }
                        else if (key.Equals("HotkeyKey", StringComparison.OrdinalIgnoreCase))
                        {
                            int k;
                            if (int.TryParse(val, out k)) settings.HotkeyKey = (Keys)k;
                        }
                        else if (key.Equals("HotkeyDisplay", StringComparison.OrdinalIgnoreCase))
                        {
                            if (!string.IsNullOrEmpty(val)) settings.HotkeyDisplay = val;
                        }
                        else if (key.Equals("Language", StringComparison.OrdinalIgnoreCase))
                        {
                            if (!string.IsNullOrEmpty(val)) settings.Language = val;
                        }
                    }
                }
            }
            catch { }
            return settings;
        }
    }
}
