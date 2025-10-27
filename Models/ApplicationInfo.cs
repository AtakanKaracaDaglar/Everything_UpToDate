using System;

namespace Everything_UpToDate.Models
{
    /// <summary>
    /// Uygulama bilgilerini tutan model sýnýfý
    /// </summary>
    public class ApplicationInfo
    {
        public string Name { get; set; }
        public string Id { get; set; } // WinGet ID'si
        public string CurrentVersion { get; set; }
        public string LatestVersion { get; set; }
        public string Description { get; set; }
        public string InstallPath { get; set; }
        public long UpdateSizeBytes { get; set; }
        public UpdateStatus Status { get; set; }
        public DateTime LastChecked { get; set; } = DateTime.Now; // YENÝ
        
        // YENÝ: Paket kaynaðý bilgisi
        public PackageSource Source { get; set; }
        
        // YENÝ: Kaynak adý
        public string SourceName
        {
            get
            {
                switch (Source)
                {
                    case PackageSource.WinGet:
                        return "WinGet";
                    case PackageSource.Chocolatey:
                        return "Chocolatey";
                    case PackageSource.MicrosoftStore:
                        return "Microsoft Store";
                    case PackageSource.Steam:
                        return "Steam";
                    default:
                        return "Unknown";
                }
            }
        }

        /// <summary>
        /// Güncelleme mevcut mu kontrol eder
        /// </summary>
        public bool IsUpdateAvailable
        {
            get
            {
                return !string.IsNullOrEmpty(LatestVersion) &&
                       CurrentVersion != LatestVersion &&
                       LatestVersion != "N/A";
            }
        }

        public ApplicationInfo()
        {
            Status = UpdateStatus.Idle;
            LastChecked = DateTime.Now;
        }

        public string FormattedSize
        {
            get
            {
                if (UpdateSizeBytes < 1024)
                    return $"{UpdateSizeBytes} B";
                else if (UpdateSizeBytes < 1024 * 1024)
                    return $"{UpdateSizeBytes / 1024.0:F2} KB";
                else if (UpdateSizeBytes < 1024 * 1024 * 1024)
                    return $"{UpdateSizeBytes / (1024.0 * 1024.0):F2} MB";
                else
                    return $"{UpdateSizeBytes / (1024.0 * 1024.0 * 1024.0):F2} GB";
            }
        }

        // YENÝ: Form1.cs'de kullanýlan metod
        public string GetUpdateSizeFormatted()
        {
            return FormattedSize;
        }
    }

    /// <summary>
    /// Güncelleme durumu enum
    /// </summary>
    public enum UpdateStatus
    {
        Idle,
        Checking,
        Downloading,
        Installing,
        Completed,
        Failed,
        UpToDate
    }
}
