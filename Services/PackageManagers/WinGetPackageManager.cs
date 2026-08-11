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
                            CreateNoWindow = true,
                            StandardOutputEncoding = System.Text.Encoding.UTF8
                        }
                    };

                    // Output handler - gerçek zamanlý log
                    var outputBuilder = new System.Text.StringBuilder();
                    var errorBuilder = new System.Text.StringBuilder();
                    int lastProgress = 5;
                    
                    process.OutputDataReceived += (sender, e) =>
                    {
                        if (!string.IsNullOrEmpty(e.Data))
                        {
                            outputBuilder.AppendLine(e.Data);
                            Debug.WriteLine($"[WinGet] {e.Data}");
                            
                            // Loading animasyonunu atla (-\|/)
                            var line = e.Data.Trim();
                            if (line.Length == 1 && (line == "-" || line == "\\" || line == "|" || line == "/"))
                            {
                                return; // Gereksiz log yazmayalým
                            }
                            
                            // Boyut ve yüzde bilgilerini parse et
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
                            // Progress bar varsa (???? gibi)
                            else if (line.Contains("?") || line.Contains("?"))
                            {
                                status = UpdateStatus.Downloading;
                                
                                // "50.5 MB / 100 MB" formatýný yakala
                                var sizeMatch = System.Text.RegularExpressions.Regex.Match(line, @"(\d+\.?\d*)\s*(MB|GB|KB)\s*/\s*(\d+\.?\d*)\s*(MB|GB|KB)");
                                if (sizeMatch.Success)
                                {
                                    double current = double.Parse(sizeMatch.Groups[1].Value);
                                    double total = double.Parse(sizeMatch.Groups[3].Value);
                                    progressPercent = Math.Min((int)((current / total) * 70) + 20, 90); // 20-90 arasý
                                    
                                    displayMessage = $"Ýndiriliyor: {current:F1} {sizeMatch.Groups[2].Value} / {total:F1} {sizeMatch.Groups[4].Value}";
                                }
                                else
                                {
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
                            
                            lastProgress = Math.Max(lastProgress, progressPercent);
                            
                            // Progress güncelle
                            progress?.Report(new UpdateProgress
                            {
                                ApplicationName = app.Name,
                                Status = status,
                                Message = displayMessage,
                                ProgressPercentage = Math.Min(lastProgress, 99)
                            });
                        }
                    };

                    process.ErrorDataReceived += (sender, e) =>
                    {
                        if (!string.IsNullOrEmpty(e.Data))
                        {
                            errorBuilder.AppendLine(e.Data);
                            Debug.WriteLine($"[WinGet ERROR] {e.Data}");
                        }
                    };

                    process.Start();
                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();
                    process.WaitForExit();

                    // Exit code kontrolü - WinGet için 0 = baþarýlý
                    bool success = process.ExitCode == 0;
                    string fullOutput = outputBuilder.ToString();
                    string fullError = errorBuilder.ToString();

                    // Success indications (WinGet baþarýlý göstergeleri)
                    bool hasSuccessIndicator = fullOutput.Contains("Successfully") ||
                                              fullOutput.Contains("Baþarýyla") ||
                                              fullOutput.Contains("installed");

                    // Failure indicators (Baþarýsýz göstergeleri)
                    bool hasFailureIndicator = fullOutput.Contains("The package cannot be upgraded") ||
                                              fullOutput.Contains("No applicable upgrade found") ||
                                              fullOutput.Contains("does not apply to your system") ||
                                              fullOutput.Contains("Permission denied") ||
                                              fullOutput.Contains("Access denied") ||
                                              fullOutput.Contains("Access is denied");

                    // Hata kodu negatifse sistem hatasý - output'a bak
                    if (process.ExitCode < 0 || process.ExitCode > 0)
                    {
                        Debug.WriteLine($"WinGet exit code: {process.ExitCode}");
                        Debug.WriteLine($"Output: {fullOutput}");
                        Debug.WriteLine($"Error: {fullError}");

                        // Eðer output'ta baþarý göstergesi varsa baþarýlý say
                        if (hasSuccessIndicator && !hasFailureIndicator)
                        {
                            success = true;
                        }
                        else if (hasFailureIndicator)
                        {
                            success = false;
                        }
                    }

                    // Fallback: Açýk baþarý veya baþarýsýzlýk kontrolü
                    if (!success && hasSuccessIndicator && !hasFailureIndicator)
                    {
                        success = true;
                    }
                    else if (success && hasFailureIndicator)
                    {
                        success = false;
                    }

                    Debug.WriteLine($"WinGet final result: {(success ? "SUCCESS" : "FAILED")}");
                    Debug.WriteLine($"Output length: {fullOutput.Length} chars");

                    // Process'in gerçekten bitmesini bekle (timeout: 60 saniye)
                    if (!process.HasExited)
                    {
                        Debug.WriteLine("WinGet process hala çalýþýyor, 60 saniye daha bekliyoruz...");
                        process.WaitForExit(60000);
                    }

                    // Process'i kapat
                    try
                    {
                        process.Close();
                        process.Dispose();
                    }
                    catch { }

                    // Baþarýlý ise biraz daha bekle (kurulum tamamlanmasý için)
                    if (success)
                    {
                        Debug.WriteLine("Kurulum tamamlanmasý için 5 saniye bekliyoruz...");
                        System.Threading.Thread.Sleep(5000);
                    }

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
                    Debug.WriteLine($"WinGet update error: {ex.Message}");
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
