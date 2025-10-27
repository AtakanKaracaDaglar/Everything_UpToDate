namespace Everything_UpToDate.Models
{
    /// <summary>
    /// Paket yöneticisi kaynak tipleri
    /// </summary>
    public enum PackageSource
    {
        /// <summary>
        /// Windows Package Manager (WinGet)
        /// </summary>
        WinGet,

        /// <summary>
        /// Chocolatey Package Manager
        /// </summary>
        Chocolatey,

        /// <summary>
        /// Microsoft Store (UWP Apps)
        /// </summary>
        MicrosoftStore,

        /// <summary>
        /// Steam Gaming Platform
        /// </summary>
        Steam,

        /// <summary>
        /// Bilinmeyen kaynak
        /// </summary>
        Unknown
    }
}
