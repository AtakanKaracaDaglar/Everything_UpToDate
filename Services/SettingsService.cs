using System;
using System.IO;
using System.Text;
using Everything_UpToDate.Models;

namespace Everything_UpToDate.Services
{
    /// <summary>
    /// Ayarlarý yöneten servis (.ini dosyasý kullanarak)
    /// </summary>
    public class SettingsService
    {
        private readonly string _settingsPath;
        private AppSettings _settings;

        public SettingsService()
        {
            string appDataPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "EverythingUpToDate"
            );

            if (!Directory.Exists(appDataPath))
            {
                Directory.CreateDirectory(appDataPath);
            }

            _settingsPath = Path.Combine(appDataPath, "settings.ini");
            _settings = LoadSettings();
        }

        public AppSettings Settings
        {
            get { return _settings; }
        }

        /// <summary>
        /// Ayarlarý dosyadan yükle
        /// </summary>
        private AppSettings LoadSettings()
        {
            var settings = new AppSettings();

            try
            {
                if (!File.Exists(_settingsPath))
                {
                    // Ýlk kez çalýþýyorsa varsayýlan ayarlarý kaydet
                    SaveSettings(settings);
                    return settings;
                }

                var lines = File.ReadAllLines(_settingsPath, Encoding.UTF8);
                foreach (var line in lines)
                {
                    if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#"))
                        continue;

                    var parts = line.Split('=');
                    if (parts.Length != 2)
                        continue;

                    string key = parts[0].Trim();
                    string value = parts[1].Trim();

                    switch (key)
                    {
                        case "Theme":
                            settings.Theme = value;
                            break;
                        case "AutoScanEnabled":
                            settings.AutoScanEnabled = bool.Parse(value);
                            break;
                        case "AutoScanIntervalHours":
                            settings.AutoScanIntervalHours = int.Parse(value);
                            break;
                        case "ShowNotifications":
                            settings.ShowNotifications = bool.Parse(value);
                            break;
                        case "MinimizeToTray":
                            settings.MinimizeToTray = bool.Parse(value);
                            break;
                        case "StartMinimized":
                            settings.StartMinimized = bool.Parse(value);
                            break;
                        case "StartWithWindows":
                            settings.StartWithWindows = bool.Parse(value);
                            break;
                        case "LastScanTime":
                            settings.LastScanTime = DateTime.Parse(value);
                            break;
                        case "ExcludedApps":
                            settings.ExcludedApps = value.Split(';');
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Settings load error: {ex.Message}");
            }

            return settings;
        }

        /// <summary>
        /// Ayarlarý dosyaya kaydet
        /// </summary>
        public void SaveSettings(AppSettings settings = null)
        {
            if (settings != null)
            {
                _settings = settings;
            }

            try
            {
                var sb = new StringBuilder();
                sb.AppendLine("# Everything UpToDate Settings");
                sb.AppendLine($"# Last Updated: {DateTime.Now}");
                sb.AppendLine();
                sb.AppendLine($"Theme={_settings.Theme}");
                sb.AppendLine($"AutoScanEnabled={_settings.AutoScanEnabled}");
                sb.AppendLine($"AutoScanIntervalHours={_settings.AutoScanIntervalHours}");
                sb.AppendLine($"ShowNotifications={_settings.ShowNotifications}");
                sb.AppendLine($"MinimizeToTray={_settings.MinimizeToTray}");
                sb.AppendLine($"StartMinimized={_settings.StartMinimized}");
                sb.AppendLine($"StartWithWindows={_settings.StartWithWindows}");
                sb.AppendLine($"LastScanTime={_settings.LastScanTime}");
                sb.AppendLine($"ExcludedApps={string.Join(";", _settings.ExcludedApps)}");

                File.WriteAllText(_settingsPath, sb.ToString(), Encoding.UTF8);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Settings save error: {ex.Message}");
            }
        }

        /// <summary>
        /// Belirli bir ayarý güncelle ve kaydet
        /// </summary>
        public void UpdateSetting(string key, object value)
        {
            switch (key)
            {
                case "Theme":
                    _settings.Theme = value.ToString();
                    break;
                case "AutoScanEnabled":
                    _settings.AutoScanEnabled = (bool)value;
                    break;
                case "AutoScanIntervalHours":
                    _settings.AutoScanIntervalHours = (int)value;
                    break;
                case "ShowNotifications":
                    _settings.ShowNotifications = (bool)value;
                    break;
                case "MinimizeToTray":
                    _settings.MinimizeToTray = (bool)value;
                    break;
                case "StartMinimized":
                    _settings.StartMinimized = (bool)value;
                    break;
                case "StartWithWindows":
                    _settings.StartWithWindows = (bool)value;
                    break;
                case "LastScanTime":
                    _settings.LastScanTime = (DateTime)value;
                    break;
            }

            SaveSettings();
        }

        /// <summary>
        /// Hariç tutma listesine uygulama ekle
        /// </summary>
        public void AddExcludedApp(string appId)
        {
            var list = new System.Collections.Generic.List<string>(_settings.ExcludedApps);
            if (!list.Contains(appId))
            {
                list.Add(appId);
                _settings.ExcludedApps = list.ToArray();
                SaveSettings();
            }
        }

        /// <summary>
        /// Hariç tutma listesinden uygulama çýkar
        /// </summary>
        public void RemoveExcludedApp(string appId)
        {
            var list = new System.Collections.Generic.List<string>(_settings.ExcludedApps);
            if (list.Contains(appId))
            {
                list.Remove(appId);
                _settings.ExcludedApps = list.ToArray();
                SaveSettings();
            }
        }

        /// <summary>
        /// Uygulama hariç tutulmuþ mu kontrol et
        /// </summary>
        public bool IsAppExcluded(string appId)
        {
            return Array.IndexOf(_settings.ExcludedApps, appId) >= 0;
        }
    }
}
