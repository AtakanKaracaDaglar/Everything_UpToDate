using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Everything_UpToDate.Models;

namespace Everything_UpToDate.Services
{
    /// <summary>
    /// Uygulama güncelleme iþlemlerini yöneten servis sýnýfý
    /// </summary>
    public class UpdateService
    {
        private DatabaseService _databaseService;

        public UpdateService()
        {
            _databaseService = null; // Lazy initialization
        }

        public void SetDatabaseService(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        /// <summary>
        /// Sisteme kurulu uygulamalarý tarar ve güncelleme durumlarýný kontrol eder
        /// </summary>
        public async Task<List<ApplicationInfo>> ScanForApplicationsAsync()
        {
            return await Task.Run(async () =>
            {
                var applications = new List<ApplicationInfo>();

                try
                {
                    // WinGet kullanarak kurulu uygulamalarý listele
                    var process = new Process
                    {
                        StartInfo = new ProcessStartInfo
                        {
                            FileName = "winget",
                            Arguments = "upgrade --accept-source-agreements",
                            UseShellExecute = false,
                            RedirectStandardOutput = true,
                            RedirectStandardError = true,
                            CreateNoWindow = true,
                            StandardOutputEncoding = System.Text.Encoding.UTF8
                        }
                    };

                    process.Start();
                    string output = process.StandardOutput.ReadToEnd();
                    process.WaitForExit();

                    // Çýktýyý parse et
                    var lines = output.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                    bool dataStarted = false;

                    foreach (var line in lines)
                    {
                        // Baþlýk satýrýný atla
                        if (line.Contains("Name") && line.Contains("Id") && line.Contains("Version"))
                        {
                            dataStarted = true;
                            continue;
                        }

                        // Ayraç satýrlarýný atla
                        if (line.Contains("---") || line.Contains("upgrade"))
                        {
                            continue;
                        }

                        if (!dataStarted || string.IsNullOrWhiteSpace(line))
                            continue;

                        // Satýrý parse et
                        var app = ParseWinGetLine(line);
                        if (app != null)
                        {
                            applications.Add(app);
                        }
                    }

                    // Uygulamalar için boyut bilgilerini paralel olarak al
                    var sizeUpdateTasks = applications.Select(async app =>
                    {
                        // 1. WinGet manifest'inden gerçek boyutu al
                        long manifestSize = await GetPackageSizeFromManifestAsync(app.Id);
                        
                        if (manifestSize > 0)
                        {
                            app.UpdateSizeBytes = manifestSize;
                        }
                        else
                        {
                            // 2. Kurulum klasörü boyutunu hesapla (arka planda)
                            await Task.Run(() =>
                            {
                                long installSize = CalculateInstallationSize(app.InstallPath);
                                if (installSize > 0)
                                {
                                    // Güncelleme genellikle kurulum boyutunun %30-50'si kadar
                                    app.UpdateSizeBytes = (long)(installSize * 0.4);
                                }
                                else
                                {
                                    // 3. Varsayýlan tahmin
                                    app.UpdateSizeBytes = GetEstimatedSizeByCategory(app.Name);
                                }
                            });
                        }
                    }).ToArray();

                    // Tüm boyut güncellemelerinin tamamlanmasýný bekle (max 5 saniye)
                    await Task.WhenAny(
                        Task.WhenAll(sizeUpdateTasks),
                        Task.Delay(5000) // 5 saniye timeout
                    );

                    // Eðer WinGet baþarýsýz olursa veya hiç uygulama bulunamazsa, fallback olarak bazý örnek uygulamalar ekle
                    if (applications.Count == 0)
                    {
                        applications = GetFallbackApplications();
                    }
                }
                catch (Exception ex)
                {
                    // WinGet bulunamazsa veya hata olursa, fallback listeyi kullan
                    Debug.WriteLine($"WinGet error: {ex.Message}");
                    applications = GetFallbackApplications();
                }

                return applications;
            });
        }

        /// <summary>
        /// WinGet çýktý satýrýný parse eder
        /// </summary>
        private ApplicationInfo ParseWinGetLine(string line)
        {
            try
            {
                // WinGet çýktý formatý genellikle þöyle:
                // Name                                Id                    Version        Available
                // Google Chrome                       Google.Chrome         120.0.6099.109 121.0.6167.85

                // Regex ile parse et - birden fazla boþluðu ayraç olarak kullan
                var parts = Regex.Split(line.Trim(), @"\s{2,}").Select(p => p.Trim()).ToArray();

                if (parts.Length < 4) // En az 4 kolon olmalý (Name, Id, Version, Available)
                    return null;

                string name = parts[0];
                string id = parts[1];
                string currentVersion = parts[2];
                string availableVersion = parts[3];

                // Boþ veya geçersiz satýrlarý atla
                if (string.IsNullOrWhiteSpace(name) || name.Length < 2)
                    return null;

                if (string.IsNullOrWhiteSpace(id) || id.Length < 2)
                    return null;

                // "Available" kelimesini içeren satýrlarý atla (baþlýk satýrý)
                if (name.Contains("Available") || name.Contains("---") || id.Contains("Id"))
                    return null;

                var app = new ApplicationInfo
                {
                    Name = name,
                    Id = id, // WinGet ID'sini kaydet - ÇOK ÖNEMLÝ!
                    CurrentVersion = currentVersion,
                    LatestVersion = availableVersion,
                    Description = GetDescriptionForApp(name),
                    InstallPath = GetInstallPathForApp(id),
                    UpdateSizeBytes = GetEstimatedSizeByCategory(name), // Baþlangýç tahmini
                    Status = UpdateStatus.Idle
                };

                return app;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Uygulama için açýklama döndürür
        /// </summary>
        private string GetDescriptionForApp(string appName)
        {
            var descriptions = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "Chrome", "Web tarayýcýsý - Güvenlik güncellemesi ve performans iyileþtirmeleri" },
                { "Firefox", "Web tarayýcýsý - Yeni özellikler ve güvenlik yamalarý" },
                { "Visual Studio Code", "Kod editörü - Yeni özellikler ve hata düzeltmeleri" },
                { "Node.js", "JavaScript runtime - LTS versiyonu, kritik güvenlik yamalarý" },
                { "VLC", "Medya oynatýcý - Codec güncellemeleri ve stabilite iyileþtirmeleri" },
                { "7-Zip", "Dosya sýkýþtýrma aracý - Yeni arþiv formatlarý desteði" },
                { "Git", "Versiyon kontrol sistemi - Performans optimizasyonlarý" },
                { "Adobe", "PDF okuyucu - Güvenlik güncellemeleri" },
                { "WinRAR", "Arþiv yöneticisi - Hata düzeltmeleri" },
                { "Python", "Programlama dili - Yeni özellikler ve hata düzeltmeleri" },
                { "Java", "Java Runtime Environment - Güvenlik güncellemeleri" },
                { "Notepad++", "Metin editörü - Yeni özellikler ve performans iyileþtirmeleri" },
                { "Spotify", "Müzik akýþ servisi - Kullanýcý deneyimi iyileþtirmeleri" },
                { "Discord", "Ýletiþim platformu - Yeni özellikler ve hata düzeltmeleri" },
                { "Zoom", "Video konferans - Güvenlik ve stabilite güncellemeleri" }
            };

            foreach (var kvp in descriptions)
            {
                if (appName.IndexOf(kvp.Key, StringComparison.OrdinalIgnoreCase) >= 0)
                    return kvp.Value;
            }

            return "Uygulama güncellemesi - Yeni özellikler ve hata düzeltmeleri";
        }

        /// <summary>
        /// Uygulama ID'sine göre kurulum yolu tahmin eder
        /// </summary>
        private string GetInstallPathForApp(string appId)
        {
            if (string.IsNullOrWhiteSpace(appId))
                return @"C:\Program Files\Unknown";

            // Bazý yaygýn yollar
            var pathMappings = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "Google.Chrome", @"C:\Program Files\Google\Chrome\Application" },
                { "Microsoft.VisualStudioCode", @"C:\Program Files\Microsoft VS Code" },
                { "OpenJS.NodeJS", @"C:\Program Files\nodejs" },
                { "VideoLAN.VLC", @"C:\Program Files\VideoLAN\VLC" },
                { "7zip.7zip", @"C:\Program Files\7-Zip" },
                { "Git.Git", @"C:\Program Files\Git" },
                { "Adobe.Acrobat.Reader", @"C:\Program Files\Adobe\Acrobat Reader DC" },
                { "RARLab.WinRAR", @"C:\Program Files\WinRAR" }
            };

