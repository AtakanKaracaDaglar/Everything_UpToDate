using System;

namespace Everything_UpToDate.Models
{
    /// <summary>
    /// Güncelleme geçmiþi kayýt modeli
    /// </summary>
    public class UpdateHistoryEntry
    {
        public int Id { get; set; }
        public string AppName { get; set; }
        public string AppId { get; set; }
        public string FromVersion { get; set; }
        public string ToVersion { get; set; }
        public DateTime UpdateDate { get; set; }
        public bool Success { get; set; }
        public long UpdateSizeBytes { get; set; }
        public string ErrorMessage { get; set; }
        public int DurationSeconds { get; set; }

        public UpdateHistoryEntry()
        {
            UpdateDate = DateTime.Now;
            Success = true;
            ErrorMessage = string.Empty;
        }

        /// <summary>
        /// Formatlanmýþ boyut
        /// </summary>
        public string GetFormattedSize()
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

        /// <summary>
        /// Formatlanmýþ süre
        /// </summary>
        public string GetFormattedDuration()
        {
            if (DurationSeconds < 60)
                return $"{DurationSeconds} saniye";
            else if (DurationSeconds < 3600)
                return $"{DurationSeconds / 60} dakika {DurationSeconds % 60} saniye";
            else
                return $"{DurationSeconds / 3600} saat {(DurationSeconds % 3600) / 60} dakika";
        }

        /// <summary>
        /// CSV formatýnda satýr
        /// </summary>
        public string ToCsvLine()
        {
            return $"{Id},{AppName},{AppId},{FromVersion},{ToVersion}," +
                   $"{UpdateDate:yyyy-MM-dd HH:mm:ss},{Success},{UpdateSizeBytes}," +
                   $"{DurationSeconds},\"{ErrorMessage}\"";
        }

        /// <summary>
        /// CSV satýrýndan parse
        /// </summary>
        public static UpdateHistoryEntry FromCsvLine(string line)
        {
            try
            {
                var parts = line.Split(',');
                if (parts.Length < 9)
                    return null;

                return new UpdateHistoryEntry
                {
                    Id = int.Parse(parts[0]),
                    AppName = parts[1],
                    AppId = parts[2],
                    FromVersion = parts[3],
                    ToVersion = parts[4],
                    UpdateDate = DateTime.Parse(parts[5]),
                    Success = bool.Parse(parts[6]),
                    UpdateSizeBytes = long.Parse(parts[7]),
                    DurationSeconds = int.Parse(parts[8]),
                    ErrorMessage = parts.Length > 9 ? parts[9].Trim('"') : string.Empty
                };
            }
            catch
            {
                return null;
            }
        }
    }
}
