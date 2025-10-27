using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Everything_UpToDate.Models;

namespace Everything_UpToDate.Services.PackageManagers
{
    /// <summary>
    /// Microsoft Store (UWP) Package Manager implementasyonu
    /// PowerShell Get-AppxPackage kullan�r
    /// </summary>
    public class MicrosoftStorePackageManager : IPackageManager
    {
        public string Name => "Microsoft Store";

        public async Task<bool> IsInstalledAsync()
        {
            // Microsoft Store Windows 10/11'de her zaman mevcuttur
            return await Task.FromResult(true);
        }

        public async Task<List<ApplicationInfo>> ScanForUpdatesAsync()
        {
            return await Task.Run(() =>
            {
                var applications = new List<ApplicationInfo>();

                try
                {
                    // PowerShell ile Store uygulamalar�n� listele
                    var process = new Process
                    {
                        StartInfo = new ProcessStartInfo
                        {
                            FileName = "powershell.exe",
                            Arguments = "-Command \"Get-AppxPackage | Where-Object {$_.SignatureKind -eq 'Store'} | Select-Object Name, Version | Format-Table -HideTableHeaders\"",
                            UseShellExecute = false,
                            RedirectStandardOutput = true,
                            RedirectStandardError = true,
                            CreateNoWindow = true
                        }
                    };

                    process.Start();
                    string output = process.StandardOutput.ReadToEnd();
                    process.WaitForExit();

                    var lines = output.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

                    foreach (var line in lines)
                    {
                        if (string.IsNullOrWhiteSpace(line))
                            continue;

                        var app = ParseStoreLine(line);
                        if (app != null)
                        {
                            app.Source = PackageSource.MicrosoftStore;
                            app.LastChecked = DateTime.Now;
                            // Store g�ncellemeleri otomatik oldu�u i�in genelde "g�ncel" g�steririz
                            applications.Add(app);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Microsoft Store scan error: {ex.Message}");
                }

                return applications;
            });
        }

        private ApplicationInfo ParseStoreLine(string line)
        {
            try
            {
                var parts = line.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length >= 2)
                {
                    string name = parts[0];
                    string version = parts[parts.Length - 1];

                    return new ApplicationInfo
                    {
                        Name = name,
                        Id = name,
                        CurrentVersion = version,
                        LatestVersion = version, // Store otomatik g�nceller
                        Description = $"Microsoft Store app: {name}",
                        InstallPath = "Microsoft Store",
                        UpdateSizeBytes = 0,
                        Status = UpdateStatus.UpToDate
                    };
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Store parse error: {ex.Message}");
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
                        Message = "Microsoft Store uygulamalar� otomatik g�ncellenir",
                        ProgressPercentage = 50
                    });

                    // Store uygulamalar� Microsoft taraf�ndan otomatik g�ncellenir
                    // Manuel g�ncelleme ms-windows-store:// protokol� ile Store'u a�abilir
                    System.Threading.Thread.Sleep(1000);

                    progress?.Report(new UpdateProgress
                    {
                        ApplicationName = app.Name,
                        Status = UpdateStatus.Completed,
                        Message = "Store uygulamalar� arka planda otomatik g�ncellenir",
                        ProgressPercentage = 100
                    });

                    return true;
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
            progress?.Report(new UpdateProgress
            {
                ApplicationName = "Microsoft Store",
                Status = UpdateStatus.Completed,
                Message = "Store uygulamalar� otomatik olarak g�ncellenir. Microsoft Store'u a�arak kontrol edebilirsiniz.",
                ProgressPercentage = 100
            });

            await Task.CompletedTask;
        }
    }
}