            foreach (var kvp in pathMappings)
            {
                if (appId.IndexOf(kvp.Key, StringComparison.OrdinalIgnoreCase) >= 0)
                    return kvp.Value;
            }

            return $@"C:\Program Files\{appId}";
        }

        /// <summary>
        /// Güncelleme boyutunu tahmin eder (Kurulum klasörü boyutundan)
        /// </summary>
        private long EstimateUpdateSize(string appName)
        {
            // Varsayýlan boyut: 50 MB
            return 50L * 1024 * 1024;
        }

        /// <summary>
        /// Uygulama kurulum klasörünün gerçek boyutunu hesaplar
        /// </summary>
        private long CalculateInstallationSize(string installPath)
        {
            try
            {
                if (!System.IO.Directory.Exists(installPath))
                    return 0;

                var directory = new System.IO.DirectoryInfo(installPath);
                long size = 0;

                // Tüm dosyalarý topla
                foreach (var file in directory.GetFiles("*.*", System.IO.SearchOption.AllDirectories))
                {
                    try
                    {
                        size += file.Length;
                    }
                    catch
                    {
                        // Eriþim hatalarýný atla
                    }
                }

                return size;
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// WinGet manifest'inden gerçek paket boyutunu almaya çalýþýr
        /// </summary>
        private async Task<long> GetPackageSizeFromManifestAsync(string appId)
        {
            return await Task.Run(() =>
            {
                try
                {
                    var process = new Process
                    {
                        StartInfo = new ProcessStartInfo
                        {
                            FileName = "winget",
                            Arguments = $"show --id {appId}",
                            UseShellExecute = false,
                            RedirectStandardOutput = true,
                            CreateNoWindow = true,
                            StandardOutputEncoding = System.Text.Encoding.UTF8
                        }
                    };

                    process.Start();
                    string output = process.StandardOutput.ReadToEnd();
                    process.WaitForExit();

                    // "Size:" satýrýný bul
                    var lines = output.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var line in lines)
                    {
                        if (line.Contains("Size:") || line.Contains("Download Size:"))
                        {
                            // Örnek: "Download Size: 89.5 MB"
                            var sizeText = line.Split(':')[1].Trim();
                            return ParseSizeString(sizeText);
                        }
                    }

                    return 0;
                }
                catch
                {
                    return 0;
                }
            });
        }

        /// <summary>
        /// "89.5 MB" gibi string'i byte'a çevirir
        /// </summary>
        private long ParseSizeString(string sizeText)
        {
            try
            {
                sizeText = sizeText.ToUpper().Trim();

                // Sayýyý ve birimi ayýr
                var parts = sizeText.Split(' ');
                if (parts.Length < 2)
                    return 0;

                double value;
                if (!double.TryParse(parts[0], out value))
                    return 0;

                string unit = parts[1].ToUpper();

                // Birime göre çarpým faktörü
                if (unit.Contains("GB"))
                    return (long)(value * 1024 * 1024 * 1024);
                else if (unit.Contains("MB"))
                    return (long)(value * 1024 * 1024);
                else if (unit.Contains("KB"))
                    return (long)(value * 1024);
                else if (unit.Contains("B"))
                    return (long)value;

                return 0;
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// Uygulama kategorisine göre tahmini boyut döndürür
        /// </summary>
        private long GetEstimatedSizeByCategory(string appName)
        {
            var sizeMappings = new Dictionary<string, long>(StringComparer.OrdinalIgnoreCase)
            {
                { "Chrome", 89L * 1024 * 1024 },
                { "Firefox", 95L * 1024 * 1024 },
                { "Edge", 150L * 1024 * 1024 },
                { "Visual Studio Code", 125L * 1024 * 1024 },
                { "Visual Studio", 3L * 1024 * 1024 * 1024 }, // 3 GB
                { "Node.js", 32L * 1024 * 1024 },
                { "VLC", 45L * 1024 * 1024 },
                { "7-Zip", 2L * 1024 * 1024 },
                { "Git", 55L * 1024 * 1024 },
                { "Adobe", 200L * 1024 * 1024 },
                { "WinRAR", 3L * 1024 * 1024 },
                { "Python", 30L * 1024 * 1024 },
                { "Java", 150L * 1024 * 1024 },
                { "Notepad++", 4L * 1024 * 1024 },
                { "Spotify", 100L * 1024 * 1024 },
                { "Discord", 80L * 1024 * 1024 },
                { "Teams", 120L * 1024 * 1024 },
                { "Zoom", 60L * 1024 * 1024 },
                { "Docker", 500L * 1024 * 1024 },
                { "Slack", 90L * 1024 * 1024 },
                { "OBS", 150L * 1024 * 1024 }
            };

            foreach (var kvp in sizeMappings)
            {
                if (appName.IndexOf(kvp.Key, StringComparison.OrdinalIgnoreCase) >= 0)
                    return kvp.Value;
            }

            // Varsayýlan boyut: 50 MB
            return 50L * 1024 * 1024;
        }

        /// <summary>
        /// WinGet bulunamazsa fallback uygulamalar listesi
        /// </summary>
        private List<ApplicationInfo> GetFallbackApplications()
        {
            return new List<ApplicationInfo>
            {
                new ApplicationInfo
                {
                    Name = "WinGet Bulunamadý",
                    CurrentVersion = "N/A",
                    LatestVersion = "N/A",
                    Description = "Windows Package Manager (winget) sisteminizde bulunamadý. Lütfen Microsoft Store'dan 'App Installer' uygulamasýný yükleyin.",
                    InstallPath = "N/A",
                    UpdateSizeBytes = 0,
                    Status = UpdateStatus.Failed
                }
            };
        }

        /// <summary>
        /// Tek bir uygulamayý günceller
        /// </summary>
        public async Task<bool> UpdateApplicationAsync(ApplicationInfo app, IProgress<UpdateProgress> progress)
        {
            var startTime = DateTime.Now;
            bool success = false;
            string errorMessage = string.Empty;

            try
            {
                if (!app.IsUpdateAvailable)
                {
                    progress?.Report(new UpdateProgress
                    {
                        ApplicationName = app.Name,
                        Status = UpdateStatus.UpToDate,
                        Message = "Uygulama zaten güncel",
                        ProgressPercentage = 100
                    });
                    return true;
                }

                app.Status = UpdateStatus.Checking;
                progress?.Report(new UpdateProgress
                {
                    ApplicationName = app.Name,
                    Status = UpdateStatus.Checking,
                    Message = "Güncelleme kontrol ediliyor...",
                    ProgressPercentage = 0
                });

                // WinGet ile güncelleme yap - ID kullan!
                await Task.Run(() =>
                {
                    var process = new Process
                    {
                        StartInfo = new ProcessStartInfo
                        {
                            FileName = "winget",
                            Arguments = $"upgrade --id {app.Id} --silent --accept-source-agreements --accept-package-agreements",
                            UseShellExecute = false,
                            RedirectStandardOutput = true,
                            RedirectStandardError = true,
                            CreateNoWindow = true,
                            StandardOutputEncoding = System.Text.Encoding.UTF8
                        }
                    };

                    app.Status = UpdateStatus.Downloading;
                    progress?.Report(new UpdateProgress
                    {
                        ApplicationName = app.Name,
                        Status = UpdateStatus.Downloading,
                        Message = "Ýndiriliyor...",
                        ProgressPercentage = 30
                    });

                    process.Start();
                    string output = process.StandardOutput.ReadToEnd();
                    string error = process.StandardError.ReadToEnd();
                    process.WaitForExit();

                    Debug.WriteLine($"WinGet Output: {output}");
                    Debug.WriteLine($"WinGet Error: {error}");

                    if (process.ExitCode == 0 || output.Contains("Successfully installed"))
                    {
                        app.Status = UpdateStatus.Installing;
                        progress?.Report(new UpdateProgress
                        {
                            ApplicationName = app.Name,
                            Status = UpdateStatus.Installing,
                            Message = "Kuruluyor...",
                            ProgressPercentage = 75
                        });

                        app.CurrentVersion = app.LatestVersion;
                        app.Status = UpdateStatus.Completed;
                        success = true;
                    }
                    else
                    {
                        errorMessage = $"WinGet exit code: {process.ExitCode}. Error: {error}";
                        throw new Exception(errorMessage);
                    }
                });

                progress?.Report(new UpdateProgress
                {
                    ApplicationName = app.Name,
                    Status = UpdateStatus.Completed,
                    Message = "Güncelleme tamamlandý!",
                    ProgressPercentage = 100
                });

                success = true;
                return true;
            }
            catch (Exception ex)
            {
                app.Status = UpdateStatus.Failed;
                success = false;
                errorMessage = ex.Message;
                
                progress?.Report(new UpdateProgress
                {
                    ApplicationName = app.Name,
                    Status = UpdateStatus.Failed,
                    Message = $"Hata: {ex.Message}",
                    ProgressPercentage = 0
                });
                return false;
            }
            finally
            {
                // Veritabanýna kaydet
                if (_databaseService != null)
                {
                    var duration = (int)(DateTime.Now - startTime).TotalSeconds;
                    var historyEntry = new UpdateHistoryEntry
                    {
                        AppName = app.Name,
                        AppId = app.Id,
                        FromVersion = app.CurrentVersion,
                        ToVersion = app.LatestVersion,
                        UpdateDate = DateTime.Now,
                        Success = success,
                        UpdateSizeBytes = app.UpdateSizeBytes,
                        DurationSeconds = duration,
                        ErrorMessage = errorMessage
                    };

                    _databaseService.AddUpdateHistory(historyEntry);
                }
            }
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
                    var process = new Process
                    {
                        StartInfo = new ProcessStartInfo
                        {
                            FileName = "winget",
                            Arguments = "--version",
                            UseShellExecute = false,
                            RedirectStandardOutput = true,
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
