using System;
using System.Diagnostics;
using Microsoft.Win32;

namespace Everything_UpToDate.Services
{
    /// <summary>
    /// Windows ba�lang�� ayarlar�n� y�neten servis
    /// </summary>
    public class StartupService
    {
        private const string APP_NAME = "EverythingUpToDate";
        private readonly string _appPath;

        public StartupService()
        {
            _appPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
        }

        /// <summary>
        /// Windows ile ba�latmay� etkinle�tir
        /// </summary>
        public bool EnableStartup()
        {
            try
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true))
                {
                    if (key != null)
                    {
                        key.SetValue(APP_NAME, $"\"{_appPath}\" --minimized");
                        Debug.WriteLine("Startup enabled successfully");
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to enable startup: {ex.Message}");
            }

            return false;
        }

        /// <summary>
        /// Windows ile ba�latmay� devre d��� b�rak
        /// </summary>
        public bool DisableStartup()
        {
            try
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true))
                {
                    if (key != null)
                    {
                        if (key.GetValue(APP_NAME) != null)
                        {
                            key.DeleteValue(APP_NAME);
                        }
                        Debug.WriteLine("Startup disabled successfully");
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to disable startup: {ex.Message}");
            }

            return false;
        }

        /// <summary>
        /// Windows ile ba�latma durumunu kontrol et
        /// </summary>
        public bool IsStartupEnabled()
        {
            try
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", false))
                {
                    if (key != null)
                    {
                        object value = key.GetValue(APP_NAME);
                        return value != null;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to check startup status: {ex.Message}");
            }

            return false;
        }

        /// <summary>
        /// Ba�lang�� durumunu ayarla
        /// </summary>
        public bool SetStartup(bool enabled)
        {
            return enabled ? EnableStartup() : DisableStartup();
        }
    }
}
