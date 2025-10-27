using System;
using System.Drawing;
using System.Windows.Forms;
using Everything_UpToDate.Models;
using Everything_UpToDate.Services;

namespace Everything_UpToDate
{
    public class SettingsDialog : Form
    {
        private SettingsService _settingsService;
        private UpdateService _updateService;
        private StartupService _startupService;
        private DatabaseService _databaseService;

        // Paket Yöneticileri
        private CheckBox chkWinGet;
        private CheckBox chkChocolatey;
        private CheckBox chkMicrosoftStore;

        // Bildirimler
        private CheckBox chkShowNotifications;
        private CheckBox chkMinimizeToTray;

        // Arka Plan
        private CheckBox chkAutoScan;
        private NumericUpDown numScanInterval;

        // Başlangıç
        private CheckBox chkStartWithWindows;
        private CheckBox chkStartMinimized;

        // Butonlar
        private Button btnSave;
        private Button btnCancel;
        private Button btnOpenFolder;

        public SettingsDialog(SettingsService settingsService, UpdateService updateService, 
                             StartupService startupService, DatabaseService databaseService)
        {
            _settingsService = settingsService;
            _updateService = updateService;
            _startupService = startupService;
            _databaseService = databaseService;

            InitializeComponents();
            LoadSettings();
        }

        private void InitializeComponents()
        {
            // Form ayarları
            this.Text = "Ayarlar";
            this.Size = new Size(500, 520);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.White;

            int yPos = 20;

            // ===== PAKET YÖNETİCİLERİ =====
            var lblPackages = new Label
            {
                Text = "PAKET YÖNETİCİLERİ",
                Location = new Point(20, yPos),
                Size = new Size(460, 25),
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 120, 215)
            };
            this.Controls.Add(lblPackages);
            yPos += 30;

            chkWinGet = new CheckBox
            {
                Text = "WinGet (Windows Package Manager)",
                Location = new Point(40, yPos),
                Size = new Size(420, 25),
                Font = new Font("Segoe UI", 9.5F),
                Checked = true
            };
            this.Controls.Add(chkWinGet);
            yPos += 30;

            chkChocolatey = new CheckBox
            {
                Text = "Chocolatey",
                Location = new Point(40, yPos),
                Size = new Size(420, 25),
                Font = new Font("Segoe UI", 9.5F)
            };
            this.Controls.Add(chkChocolatey);
            yPos += 30;

            chkMicrosoftStore = new CheckBox
            {
                Text = "Microsoft Store (UWP Apps)",
                Location = new Point(40, yPos),
                Size = new Size(420, 25),
                Font = new Font("Segoe UI", 9.5F)
            };
            this.Controls.Add(chkMicrosoftStore);
            yPos += 40;

            // ===== BİLDİRİMLER =====
            var lblNotifications = new Label
            {
                Text = "BİLDİRİMLER",
                Location = new Point(20, yPos),
                Size = new Size(460, 25),
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 120, 215)
            };
            this.Controls.Add(lblNotifications);
            yPos += 30;

            chkShowNotifications = new CheckBox
            {
                Text = "Bildirimleri Göster",
                Location = new Point(40, yPos),
                Size = new Size(420, 25),
                Font = new Font("Segoe UI", 9.5F)
            };
            this.Controls.Add(chkShowNotifications);
            yPos += 30;

            chkMinimizeToTray = new CheckBox
            {
                Text = "X'e Basınca Sistem Tepsisine Küçült",
                Location = new Point(40, yPos),
                Size = new Size(420, 25),
                Font = new Font("Segoe UI", 9.5F)
            };
            this.Controls.Add(chkMinimizeToTray);
            yPos += 40;

