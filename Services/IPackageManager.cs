using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Everything_UpToDate.Services
{
    /// <summary>
    /// Paket yöneticisi interface'i
    /// Tüm paket yöneticileri bu interface'i implement etmelidir
    /// </summary>
    public interface IPackageManager
    {
        /// <summary>
        /// Paket yöneticisinin adý
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Paket yöneticisi kurulu mu kontrol eder
        /// </summary>
        Task<bool> IsInstalledAsync();

        /// <summary>
        /// Güncellenebilir uygulamalarý tarar
        /// </summary>
        Task<List<Models.ApplicationInfo>> ScanForUpdatesAsync();

        /// <summary>
        /// Bir uygulamayý günceller
        /// </summary>
        Task<bool> UpdateApplicationAsync(Models.ApplicationInfo app, IProgress<UpdateProgress> progress);

        /// <summary>
        /// Birden fazla uygulamayý günceller
        /// </summary>
        Task UpdateMultipleApplicationsAsync(List<Models.ApplicationInfo> applications, IProgress<UpdateProgress> progress);
    }
}
