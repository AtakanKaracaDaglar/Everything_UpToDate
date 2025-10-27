using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Everything_UpToDate.Models;

namespace Everything_UpToDate.Services
{
    /// <summary>
    /// Ana g�ncelleme servisi - T�m paket y�neticilerini koordine eder
    /// </summary>
    public class UpdateService
    {
        private DatabaseService _databaseService;
        private List<IPackageManager> _packageManagers;
        
        // Aktif paket y�neticileri
        private Dictionary<PackageSource, bool> _enabledSources;

        public UpdateService()
        {
            _databaseService = null; // Lazy initialization
            _packageManagers = new List<IPackageManager>();
            _enabledSources = new Dictionary<PackageSource, bool>
            {
                { PackageSource.WinGet, true },
                { PackageSource.Chocolatey, false },
                { PackageSource.MicrosoftStore, false },
                { PackageSource.Steam, false }
            };
            
            InitializePackageManagers();
        }

        private void InitializePackageManagers()
        {
            // WinGet'i ekle
            _packageManagers.Add(new PackageManagers.WinGetPackageManager());
            
            // Chocolatey'i ekle
            _packageManagers.Add(new PackageManagers.ChocolateyPackageManager());
            
            // Microsoft Store'u ekle
            _packageManagers.Add(new PackageManagers.MicrosoftStorePackageManager());
            
            // Steam - gelecekte eklenecek
            // _packageManagers.Add(new SteamPackageManager());
        }