            // ===== ARKA PLAN TARAMA =====
            var lblBackground = new Label
            {
                Text = "ARKA PLAN TARAMA",
                Location = new Point(20, yPos),
                Size = new Size(460, 25),
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 120, 215)
            };
            this.Controls.Add(lblBackground);
            yPos += 30;

            chkAutoScan = new CheckBox
            {
                Text = "Otomatik Tarama Aktif",
                Location = new Point(40, yPos),
                Size = new Size(200, 25),
                Font = new Font("Segoe UI", 9.5F)
            };
            this.Controls.Add(chkAutoScan);

            var lblInterval = new Label
            {
                Text = "Tarama Aralığı (saat):",
                Location = new Point(250, yPos + 2),
                Size = new Size(130, 20),
                Font = new Font("Segoe UI", 9F)
            };
            this.Controls.Add(lblInterval);

            numScanInterval = new NumericUpDown
            {
                Location = new Point(380, yPos),
                Size = new Size(60, 25),
                Minimum = 1,
                Maximum = 24,
                Value = 6,
                Font = new Font("Segoe UI", 9.5F)
            };
            this.Controls.Add(numScanInterval);
            yPos += 40;

            // ===== BAŞLANGIÇ =====
            var lblStartup = new Label
            {
                Text = "BAŞLANGIÇ",
                Location = new Point(20, yPos),
                Size = new Size(460, 25),
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 120, 215)
            };
            this.Controls.Add(lblStartup);
            yPos += 30;

            chkStartWithWindows = new CheckBox
            {
                Text = "Windows ile Başlat",
                Location = new Point(40, yPos),
                Size = new Size(420, 25),
                Font = new Font("Segoe UI", 9.5F)
            };
            this.Controls.Add(chkStartWithWindows);
            yPos += 30;

            chkStartMinimized = new CheckBox
            {
                Text = "Küçültülmüş Başlat",
                Location = new Point(40, yPos),
                Size = new Size(420, 25),
                Font = new Font("Segoe UI", 9.5F)
            };
            this.Controls.Add(chkStartMinimized);
            yPos += 50;

            // ===== BUTONLAR =====
            btnOpenFolder = new Button
            {
                Text = "Ayar Klasörünü Aç",
                Location = new Point(20, yPos),
                Size = new Size(150, 35),
                Font = new Font("Segoe UI", 9F),
                BackColor = Color.FromArgb(100, 100, 100),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnOpenFolder.Click += BtnOpenFolder_Click;
            this.Controls.Add(btnOpenFolder);

            btnCancel = new Button
            {
                Text = "İptal",
                Location = new Point(250, yPos),
                Size = new Size(100, 35),
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                BackColor = Color.FromArgb(150, 150, 150),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                DialogResult = DialogResult.Cancel
            };
            this.Controls.Add(btnCancel);

            btnSave = new Button
            {
                Text = "Kaydet",
                Location = new Point(360, yPos),
                Size = new Size(100, 35),
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                BackColor = Color.FromArgb(0, 158, 115),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnSave.Click += BtnSave_Click;
            this.Controls.Add(btnSave);
        }

        private void LoadSettings()
        {
            var settings = _settingsService.Settings;

            // Paket yöneticileri
            chkWinGet.Checked = _updateService.IsSourceEnabled(PackageSource.WinGet);
            chkChocolatey.Checked = _updateService.IsSourceEnabled(PackageSource.Chocolatey);
            chkMicrosoftStore.Checked = _updateService.IsSourceEnabled(PackageSource.MicrosoftStore);

            // Bildirimler
            chkShowNotifications.Checked = settings.ShowNotifications;
            chkMinimizeToTray.Checked = settings.MinimizeToTray;

            // Arka plan
            chkAutoScan.Checked = settings.AutoScanEnabled;
            numScanInterval.Value = settings.AutoScanIntervalHours;

            // Başlangıç
            chkStartWithWindows.Checked = _startupService.IsStartupEnabled();
            chkStartMinimized.Checked = settings.StartMinimized;
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            // Paket yöneticileri
            _updateService.SetSourceEnabled(PackageSource.WinGet, chkWinGet.Checked);
            _updateService.SetSourceEnabled(PackageSource.Chocolatey, chkChocolatey.Checked);
            _updateService.SetSourceEnabled(PackageSource.MicrosoftStore, chkMicrosoftStore.Checked);

            // Bildirimler
            _settingsService.UpdateSetting("ShowNotifications", chkShowNotifications.Checked);
            _settingsService.UpdateSetting("MinimizeToTray", chkMinimizeToTray.Checked);

            // Arka plan
            _settingsService.UpdateSetting("AutoScanEnabled", chkAutoScan.Checked);
            _settingsService.UpdateSetting("AutoScanIntervalHours", (int)numScanInterval.Value);

            // Başlangıç
            if (chkStartWithWindows.Checked)
                _startupService.EnableStartup();
            else
                _startupService.DisableStartup();

            _settingsService.UpdateSetting("StartMinimized", chkStartMinimized.Checked);

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void BtnOpenFolder_Click(object sender, EventArgs e)
        {
            string path = System.IO.Path.GetDirectoryName(_settingsService.GetSettingsFilePath());
            System.Diagnostics.Process.Start("explorer.exe", path);
        }
    }
}
