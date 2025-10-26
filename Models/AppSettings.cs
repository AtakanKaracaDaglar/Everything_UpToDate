using System;

namespace Everything_UpToDate.Models
{
    /// <summary>
    /// Uygulama ayarlarý modeli
    /// </summary>
    public class AppSettings
    {
        public string Theme { get; set; } = "Light"; // "Light" veya "Dark"
        public bool AutoScanEnabled { get; set; } = false;
        public int AutoScanIntervalHours { get; set; } = 6;
        public bool ShowNotifications { get; set; } = true;
        public bool MinimizeToTray { get; set; } = true;
        public bool StartMinimized { get; set; } = false;
        public bool StartWithWindows { get; set; } = false;
        public DateTime LastScanTime { get; set; } = DateTime.MinValue;
        public string[] ExcludedApps { get; set; } = new string[0];
    }
}
