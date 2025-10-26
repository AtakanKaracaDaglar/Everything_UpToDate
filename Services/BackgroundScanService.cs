using System;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Everything_UpToDate.Services
{
    /// <summary>
    /// Arka plan tarama servisi
    /// </summary>
    public class BackgroundScanService : IDisposable
    {
        private readonly UpdateService _updateService;
        private readonly NotificationService _notificationService;
        private readonly SettingsService _settingsService;
        private Timer _scanTimer;
        private bool _isScanning = false;

        public event EventHandler<int> OnScanCompleted;

        public BackgroundScanService(
            UpdateService updateService,
            NotificationService notificationService,
            SettingsService settingsService)
        {
            _updateService = updateService;
            _notificationService = notificationService;
            _settingsService = settingsService;
        }

        /// <summary>
        /// Arka plan taramayý baþlat
        /// </summary>
        public void Start()
        {
            if (!_settingsService.Settings.AutoScanEnabled)
                return;

            Stop(); // Önce varsa durdur

            int intervalMs = _settingsService.Settings.AutoScanIntervalHours * 60 * 60 * 1000;

            _scanTimer = new Timer(async (state) =>
            {
                await PerformScanAsync();
            }, null, TimeSpan.FromSeconds(10), TimeSpan.FromMilliseconds(intervalMs));

            Debug.WriteLine($"Background scan started - Interval: {_settingsService.Settings.AutoScanIntervalHours} hours");
        }

        /// <summary>
        /// Arka plan taramayý durdur
        /// </summary>
        public void Stop()
        {
            if (_scanTimer != null)
            {
                _scanTimer.Dispose();
                _scanTimer = null;
                Debug.WriteLine("Background scan stopped");
            }
        }

        /// <summary>
        /// Manuel tarama tetikle
        /// </summary>
        public async Task TriggerScanAsync()
        {
            await PerformScanAsync();
        }

        /// <summary>
        /// Tarama iþlemini gerçekleþtir
        /// </summary>
        private async Task PerformScanAsync()
        {
            if (_isScanning)
            {
                Debug.WriteLine("Scan already in progress, skipping...");
                return;
            }

            try
            {
                _isScanning = true;
                Debug.WriteLine($"Background scan started at {DateTime.Now:HH:mm:ss}");

                // Tooltip güncelle
                _notificationService.UpdateTooltip("Everything UpToDate - Taranýyor...");

                var applications = await _updateService.ScanForApplicationsAsync();
                int updateCount = 0;

                foreach (var app in applications)
                {
                    if (app.IsUpdateAvailable)
                        updateCount++;
                }

                // Son tarama zamanýný güncelle
                _settingsService.UpdateSetting("LastScanTime", DateTime.Now);

                // Tooltip güncelle
                if (updateCount > 0)
                {
                    _notificationService.UpdateTooltip($"Everything UpToDate - {updateCount} güncelleme mevcut");
                    _notificationService.ShowUpdateNotification(updateCount);
                }
                else
                {
                    _notificationService.UpdateTooltip("Everything UpToDate - Tüm uygulamalar güncel");
                }

                Debug.WriteLine($"Background scan completed - {updateCount} updates found");

                // Event fýrlat
                OnScanCompleted?.Invoke(this, updateCount);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Background scan error: {ex.Message}");
                _notificationService.UpdateTooltip("Everything UpToDate - Tarama hatasý");
            }
            finally
            {
                _isScanning = false;
            }
        }

        /// <summary>
        /// Interval süresini güncelle
        /// </summary>
        public void UpdateInterval(int hours)
        {
            _settingsService.UpdateSetting("AutoScanIntervalHours", hours);
            
            if (_settingsService.Settings.AutoScanEnabled)
            {
                // Restart with new interval
                Start();
            }
        }

        /// <summary>
        /// Otomatik taramayý aç/kapat
        /// </summary>
        public void SetEnabled(bool enabled)
        {
            _settingsService.UpdateSetting("AutoScanEnabled", enabled);

            if (enabled)
            {
                Start();
            }
            else
            {
                Stop();
            }
        }

        public void Dispose()
        {
            Stop();
        }
    }
}
