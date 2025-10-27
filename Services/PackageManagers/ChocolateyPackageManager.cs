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
                        ProgressPercentage = 10
                    });

                    var process = new Process
                    {
                        StartInfo = new ProcessStartInfo
                        {
                            FileName = "choco",
                            Arguments = $"upgrade {app.Id} -y",
                            UseShellExecute = false,
                            RedirectStandardOutput = true,
                            RedirectStandardError = true,
                            CreateNoWindow = true
                        }
                    };

                    process.Start();

                    int progressValue = 20;
                    while (!process.HasExited && progressValue < 90)
                    {
                        System.Threading.Thread.Sleep(1000);
                        progressValue += 10;
                        progress?.Report(new UpdateProgress
                        {
                            ApplicationName = app.Name,
                            Status = UpdateStatus.Installing,
                            Message = "Yükleniyor...",
                            ProgressPercentage = Math.Min(progressValue, 90)
                        });
                    }

                    process.WaitForExit();
                    bool success = process.ExitCode == 0;

                    progress?.Report(new UpdateProgress
                    {
                        ApplicationName = app.Name,
                        Status = success ? UpdateStatus.Completed : UpdateStatus.Failed,
                        Message = success ? "Güncelleme tamamlandý" : "Güncelleme baþarýsýz",
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
