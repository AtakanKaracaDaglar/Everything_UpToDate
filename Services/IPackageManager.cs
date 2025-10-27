using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Everything_UpToDate.Services
{
    /// <summary>
    /// Paket y�neticisi interface'i
    /// T�m paket y�neticileri bu interface'i implement etmelidir
    /// </summary>
    public interface IPackageManager
    {
        /// <summary>
        /// Paket y�neticisinin ad�
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Paket y�neticisi kurulu mu kontrol eder
        /// </summary>
        Task<bool> IsInstalledAsync();

        /// <summary>
        /// G�ncellenebilir uygulamalar� tarar
        /// </summary>
        Task<List<Models.ApplicationInfo>> ScanForUpdatesAsync();

        /// <summary>
        /// Bir uygulamay� g�nceller
        /// </summary>
        Task<bool> UpdateApplicationAsync(Models.ApplicationInfo app, IProgress<UpdateProgress> progress);

        /// <summary>
        /// Birden fazla uygulamay� g�nceller
        /// </summary>
        Task UpdateMultipleApplicationsAsync(List<Models.ApplicationInfo> applications, IProgress<UpdateProgress> progress);
    }
}
