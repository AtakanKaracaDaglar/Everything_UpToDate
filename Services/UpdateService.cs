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
    /// Uygulama g�ncelleme i�lemlerini y�neten servis s�n�f�
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
        /// Sisteme kurulu uygulamalar� tarar ve g�ncelleme durumlar�n� kontrol eder
        /// </summary>
        public async Task<List<ApplicationInfo>> ScanForApplicationsAsync()
        {
            return await Task.Run(async () =>
            {
                var applications = new List<ApplicationInfo>();

                try
                {
                    // WinGet kullanarak kurulu uygulamalar� listele
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

                    // ��kt�y� parse et
                    var lines = output.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                    bool dataStarted = false;

                    foreach (var line in lines)
                    {
                        // Ba�l�k sat�r�n� atla
                        if (line.Contains("Name") && line.Contains("Id") && line.Contains("Version"))
                        {
                            dataStarted = true;
                            continue;
                        }

                        // Ayra� sat�rlar�n� atla
                        if (line.Contains("---") || line.Contains("upgrade"))
                        {
                            continue;
                        }

                        if (!dataStarted || string.IsNullOrWhiteSpace(line))
                            continue;

                        // Sat�r� parse et
                        var app = ParseWinGetLine(line);
                        if (app != null)
                        {
                            applications.Add(app);
                        }
                    }

                    // Uygulamalar i�in boyut bilgilerini paralel olarak al
                    var sizeUpdateTasks = applications.Select(async app =>
                    {
                        // 1. WinGet manifest'inden ger�ek boyutu al
                        long manifestSize = await GetPackageSizeFromManifestAsync(app.Id);
                        
                        if (manifestSize > 0)
                        {
                            app.UpdateSizeBytes = manifestSize;
                        }
                        else
                        {
                            // 2. Kurulum klas�r� boyutunu hesapla (arka planda)
                            await Task.Run(() =>
                            {
                                long installSize = CalculateInstallationSize(app.InstallPath);
                                if (installSize > 0)
                                {
                                    // G�ncelleme genellikle kurulum boyutunun %30-50'si kadar
                                    app.UpdateSizeBytes = (long)(installSize * 0.4);
                                }
                                else
                                {
                                    // 3. Varsay�lan tahmin
                                    app.UpdateSizeBytes = GetEstimatedSizeByCategory(app.Name);
                                }
                            });
                        }
                    }).ToArray();

                    // T�m boyut g�ncellemelerinin tamamlanmas�n� bekle (max 5 saniye)
                    await Task.WhenAny(
                        Task.WhenAll(sizeUpdateTasks),
                        Task.Delay(5000) // 5 saniye timeout
                    );

                    // E�er WinGet ba�ar�s�z olursa veya hi� uygulama bulunamazsa, fallback olarak baz� �rnek uygulamalar ekle
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
        /// WinGet ��kt� sat�r�n� parse eder
        /// </summary>
        private ApplicationInfo ParseWinGetLine(string line)
        {
            try
            {
                // WinGet ��kt� format� genellikle ��yle:
                // Name                                Id                    Version        Available
                // Google Chrome                       Google.Chrome         120.0.6099.109 121.0.6167.85

                // Regex ile parse et - birden fazla bo�lu�u ayra� olarak kullan
                var parts = Regex.Split(line.Trim(), @"\s{2,}").Select(p => p.Trim()).ToArray();

                if (parts.Length < 4) // En az 4 kolon olmal� (Name, Id, Version, Available)
                    return null;

                string name = parts[0];
                string id = parts[1];
                string currentVersion = parts[2];
                string availableVersion = parts[3];

                // Bo� veya ge�ersiz sat�rlar� atla
                if (string.IsNullOrWhiteSpace(name) || name.Length < 2)
                    return null;

                if (string.IsNullOrWhiteSpace(id) || id.Length < 2)
                    return null;

                // "Available" kelimesini i�eren sat�rlar� atla (ba�l�k sat�r�)
                if (name.Contains("Available") || name.Contains("---") || id.Contains("Id"))
                    return null;

                var app = new ApplicationInfo
                {
                    Name = name,
                    Id = id, // WinGet ID'sini kaydet - �OK �NEML�!
                    CurrentVersion = currentVersion,
                    LatestVersion = availableVersion,
                    Description = GetDescriptionForApp(name),
                    InstallPath = GetInstallPathForApp(id),
                    UpdateSizeBytes = GetEstimatedSizeByCategory(name), // Ba�lang�� tahmini
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
        /// Uygulama i�in a��klama d�nd�r�r
        /// </summary>
        private string GetDescriptionForApp(string appName)
        {
            var descriptions = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "Chrome", "Web taray�c�s� - G�venlik g�ncellemesi ve performans iyile�tirmeleri" },
                { "Firefox", "Web taray�c�s� - Yeni �zellikler ve g�venlik yamalar�" },
                { "Visual Studio Code", "Kod edit�r� - Yeni �zellikler ve hata d�zeltmeleri" },
                { "Node.js", "JavaScript runtime - LTS versiyonu, kritik g�venlik yamalar�" },
                { "VLC", "Medya oynat�c� - Codec g�ncellemeleri ve stabilite iyile�tirmeleri" },
                { "7-Zip", "Dosya s�k��t�rma arac� - Yeni ar�iv formatlar� deste�i" },
                { "Git", "Versiyon kontrol sistemi - Performans optimizasyonlar�" },
                { "Adobe", "PDF okuyucu - G�venlik g�ncellemeleri" },
                { "WinRAR", "Ar�iv y�neticisi - Hata d�zeltmeleri" },
                { "Python", "Programlama dili - Yeni �zellikler ve hata d�zeltmeleri" },
                { "Java", "Java Runtime Environment - G�venlik g�ncellemeleri" },
                { "Notepad++", "Metin edit�r� - Yeni �zellikler ve performans iyile�tirmeleri" },
                { "Spotify", "M�zik ak�� servisi - Kullan�c� deneyimi iyile�tirmeleri" },
                { "Discord", "�leti�im platformu - Yeni �zellikler ve hata d�zeltmeleri" },
                { "Zoom", "Video konferans - G�venlik ve stabilite g�ncellemeleri" }
            };

            foreach (var kvp in descriptions)
            {
                if (appName.IndexOf(kvp.Key, StringComparison.OrdinalIgnoreCase) >= 0)
                    return kvp.Value;
            }

            return "Uygulama g�ncellemesi - Yeni �zellikler ve hata d�zeltmeleri";
        }

        /// <summary>
        /// Uygulama ID'sine g�re kurulum yolu tahmin eder
        /// </summary>
        private string GetInstallPathForApp(string appId)
        {
            if (string.IsNullOrWhiteSpace(appId))
                return @"C:\Program Files\Unknown";

            // Baz� yayg�n yollar
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
        /// G�ncelleme boyutunu tahmin eder (Kurulum klas�r� boyutundan)
        /// </summary>
        private long EstimateUpdateSize(string appName)
        {
            // Varsay�lan boyut: 50 MB
            return 50L * 1024 * 1024;
        }

        /// <summary>
        /// Uygulama kurulum klas�r�n�n ger�ek boyutunu hesaplar
        /// </summary>
        private long CalculateInstallationSize(string installPath)
        {
            try
            {
                if (!System.IO.Directory.Exists(installPath))
                    return 0;

                var directory = new System.IO.DirectoryInfo(installPath);
                long size = 0;

                // T�m dosyalar� topla
                foreach (var file in directory.GetFiles("*.*", System.IO.SearchOption.AllDirectories))
                {
                    try
                    {
                        size += file.Length;
                    }
                    catch
                    {
                        // Eri�im hatalar�n� atla
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
        /// WinGet manifest'inden ger�ek paket boyutunu almaya �al���r
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

                    // "Size:" sat�r�n� bul
                    var lines = output.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var line in lines)
                    {
                        if (line.Contains("Size:") || line.Contains("Download Size:"))
                        {
                            // �rnek: "Download Size: 89.5 MB"
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
        /// "89.5 MB" gibi string'i byte'a �evirir
        /// </summary>
        private long ParseSizeString(string sizeText)
        {
            try
            {
                sizeText = sizeText.ToUpper().Trim();

                // Say�y� ve birimi ay�r
                var parts = sizeText.Split(' ');
                if (parts.Length < 2)
                    return 0;

                double value;
                if (!double.TryParse(parts[0], out value))
                    return 0;

                string unit = parts[1].ToUpper();

                // Birime g�re �arp�m fakt�r�
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
        /// Uygulama kategorisine g�re tahmini boyut d�nd�r�r
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

            // Varsay�lan boyut: 50 MB
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
                    Name = "WinGet Bulunamad�",
                    CurrentVersion = "N/A",
                    LatestVersion = "N/A",
                    Description = "Windows Package Manager (winget) sisteminizde bulunamad�. L�tfen Microsoft Store'dan 'App Installer' uygulamas�n� y�kleyin.",
                    InstallPath = "N/A",
                    UpdateSizeBytes = 0,
                    Status = UpdateStatus.Failed
                }
            };
        }

        /// <summary>
        /// Tek bir uygulamay� g�nceller
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
                        Message = "Uygulama zaten g�ncel",
                        ProgressPercentage = 100
                    });
                    return true;
                }

                app.Status = UpdateStatus.Checking;
                progress?.Report(new UpdateProgress
                {
                    ApplicationName = app.Name,
                    Status = UpdateStatus.Checking,
                    Message = "G�ncelleme kontrol ediliyor...",
                    ProgressPercentage = 0
                });

                // WinGet ile g�ncelleme yap - ID kullan!
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
                        Message = "�ndiriliyor...",
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
                    Message = "G�ncelleme tamamland�!",
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
                // Veritaban�na kaydet
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
