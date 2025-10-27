using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Everything_UpToDate.Models;

namespace Everything_UpToDate.Services.PackageManagers
{
    /// <summary>
    /// Windows Package Manager (WinGet) implementasyonu
    /// </summary>
    public class WinGetPackageManager : IPackageManager
    {
        public string Name => "WinGet";

        public async Task<bool> IsInstalledAsync()
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

        public async Task<List<ApplicationInfo>> ScanForUpdatesAsync()
        {
            return await Task.Run(() =>
            {
                var applications = new List<ApplicationInfo>();

                try
                {
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

                    // Output'u satýrlara böl
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

                        if (!dataStarted || line.Contains("---") || line.Contains("upgrade") || 
                            line.Contains("upgrades available"))
                            continue;

                        // WinGet çýktýsýný parse et
                        var app = ParseWinGetLine(line);
                        if (app != null)
                        {
                            app.Source = PackageSource.WinGet;
                            app.LastChecked = DateTime.Now;
                            applications.Add(app);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"WinGet scan error: {ex.Message}");
                }

                return applications;
            });
        }

        private ApplicationInfo ParseWinGetLine(string line)
        {
            try
            {
                // WinGet çýktý formatý: Name   Id   Version   Available   Source
                // Regex ile parse et
                var parts = Regex.Split(line.Trim(), @"\s{2,}").Where(s => !string.IsNullOrWhiteSpace(s)).ToArray();

                if (parts.Length >= 4)
                {
                    return new ApplicationInfo
                    {
                        Name = parts[0].Trim(),
                        Id = parts[1].Trim(),
                        CurrentVersion = parts[2].Trim(),
                        LatestVersion = parts[3].Trim(),
                        Description = $"Güncellenebilir uygulama: {parts[0]}",
                        InstallPath = "WinGet Package",
                        UpdateSizeBytes = 0, // Boyut hesaplamasý ayrý yapýlacak
                        Status = UpdateStatus.Idle
                    };
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Parse error: {ex.Message} - Line: {line}");
            }

            return null;
        }

        public async Task<bool> UpdateApplicationAsync(ApplicationInfo app, IProgress<UpdateProgress> progress)
        {
            return await Task.Run(() =>
            {
                try
                {
                    progress?.Report(new UpdateProgress
                    {
                        ApplicationName = app.Name,
                        Status = UpdateStatus.Downloading,
                        Message = "Güncelleme baþlatýlýyor...",
                        ProgressPercentage = 5
                    });

                    var process = new Process
                    {
                        StartInfo = new ProcessStartInfo
                        {
                            FileName = "winget",
                            Arguments = $"upgrade --id {app.Id} --force --accept-source-agreements --accept-package-agreements --silent",
                            UseShellExecute = false,
                            RedirectStandardOutput = true,
                            RedirectStandardError = true,
                            CreateNoWindow = true
                        }
                    };

                    // Output handler - gerçek zamanlý log
                    var outputBuilder = new System.Text.StringBuilder();
                    int lastProgress = 5;
                    
                    process.OutputDataReceived += (sender, e) =>
                    {
                        if (!string.IsNullOrEmpty(e.Data))
                        {
                            outputBuilder.AppendLine(e.Data);
                            var line = e.Data.Trim();
                            
                            // Loading animasyonunu atla (-\|/)
                            if (line.Length == 1 && (line == "-" || line == "\\" || line == "|" || line == "/"))
                            {
                                return; // Gereksiz log yazmayalým
                            }
                            
                            Debug.WriteLine($"[WinGet] {line}");
                            
                            UpdateStatus status = UpdateStatus.Downloading;
                            int progressPercent = lastProgress;
                            string displayMessage = line;
                            
                            // "Downloading" içeriyorsa
                            if (line.Contains("Downloading") || line.Contains("Download"))
                            {
                                status = UpdateStatus.Downloading;
                                progressPercent = 20;
                                displayMessage = "Ýndiriliyor...";
                            }
                            // Progress bar veya boyut bilgisi varsa
                            else if (line.Contains("?") || line.Contains("KB") || line.Contains("MB") || line.Contains("GB"))
                            {
                                status = UpdateStatus.Downloading;
                                
                                // "548 KB / 548 KB" formatýný yakala
                                var sizeMatch = System.Text.RegularExpressions.Regex.Match(line, @"(\d+\.?\d*)\s*(KB|MB|GB)\s*/\s*(\d+\.?\d*)\s*(KB|MB|GB)");
                                if (sizeMatch.Success)
                                {
                                    double current = double.Parse(sizeMatch.Groups[1].Value);
                                    string currentUnit = sizeMatch.Groups[2].Value;
                                    double total = double.Parse(sizeMatch.Groups[3].Value);
                                    string totalUnit = sizeMatch.Groups[4].Value;
                                    
                                    // Progress hesapla
                                    progressPercent = Math.Min((int)((current / total) * 70) + 20, 90); // 20-90 arasý
                                    
                                    displayMessage = $"Ýndiriliyor: {current:F1} {currentUnit} / {total:F1} {totalUnit}";
                                }
                                else
                                {
                                    // Boyut bulunamadý ama progress var
                                    displayMessage = "Ýndiriliyor...";
                                    progressPercent = 50;
                                }
                            }
                            // "Installing" içeriyorsa
                            else if (line.Contains("Installing") || line.Contains("install") || line.Contains("Yükleniyor"))
                            {
                                status = UpdateStatus.Installing;
                                progressPercent = 92;
                                displayMessage = "Yükleniyor...";
                            }
                            // "Successfully" içeriyorsa
                            else if (line.Contains("Successfully") || line.Contains("Baþarýyla"))
                            {
                                status = UpdateStatus.Installing;
                                progressPercent = 98;
                                displayMessage = line;
                            }
                            // "Found" - uygulama bulundu
                            else if (line.Contains("Found"))
                            {
                                status = UpdateStatus.Downloading;
                                progressPercent = 10;
                                displayMessage = line;
                            }
                            
                            lastProgress = progressPercent;
                            
                            // Progress güncelle
                            progress?.Report(new UpdateProgress
                            {
                                ApplicationName = app.Name,
                                Status = status,
                                Message = displayMessage,
                                ProgressPercentage = progressPercent
                            });
                        }
                    };

                    process.ErrorDataReceived += (sender, e) =>
                    {
                        if (!string.IsNullOrEmpty(e.Data))
                        {
                            Debug.WriteLine($"[WinGet ERROR] {e.Data}");
                            progress?.Report(new UpdateProgress
                            {
                                ApplicationName = app.Name,
                                Status = UpdateStatus.Failed,
                                Message = $"Hata: {e.Data}",
                                ProgressPercentage = lastProgress
                            });
                        }
                    };

                    process.Start();
                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();
                    process.WaitForExit();

                    bool success = process.ExitCode == 0;

                    progress?.Report(new UpdateProgress
                    {
                        ApplicationName = app.Name,
                        Status = success ? UpdateStatus.Completed : UpdateStatus.Failed,
                        Message = success ? "? Güncelleme tamamlandý!" : $"? Güncelleme baþarýsýz (Exit Code: {process.ExitCode})",
                        ProgressPercentage = 100
                    });

                    if (success)
                    {
                        app.CurrentVersion = app.LatestVersion;
                        app.Status = UpdateStatus.Completed;
                    }

                    return success;
                }
                catch (Exception ex)
                {
                    progress?.Report(new UpdateProgress
                    {
                        ApplicationName = app.Name,
                        Status = UpdateStatus.Failed,
                        Message = $"Hata: {ex.Message}",
                        ProgressPercentage = 100
                    });
                    return false;
                }
            });
        }

        public async Task UpdateMultipleApplicationsAsync(List<ApplicationInfo> applications, IProgress<UpdateProgress> progress)
        {
            int total = applications.Count;
            int current = 0;

            foreach (var app in applications)
            {
                current++;
                progress?.Report(new UpdateProgress
                {
                    ApplicationName = "Toplu Güncelleme",
                    Status = UpdateStatus.Downloading,
                    Message = $"({current}/{total}) {app.Name} güncelleniyor...",
                    ProgressPercentage = (current * 100) / total
                });

                await UpdateApplicationAsync(app, progress);
            }
        }
    }
}
