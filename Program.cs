using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Security.Principal;

namespace Everything_UpToDate
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Admin izni kontrol et
            if (!IsRunningAsAdmin())
            {
                // Admin olarak yeniden başlat
                var psi = new ProcessStartInfo
                {
                    FileName = Application.ExecutablePath,
                    UseShellExecute = true,
                    Verb = "runas" // Admin olarak çalıştır
                };

                try
                {
                    Process.Start(psi);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        "Program admin yetkisiyle çalıştırılamadı!\n\n" +
                        "Lütfen uygulamayı sağ tıklayarak 'Yönetici olarak çalıştır' seçeneğini kullanın.\n\n" +
                        $"Hata: {ex.Message}",
                        "Admin Gerekli",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }

                Application.Exit();
                return;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }

        /// <summary>
        /// Program admin yetkisiyle çalışıyor mu kontrol et
        /// </summary>
        private static bool IsRunningAsAdmin()
        {
            try
            {
                WindowsIdentity identity = WindowsIdentity.GetCurrent();
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                return principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
            catch
            {
                return false;
            }
        }
    }
}
