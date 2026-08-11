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
    /// Chocolatey Package Manager implementasyonu
    /// </summary>
    public class ChocolateyPackageManager : IPackageManager
    {
        public string Name => "Chocolatey";

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
                            FileName = "choco",
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
                            FileName = "choco",
                            Arguments = "outdated -r",
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

                    // Chocolatey çýktý formatý: PackageName|CurrentVersion|AvailableVersion|Pinned
                    var lines = output.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

                    foreach (var line in lines)
                    {
                        if (string.IsNullOrWhiteSpace(line) || line.StartsWith("Chocolatey"))
                            continue;

                        var app = ParseChocolateyLine(line);
                        if (app != null)
                        {
                            app.Source = PackageSource.Chocolatey;
                            app.LastChecked = DateTime.Now;
                            applications.Add(app);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Chocolatey scan error: {ex.Message}");
                }

                return applications;
            });
        }

        private ApplicationInfo ParseChocolateyLine(string line)
        {
            try
            {
                // Format: PackageName|CurrentVersion|AvailableVersion|Pinned
                var parts = line.Split('|');

                if (parts.Length >= 3)
                {
                    return new ApplicationInfo
                    {
                        Name = parts[0].Trim(),
                        Id = parts[0].Trim().ToLower(),
                        CurrentVersion = parts[1].Trim(),
                        LatestVersion = parts[2].Trim(),
                        Description = $"Chocolatey package: {parts[0]}",
                        InstallPath = "Chocolatey Package",
                        UpdateSizeBytes = 0,
                        Status = UpdateStatus.Idle
                    };
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Chocolatey parse error: {ex.Message} - Line: {line}");
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
                        Message = "Chocolatey güncelleme baþlatýlýyor...",
                        ProgressPercentage = 5
                    });

                    var upgradeProcess = new Process
                    {
                        StartInfo = new ProcessStartInfo
                        {
                            FileName = "choco",
                            Arguments = $"upgrade {app.Id} -y --force --no-progress",
                            UseShellExecute = false,
                            RedirectStandardOutput = true,
                            RedirectStandardError = true,
                            CreateNoWindow = true,
                            StandardOutputEncoding = System.Text.Encoding.UTF8
                        }
                    };

                    upgradeProcess.Start();
                    var outputBuilder = new System.Text.StringBuilder();
                    var errorBuilder = new System.Text.StringBuilder();
                    int lastProgress = 20;

                    upgradeProcess.OutputDataReceived += (sender, e) =>
                    {
                        if (!string.IsNullOrEmpty(e.Data))
                        {
                            outputBuilder.AppendLine(e.Data);
                            var line = e.Data.Trim();

                            Debug.WriteLine($"[Choco Upgrade] {e.Data}");

                            // Progress güncelle
                            UpdateStatus status = UpdateStatus.Installing;
                            int progressPercent = lastProgress;
                            string displayMessage = line;

                            if (line.Contains("Downloading") || line.Contains("Downloading package"))
                            {
                                status = UpdateStatus.Downloading;
                                progressPercent = 40;
                                displayMessage = "Ýndiriliyor...";
                            }
                            else if (line.Contains("Installing") || line.Contains("Installed"))
                            {
                                status = UpdateStatus.Installing;
                                progressPercent = 80;
                                displayMessage = "Yükleniyor...";
                            }
                            else if (line.Contains("completed successfully") || line.Contains("tamamlandý"))
                            {
                                status = UpdateStatus.Installing;
                                progressPercent = 95;
                                displayMessage = "Yükleme tamamlanýyor...";
                            }

                            lastProgress = Math.Max(lastProgress, progressPercent);

                            progress?.Report(new UpdateProgress
                            {
                                ApplicationName = app.Name,
                                Status = status,
                                Message = displayMessage,
                                ProgressPercentage = Math.Min(lastProgress, 99)
                            });
                        }
                    };

                    upgradeProcess.ErrorDataReceived += (sender, e) =>
                    {
                        if (!string.IsNullOrEmpty(e.Data))
                        {
                            errorBuilder.AppendLine(e.Data);
                            Debug.WriteLine($"[Choco Error] {e.Data}");
                        }
                    };

                    upgradeProcess.BeginOutputReadLine();
                    upgradeProcess.BeginErrorReadLine();
                    upgradeProcess.WaitForExit();

                    bool success = upgradeProcess.ExitCode == 0;
                    string fullOutput = outputBuilder.ToString();
                    string fullError = errorBuilder.ToString();

                    // Success indications (Chocolatey baþarýlý olmuþ göstergeleri)
                    bool hasSuccessIndicator = fullOutput.Contains("Upgrade of ") ||
                                               fullOutput.Contains("installed") ||
                                               fullOutput.Contains("successfully");

                    // Failure indicators (Baþarýsýz olmuþ göstergeleri)
                    bool hasFailureIndicator = fullOutput.Contains("No applicable upgrade found") ||
                                               fullOutput.Contains("cannot be upgraded") ||
                                               fullOutput.Contains("does not apply to your system") ||
                                               fullOutput.Contains("Exit code of") ||
                                               fullOutput.Contains("Permission denied") ||
                                               fullOutput.Contains("Access is denied");

                    // Hata kodu negatifse sistem hatasý
                    if (upgradeProcess.ExitCode < 0)
                    {
                        Debug.WriteLine($"Chocolatey negative exit code detected: {upgradeProcess.ExitCode}");
                        Debug.WriteLine($"Output: {fullOutput}");
                        Debug.WriteLine($"Error: {fullError}");

                        // Eðer output'ta baþarý göstergesi varsa baþarýlý say
                        if (hasSuccessIndicator)
                        {
                            success = true;
                        }
                        else if (hasFailureIndicator)
                        {
                            success = false;
                        }
                    }

                    // Fallback: Açýk baþarý veya baþarýsýzlýk kontrolü
                    if (!success && hasSuccessIndicator)
                    {
                        success = true;
                    }
                    else if (success && hasFailureIndicator)
                    {
                        success = false;
                    }

                    Debug.WriteLine($"Chocolatey final result: {(success ? "SUCCESS" : "FAILED")}");
                    Debug.WriteLine($"Output length: {fullOutput.Length} chars");

                    // Process'in gerçekten bitmesini bekle (timeout: 60 saniye)
                    if (!upgradeProcess.HasExited)
                    {
                        Debug.WriteLine("Process hala çalýþýyor, 60 saniye daha bekliyoruz...");
                        upgradeProcess.WaitForExit(60000);
                    }

                    // Process'i kapat
                    try
                    {
                        upgradeProcess.Close();
                        upgradeProcess.Dispose();
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
                        Message = success ? "? Güncelleme tamamlandý!" : $"? Güncelleme baþarýsýz",
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
                    Debug.WriteLine($"Chocolatey update error: {ex.Message}\n{ex.StackTrace}");
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
