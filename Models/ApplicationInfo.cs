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
        public DateTime LastChecked { get; set; }
        public bool IsUpdateAvailable => CurrentVersion != LatestVersion;
        public UpdateStatus Status { get; set; }
        public string InstallPath { get; set; }
        public long UpdateSizeBytes { get; set; }

        public ApplicationInfo()
        {
            Status = UpdateStatus.Idle;
            LastChecked = DateTime.Now;
        }

        public string GetUpdateSizeFormatted()
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