        public void SetDatabaseService(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        /// <summary>
        /// Belirli bir kayna�� aktif/pasif eder
        /// </summary>
        public void SetSourceEnabled(PackageSource source, bool enabled)
        {
            if (_enabledSources.ContainsKey(source))
            {
                _enabledSources[source] = enabled;
            }
        }

        /// <summary>
        /// Bir kayna��n aktif olup olmad���n� kontrol eder
        /// </summary>
        public bool IsSourceEnabled(PackageSource source)
        {
            return _enabledSources.ContainsKey(source) && _enabledSources[source];
        }

        /// <summary>
        /// Aktif t�m kaynaklardan uygulamalar� tarar
        /// </summary>
        public async Task<List<ApplicationInfo>> ScanForApplicationsAsync()
        {
            var allApplications = new List<ApplicationInfo>();

            foreach (var manager in _packageManagers)
            {
                try
                {
                    // Paket y�neticisinin kayna��n� bul
                    var source = GetSourceFromManager(manager);
                    
                    // E�er bu kaynak aktif de�ilse atla
                    if (!IsSourceEnabled(source))
                    {
                        Debug.WriteLine($"{manager.Name} devre d���, atlan�yor");
                        continue;
                    }

                    // Paket y�neticisi kurulu mu kontrol et
                    bool isInstalled = await manager.IsInstalledAsync();
                    if (!isInstalled)
                    {
                        Debug.WriteLine($"{manager.Name} kurulu de�il, atlan�yor");
                        continue;
                    }

                    Debug.WriteLine($"{manager.Name} taran�yor...");

                    // Uygulamalar� tara
                    var apps = await manager.ScanForUpdatesAsync();
                    allApplications.AddRange(apps);

                    Debug.WriteLine($"{manager.Name}: {apps.Count} uygulama bulundu");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error scanning {manager.Name}: {ex.Message}");
                }
            }

            return allApplications;
        }

        /// <summary>
        /// Paket y�neticisinden source'u ��kar�r
        /// </summary>
        private PackageSource GetSourceFromManager(IPackageManager manager)
        {
            // Name bazl� source belirleme
            if (manager.Name == "WinGet") return PackageSource.WinGet;
            if (manager.Name == "Chocolatey") return PackageSource.Chocolatey;
            if (manager.Name == "Microsoft Store") return PackageSource.MicrosoftStore;
            if (manager.Name == "Steam") return PackageSource.Steam;
            return PackageSource.Unknown;
        }

        /// <summary>
        /// Tek bir uygulamay� g�nceller
        /// </summary>
        public async Task<bool> UpdateApplicationAsync(ApplicationInfo app, IProgress<UpdateProgress> progress)
        {
            var startTime = DateTime.Now;
            var fromVersion = app.CurrentVersion;
            
            // �lgili paket y�neticisini bul
            var manager = _packageManagers.FirstOrDefault(m => GetSourceFromManager(m) == app.Source);
            
            if (manager == null)
            {
                progress?.Report(new UpdateProgress
                {
                    ApplicationName = app.Name,
                    Status = UpdateStatus.Failed,
                    Message = "Paket y�neticisi bulunamad�",
                    ProgressPercentage = 100
                });
                return false;
            }
            
            // G�ncellemeyi yap
            bool success = await manager.UpdateApplicationAsync(app, progress);
            
            // Database'e kaydet (e�er DatabaseService varsa)
            if (_databaseService != null)
            {
                var duration = (DateTime.Now - startTime).TotalSeconds;
                
                var historyEntry = new UpdateHistoryEntry
                {
                    AppName = app.Name,
                    AppId = app.Id,
                    FromVersion = fromVersion,
                    ToVersion = app.LatestVersion,
                    UpdateDate = DateTime.Now,
                    Success = success,
                    UpdateSizeBytes = app.UpdateSizeBytes,
                    DurationSeconds = (int)duration,
                    ErrorMessage = success ? null : "G�ncelleme ba�ar�s�z oldu"
                };
                
                _databaseService.AddUpdateHistory(historyEntry);
                Debug.WriteLine($"Database kayd� eklendi: {app.Name} - {(success ? "Ba�ar�l�" : "Ba�ar�s�z")}");
            }
            
            return success;
        }

        /// <summary>
        /// Birden fazla uygulamay� s�rayla g�nceller
        /// </summary>
        public async Task UpdateMultipleApplicationsAsync(List<ApplicationInfo> applications, IProgress<UpdateProgress> progress)
        {
            int totalApps = applications.Count;
            int currentApp = 0;

            foreach (var app in applications)
            {
                currentApp++;
                progress?.Report(new UpdateProgress
                {
                    ApplicationName = "Toplu G�ncelleme",
                    Status = UpdateStatus.Downloading,
                    Message = $"({currentApp}/{totalApps}) {app.Name} g�ncelleniyor...",
                    ProgressPercentage = (currentApp * 100) / totalApps
                });

                await UpdateApplicationAsync(app, progress);
            }

            progress?.Report(new UpdateProgress
            {
                ApplicationName = "Toplu G�ncelleme",
                Status = UpdateStatus.Completed,
                Message = $"T�m g�ncellemeler tamamland�! ({totalApps} uygulama)",
                ProgressPercentage = 100
            });
        }

        /// <summary>
        /// WinGet'in sistemde kurulu olup olmad���n� kontrol eder
        /// </summary>
        public async Task<bool> CheckWinGetAvailabilityAsync()
        {
            return await Task.Run(() =>
            {
                try
                {
                    var process = new System.Diagnostics.Process
                    {
                        StartInfo = new System.Diagnostics.ProcessStartInfo
                        {
                            FileName = "winget",
                            Arguments = "--version",
                            UseShellExecute = false,
                            RedirectStandardOutput = true,
                            RedirectStandardError = true,
                            CreateNoWindow = true
                        }
                    };

                    process.Start();
                    process.WaitForExit();
                    
                    return process.ExitCode == 0;
                }
                catch
                {
                    return false;
                }
            });
        }
    }

    /// <summary>
    /// G�ncelleme ilerlemesi i�in progress s�n�f�
    /// </summary>
    public class UpdateProgress
    {
        public string ApplicationName { get; set; }
        public UpdateStatus Status { get; set; }
        public string Message { get; set; }
        public int ProgressPercentage { get; set; }
    }
}
