using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Everything_UpToDate.Models;

namespace Everything_UpToDate.Services
{
    /// <summary>
    /// Güncelleme geçmiþi veritabaný servisi (CSV tabanlý)
    /// </summary>
    public class DatabaseService
    {
        private readonly string _dbPath;
        private readonly string _dbFile;
        private List<UpdateHistoryEntry> _cache;
        private int _nextId = 1;

        public DatabaseService()
        {
            _dbPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "EverythingUpToDate"
            );

            if (!Directory.Exists(_dbPath))
            {
                Directory.CreateDirectory(_dbPath);
            }

            _dbFile = Path.Combine(_dbPath, "update_history.csv");
            _cache = new List<UpdateHistoryEntry>();
            
            LoadFromFile();
        }

        /// <summary>
        /// Dosyadan geçmiþi yükle
        /// </summary>
        private void LoadFromFile()
        {
            try
            {
                if (!File.Exists(_dbFile))
                {
                    // Ýlk kez çalýþýyorsa baþlýk satýrý oluþtur
                    CreateEmptyDatabase();
                    return;
                }

                var lines = File.ReadAllLines(_dbFile, Encoding.UTF8);
                _cache.Clear();

                // Ýlk satýr baþlýk, atla
                for (int i = 1; i < lines.Length; i++)
                {
                    if (string.IsNullOrWhiteSpace(lines[i]))
                        continue;

                    var entry = UpdateHistoryEntry.FromCsvLine(lines[i]);
                    if (entry != null)
                    {
                        _cache.Add(entry);
                        if (entry.Id >= _nextId)
                            _nextId = entry.Id + 1;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Database load error: {ex.Message}");
            }
        }

        /// <summary>
        /// Boþ veritabaný oluþtur
        /// </summary>
        private void CreateEmptyDatabase()
        {
            try
            {
                var header = "Id,AppName,AppId,FromVersion,ToVersion,UpdateDate,Success,UpdateSizeBytes,DurationSeconds,ErrorMessage\n";
                File.WriteAllText(_dbFile, header, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Database create error: {ex.Message}");
            }
        }

        /// <summary>
        /// Dosyaya kaydet
        /// </summary>
        private void SaveToFile()
        {
            try
            {
                var sb = new StringBuilder();
                sb.AppendLine("Id,AppName,AppId,FromVersion,ToVersion,UpdateDate,Success,UpdateSizeBytes,DurationSeconds,ErrorMessage");

                foreach (var entry in _cache.OrderBy(e => e.UpdateDate))
                {
                    sb.AppendLine(entry.ToCsvLine());
                }

                File.WriteAllText(_dbFile, sb.ToString(), Encoding.UTF8);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Database save error: {ex.Message}");
            }
        }

        /// <summary>
        /// Yeni güncelleme kaydý ekle
        /// </summary>
        public void AddUpdateHistory(UpdateHistoryEntry entry)
        {
            entry.Id = _nextId++;
            _cache.Add(entry);
            SaveToFile();
        }

        /// <summary>
        /// Tüm geçmiþi getir
        /// </summary>
        public List<UpdateHistoryEntry> GetAllHistory()
        {
            return new List<UpdateHistoryEntry>(_cache);
        }

        /// <summary>
        /// Belirli bir uygulamanýn geçmiþini getir
        /// </summary>
        public List<UpdateHistoryEntry> GetHistoryByApp(string appId)
        {
            return _cache.Where(e => e.AppId == appId).OrderByDescending(e => e.UpdateDate).ToList();
        }

        /// <summary>
        /// Tarih aralýðýna göre geçmiþ getir
        /// </summary>
        public List<UpdateHistoryEntry> GetHistoryByDateRange(DateTime from, DateTime to)
        {
            return _cache.Where(e => e.UpdateDate >= from && e.UpdateDate <= to)
                        .OrderByDescending(e => e.UpdateDate)
                        .ToList();
        }

        /// <summary>
        /// Son N güncellemeleri getir
        /// </summary>
        public List<UpdateHistoryEntry> GetRecentHistory(int count)
        {
            return _cache.OrderByDescending(e => e.UpdateDate).Take(count).ToList();
        }

        /// <summary>
        /// Ýstatistikler
        /// </summary>
        public (int TotalUpdates, int SuccessfulUpdates, int FailedUpdates, long TotalBytes) GetStatistics()
        {
            int total = _cache.Count;
            int successful = _cache.Count(e => e.Success);
            int failed = _cache.Count(e => !e.Success);
            long totalBytes = _cache.Sum(e => e.UpdateSizeBytes);

            return (total, successful, failed, totalBytes);
        }

        /// <summary>
        /// En çok güncellenen uygulamalar
        /// </summary>
        public List<(string AppName, int Count)> GetMostUpdatedApps(int topN)
        {
            return _cache
                .GroupBy(e => e.AppName)
                .Select(g => (AppName: g.Key, Count: g.Count()))
                .OrderByDescending(x => x.Count)
                .Take(topN)
                .ToList();
        }

        /// <summary>
        /// Aylýk güncelleme sayýlarý
        /// </summary>
        public Dictionary<string, int> GetMonthlyStatistics()
        {
            return _cache
                .GroupBy(e => e.UpdateDate.ToString("yyyy-MM"))
                .ToDictionary(g => g.Key, g => g.Count());
        }

        /// <summary>
        /// Geçmiþi temizle
        /// </summary>
        public void ClearHistory()
        {
            _cache.Clear();
            _nextId = 1;
            CreateEmptyDatabase();
        }

        /// <summary>
        /// Eski kayýtlarý sil (X günden eski)
        /// </summary>
        public void DeleteOldRecords(int daysOld)
        {
            var cutoffDate = DateTime.Now.AddDays(-daysOld);
            _cache.RemoveAll(e => e.UpdateDate < cutoffDate);
            SaveToFile();
        }

        /// <summary>
        /// Export to CSV (tüm geçmiþ)
        /// </summary>
        public void ExportToCsv(string filePath)
        {
            try
            {
                var sb = new StringBuilder();
                sb.AppendLine("Id,AppName,AppId,FromVersion,ToVersion,UpdateDate,Success,UpdateSizeBytes,DurationSeconds,ErrorMessage");

                foreach (var entry in _cache.OrderBy(e => e.UpdateDate))
                {
                    sb.AppendLine(entry.ToCsvLine());
                }

                File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);
            }
            catch (Exception ex)
            {
                throw new Exception($"Export failed: {ex.Message}");
            }
        }

        /// <summary>
        /// Database dosya yolunu döndürür
        /// </summary>
        public string GetDatabasePath()
        {
            return _dbFile;
        }
    }
}
