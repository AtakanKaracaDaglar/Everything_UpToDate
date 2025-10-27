using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Everything_UpToDate.Models;

namespace Everything_UpToDate.Services
{
    /// <summary>
    /// Ana güncelleme servisi - Tüm paket yöneticilerini koordine eder
    /// </summary>
    public class UpdateService
    {
        private DatabaseService _databaseService;
        private List<IPackageManager> _packageManagers;
        
        // Aktif paket yöneticileri
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
        /// Belirli bir kaynaðý aktif/pasif eder
        /// </summary>
        public void SetSourceEnabled(PackageSource source, bool enabled)
        {
            if (_enabledSources.ContainsKey(source))
            {
                _enabledSources[source] = enabled;
            }
        }

        /// <summary>
        /// Bir kaynaðýn aktif olup olmadýðýný kontrol eder
        /// </summary>
        public bool IsSourceEnabled(PackageSource source)
        {
            return _enabledSources.ContainsKey(source) && _enabledSources[source];
        }

        /// <summary>
        /// Aktif tüm kaynaklardan uygulamalarý tarar
        /// </summary>
        public async Task<List<ApplicationInfo>> ScanForApplicationsAsync()
        {
            var allApplications = new List<ApplicationInfo>();

            foreach (var manager in _packageManagers)
            {
                try
                {
                    // Paket yöneticisinin kaynaðýný bul
                    var source = GetSourceFromManager(manager);
                    
                    // Eðer bu kaynak aktif deðilse atla
                    if (!IsSourceEnabled(source))
                    {
                        Debug.WriteLine($"{manager.Name} devre dýþý, atlanýyor");
                        continue;
                    }

                    // Paket yöneticisi kurulu mu kontrol et
                    bool isInstalled = await manager.IsInstalledAsync();
                    if (!isInstalled)
                    {
                        Debug.WriteLine($"{manager.Name} kurulu deðil, atlanýyor");
                        continue;
                    }

                    Debug.WriteLine($"{manager.Name} taranýyor...");

                    // Uygulamalarý tara
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
        /// Paket yöneticisinden source'u çýkarýr
        /// </summary>
        private PackageSource GetSourceFromManager(IPackageManager manager)
        {
            // Name bazlý source belirleme
            if (manager.Name == "WinGet") return PackageSource.WinGet;
            if (manager.Name == "Chocolatey") return PackageSource.Chocolatey;
            if (manager.Name == "Microsoft Store") return PackageSource.MicrosoftStore;
            if (manager.Name == "Steam") return PackageSource.Steam;
            return PackageSource.Unknown;
        }

        /// <summary>
        /// Tek bir uygulamayý günceller
        /// </summary>
        public async Task<bool> UpdateApplicationAsync(ApplicationInfo app, IProgress<UpdateProgress> progress)
        {
            var startTime = DateTime.Now;
            var fromVersion = app.CurrentVersion;
            
            // Ýlgili paket yöneticisini bul
            var manager = _packageManagers.FirstOrDefault(m => GetSourceFromManager(m) == app.Source);
            
            if (manager == null)
            {
                progress?.Report(new UpdateProgress
                {
                    ApplicationName = app.Name,
                    Status = UpdateStatus.Failed,
                    Message = "Paket yöneticisi bulunamadý",
                    ProgressPercentage = 100
                });
                return false;
            }
            
            // Güncellemeyi yap
            bool success = await manager.UpdateApplicationAsync(app, progress);
            
            // Database'e kaydet (eðer DatabaseService varsa)
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
                    ErrorMessage = success ? null : "Güncelleme baþarýsýz oldu"
                };
                
                _databaseService.AddUpdateHistory(historyEntry);
                Debug.WriteLine($"Database kaydý eklendi: {app.Name} - {(success ? "Baþarýlý" : "Baþarýsýz")}");
            }
            
            return success;
        }

        /// <summary>
        /// Birden fazla uygulamayý sýrayla günceller
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
                    ApplicationName = "Toplu Güncelleme",
                    Status = UpdateStatus.Downloading,
                    Message = $"({currentApp}/{totalApps}) {app.Name} güncelleniyor...",
                    ProgressPercentage = (currentApp * 100) / totalApps
                });

                await UpdateApplicationAsync(app, progress);
            }

            progress?.Report(new UpdateProgress
            {
                ApplicationName = "Toplu Güncelleme",
                Status = UpdateStatus.Completed,
                Message = $"Tüm güncellemeler tamamlandý! ({totalApps} uygulama)",
                ProgressPercentage = 100
            });
        }

        /// <summary>
        /// WinGet'in sistemde kurulu olup olmadýðýný kontrol eder
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
    /// Güncelleme ilerlemesi için progress sýnýfý
    /// </summary>
    public class UpdateProgress
    {
        public string ApplicationName { get; set; }
        public UpdateStatus Status { get; set; }
        public string Message { get; set; }
        public int ProgressPercentage { get; set; }
    }
}
