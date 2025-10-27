using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Everything_UpToDate.Models;
using Everything_UpToDate.Services;

namespace Everything_UpToDate
{
    public partial class Form1 : Form
    {
        private readonly UpdateService _updateService;
        private readonly ThemeService _themeService;
        private readonly SettingsService _settingsService;
        private readonly NotificationService _notificationService;
        private readonly BackgroundScanService _backgroundScanService;
        private readonly StartupService _startupService;
        private readonly DatabaseService _databaseService;
        
        private List<ApplicationInfo> _applications;
        private List<ApplicationInfo> _filteredApplications;
        private bool _isUpdating = false;
        private int _sortColumn = -1;
        private bool _isClosing = false;
        private bool _hasShownTrayMessage = false;

        public Form1()
        {
            InitializeComponent();
            
            _updateService = new UpdateService();
            _themeService = new ThemeService();
            _settingsService = new SettingsService();
            _databaseService = new DatabaseService();
            _notificationService = new NotificationService(_settingsService);
            _backgroundScanService = new BackgroundScanService(_updateService, _notificationService, _settingsService);
            _startupService = new StartupService();
            
            // Database'i UpdateService'e bağla
            _updateService.SetDatabaseService(_databaseService);
            
            _applications = new List<ApplicationInfo>();
            _filteredApplications = new List<ApplicationInfo>();

            // Event handlers
            _notificationService.OnShowMainWindow += (s, e) => ShowMainWindow();
            _notificationService.OnScanRequested += async (s, e) => await RefreshApplicationsAsync();
            _notificationService.OnExitRequested += (s, e) => ExitApplication();
            _backgroundScanService.OnScanCompleted += BackgroundScanService_OnScanCompleted;

            // Form close event
            this.FormClosing += Form1_FormClosing;
        }

        private void btnSettings_Click(object sender, EventArgs e)
        {
            // Ayarlar DialogBox'ı
            var settings = _settingsService.Settings;

            var settingsForm = new SettingsDialog(_settingsService, _updateService, _startupService, _databaseService);
            settingsForm.ShowDialog(this);

            // Ayarlar kaydedildi notification
            if (settingsForm.DialogResult == DialogResult.OK)
            {
                MessageBox.Show("Ayarlar kaydedildi!", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void BackgroundScanService_OnScanCompleted(object sender, int updateCount)
        {
            // UI thread'de çalıştır
            if (InvokeRequired)
            {
                Invoke(new Action<object, int>(BackgroundScanService_OnScanCompleted), sender, updateCount);
                return;
            }

            // Eğer form görünürse listeyi güncelle
            if (this.Visible && !this.WindowState.Equals(FormWindowState.Minimized))
            {
                RefreshApplicationsAsync().ConfigureAwait(false);
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_isClosing)
            {
                // Gerçekten çıkıyoruz
                return;
            }

            if (_settingsService.Settings.MinimizeToTray)
            {
                // Minimize to tray ayarı açıksa direk gizle
                e.Cancel = true;
                MinimizeToTray();
            }
            else
            {
                // Kullanıcıya sor
                var result = MessageBox.Show(
                    "Ne yapmak istersiniz?\n\n" +
                    "• EVET: Uygulamadan tamamen çık\n" +
                    "• HAYIR: Sistem tepsisine küçült (arka planda çalışmaya devam eder)\n" +
                    "• İPTAL: Hiçbir şey yapma",
                    "Çıkış Seçenekleri",
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Question);

                switch (result)
                {
                    case DialogResult.Yes:
                        // Tamamen çık
                        ExitApplication();
                        break;
                    case DialogResult.No:
                        // Tray'e küçült
                        e.Cancel = true;
                        MinimizeToTray();
                        break;
                    case DialogResult.Cancel:
                        // Hiçbir şey yapma
                        e.Cancel = true;
                        break;
                }
            }
        }

        private void MinimizeToTray()
        {
            this.Hide();
            _notificationService.Show();

            // İlk kez tray'e gidiyorsa bildirim göster
            if (!_hasShownTrayMessage)
            {
                _notificationService.ShowBalloonTip(
                    "Everything UpToDate",
                    "Uygulama arka planda çalışmaya devam ediyor. Çift tıklayarak açabilirsiniz.",
                    ToolTipIcon.Info,
                    3000
                );
                _hasShownTrayMessage = true;
            }
        }

        private void ShowMainWindow()
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
            this.BringToFront();
            this.Activate();
        }

        private void ExitApplication()
        {
            _isClosing = true;
            _backgroundScanService.Stop();
            _notificationService.Hide();
            Application.Exit();
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            // Ayarları yükle ve tema uygula
            LoadSettings();

            // Minimize başlatma kontrolü
            string[] args = Environment.GetCommandLineArgs();
            bool startMinimized = _settingsService.Settings.StartMinimized || 
                                 Array.IndexOf(args, "--minimized") >= 0;

            if (startMinimized)
            {
                this.WindowState = FormWindowState.Minimized;
                this.ShowInTaskbar = false;
                MinimizeToTray();
            }

            // Arka plan taramayı başlat
            _backgroundScanService.Start();

            // WinGet kontrolü yap
            bool wingetAvailable = await _updateService.CheckWinGetAvailabilityAsync();
            
            if (!wingetAvailable)
            {
                var result = MessageBox.Show(
                    "Windows Package Manager (WinGet) sisteminizde bulunamadı!\n\n" +
                    "WinGet, uygulamaları otomatik olarak taramak ve güncellemek için gereklidir.\n\n" +
                    "Çözüm:\n" +
                    "1. Microsoft Store'u açın\n" +
                    "2. 'App Installer' uygulamasını arayın ve yükleyin\n" +
                    "3. Bu programı yeniden başlatın\n\n" +
                    "Yine de devam etmek istiyor musunuz? (Demo modu aktif olacak)",
                    "WinGet Bulunamadı",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (result == DialogResult.No)
                {
                    Application.Exit();
                    return;
                }
            }

            // Form görünürse otomatik tarama yap
            if (!startMinimized)
            {
                await RefreshApplicationsAsync();
            }
        }

        private void LoadSettings()
        {
            var settings = _settingsService.Settings;

            // Tema uygula
            if (settings.Theme == "Dark")
            {
                _themeService.CurrentTheme = ThemeService.Theme.Dark;
                btnToggleTheme.Text = "☀️ Light Mode";
            }
            else
            {
                _themeService.CurrentTheme = ThemeService.Theme.Light;
                btnToggleTheme.Text = "🌙 Dark Mode";
            }

            _themeService.ApplyTheme(this);

            // Son tarama zamanını göster
            if (settings.LastScanTime != DateTime.MinValue)
            {
                lblLastScan.Text = $"Son tarama: {settings.LastScanTime:dd.MM.yyyy HH:mm}";
            }
        }

        private void btnToggleTheme_Click(object sender, EventArgs e)
        {
            _themeService.ToggleTheme();

            if (_themeService.CurrentTheme == ThemeService.Theme.Dark)
            {
                btnToggleTheme.Text = "☀️ Light Mode";
                _settingsService.UpdateSetting("Theme", "Dark");
            }
            else
            {
                btnToggleTheme.Text = "🌙 Dark Mode";
                _settingsService.UpdateSetting("Theme", "Light");
            }

            _themeService.ApplyTheme(this);

            // ListView itemlerini yeniden renklendir
            UpdateListViewItemColors();
        }

        private void UpdateListViewItemColors()
        {
            foreach (ListViewItem item in listViewApps.Items)
            {
                var app = item.Tag as ApplicationInfo;
                if (app != null)
                {
                    if (app.IsUpdateAvailable)
                    {
                        item.BackColor = _themeService.GetColor("updateavailable");
                        item.ForeColor = _themeService.GetColor("foreground");
                    }
                    else
                    {
                        item.BackColor = _themeService.GetColor("uptodate");
                        item.ForeColor = _themeService.GetColor("foreground");
                    }
                }
            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            FilterApplications();
        }

        private void FilterApplications()
        {
            string searchText = txtSearch.Text.ToLower();

            if (string.IsNullOrWhiteSpace(searchText))
            {
                _filteredApplications = new List<ApplicationInfo>(_applications);
            }
            else
            {
                _filteredApplications = _applications
                    .Where(a => a.Name.ToLower().Contains(searchText) || 
                                a.Id.ToLower().Contains(searchText))
                    .ToList();
            }

            PopulateListView();
        }

        private void listViewApps_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            // Sütuna göre sıralama
            if (e.Column == _sortColumn)
            {
                // Aynı sütuna tıklandıysa tersine çevir
                _filteredApplications.Reverse();
            }
            else
            {
                // Yeni sütuna göre sırala
                _sortColumn = e.Column;

                switch (_sortColumn)
                {
                    case 0: // Name
                        _filteredApplications = _filteredApplications.OrderBy(a => a.Name).ToList();
                        break;
                    case 1: // Current Version
                        _filteredApplications = _filteredApplications.OrderBy(a => a.CurrentVersion).ToList();
                        break;
                    case 2: // Latest Version
                        _filteredApplications = _filteredApplications.OrderBy(a => a.LatestVersion).ToList();
                        break;
                    case 3: // Status
                        _filteredApplications = _filteredApplications.OrderBy(a => a.IsUpdateAvailable).ToList();
                        break;
                    case 4: // Size
                        _filteredApplications = _filteredApplications.OrderBy(a => a.UpdateSizeBytes).ToList();
                        break;
                }
            }

            PopulateListView();
        }

        private async void btnRefresh_Click(object sender, EventArgs e)
        {
            await RefreshApplicationsAsync();
        }

        private async Task RefreshApplicationsAsync()
        {
            try
            {
                SetUIState(false);
                lblStatus.Text = "Uygulamalar taranıyor...";
                lblStatus.ForeColor = _themeService.GetColor("accent");
                progressBar.Style = ProgressBarStyle.Marquee;

                _applications = await _updateService.ScanForApplicationsAsync();
                _filteredApplications = new List<ApplicationInfo>(_applications);
                
                PopulateListView();
                UpdateCountLabel();

                // Son tarama zamanını güncelle
                _settingsService.UpdateSetting("LastScanTime", DateTime.Now);
                lblLastScan.Text = $"Son tarama: {DateTime.Now:dd.MM.yyyy HH:mm}";

                lblStatus.Text = $"Tarama tamamlandı - {_applications.Count} uygulama bulundu";
                lblStatus.ForeColor = _themeService.GetColor("success");
                progressBar.Style = ProgressBarStyle.Continuous;
                progressBar.Value = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Tarama sırasında hata: {ex.Message}", "Hata", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                lblStatus.Text = "Tarama başarısız";
                lblStatus.ForeColor = _themeService.GetColor("error");
            }
            finally
            {
                SetUIState(true);
            }
        }

        private void PopulateListView()
        {
            listViewApps.Items.Clear();

            var appsToShow = _filteredApplications.Count > 0 ? _filteredApplications : _applications;

            foreach (var app in appsToShow)
            {
                var item = new ListViewItem(app.Name);
                item.SubItems.Add(app.CurrentVersion);
                item.SubItems.Add(app.LatestVersion);
                
                string statusText = app.IsUpdateAvailable ? "Güncelleme Mevcut ⚠" : "Güncel ✓";
                item.SubItems.Add(statusText);
                
                // Kaynak bilgisini göster (sadece isim)
                item.SubItems.Add(app.SourceName);
                
                item.Tag = app;

                // Güncelleme varsa renklendirme (tema duyarlı)
                if (app.IsUpdateAvailable)
                {
                    item.BackColor = _themeService.GetColor("updateavailable");
                    item.ForeColor = _themeService.GetColor("foreground");
                }
                else
                {
                    item.BackColor = _themeService.GetColor("uptodate");
                    item.ForeColor = _themeService.GetColor("foreground");
                }

                listViewApps.Items.Add(item);
            }

            // Arama sonucu bilgisi
            if (!string.IsNullOrWhiteSpace(txtSearch.Text))
            {
                lblStatus.Text = $"{appsToShow.Count} uygulama bulundu ('{txtSearch.Text}' araması)";
            }
        }

        private async void btnUpdateSelected_Click(object sender, EventArgs e)
        {
            var selectedApps = GetSelectedApplications();
            
            if (selectedApps.Count == 0)
            {
                MessageBox.Show("Lütfen güncellenecek uygulamaları seçin!", "Uyarı", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var updatableApps = selectedApps.Where(a => a.IsUpdateAvailable).ToList();
            if (updatableApps.Count == 0)
            {
                MessageBox.Show("Seçili uygulamaların hepsi güncel!", "Bilgi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var result = MessageBox.Show(
                $"{updatableApps.Count} uygulama güncellenecek. Devam etmek istiyor musunuz?",
                "Onay",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                await UpdateApplicationsAsync(updatableApps);
            }
        }

        private async void btnUpdateAll_Click(object sender, EventArgs e)
        {
            var updatableApps = _applications.Where(a => a.IsUpdateAvailable).ToList();

            if (updatableApps.Count == 0)
            {
                MessageBox.Show("Tüm uygulamalar güncel!", "Bilgi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var result = MessageBox.Show(
                $"Toplam {updatableApps.Count} uygulama güncellenecek. Bu işlem biraz zaman alabilir.\n\nDevam etmek istiyor musunuz?",
                "Toplu Güncelleme",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                await UpdateApplicationsAsync(updatableApps);
            }
        }

        private async Task UpdateApplicationsAsync(List<ApplicationInfo> applications)
        {
            if (_isUpdating) return;

            try
            {
                _isUpdating = true;
                SetUIState(false);
                
                // Detay textbox'ını temizle
                txtDetails.Clear();
                txtDetails.AppendText($"═══════════════════════════════════════\r\n");
                txtDetails.AppendText($"  GÜNCELLEME BAŞLADI\r\n");
                txtDetails.AppendText($"  {applications.Count} uygulama güncellenecek\r\n");
                txtDetails.AppendText($"═══════════════════════════════════════\r\n\r\n");

                var progress = new Progress<UpdateProgress>(UpdateProgressHandler);

                if (applications.Count == 1)
                {
                    await _updateService.UpdateApplicationAsync(applications[0], progress);
                }
                else
                {
                    await _updateService.UpdateMultipleApplicationsAsync(applications, progress);
                }

                txtDetails.AppendText($"\r\n═══════════════════════════════════════\r\n");
                txtDetails.AppendText($"  TÜM GÜNCELLEMELER TAMAMLANDI ✓\r\n");
                txtDetails.AppendText($"═══════════════════════════════════════\r\n");
                txtDetails.AppendText($"\r\nYeniden taranıyor...\r\n");

                // Güncelleme sonrası yeniden tarama yap
                await RefreshApplicationsAsync();

                MessageBox.Show(
                    $"Güncelleme tamamlandı!\n\n{applications.Count} uygulama güncellendi.\n\nUygulama listesi yenilendi.",
                    "Başarılı",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                lblStatus.Text = "Tüm güncellemeler tamamlandı ✓";
                lblStatus.ForeColor = _themeService.GetColor("success");
            }
            catch (Exception ex)
            {
                txtDetails.AppendText($"\r\n[HATA] {ex.Message}\r\n");
                MessageBox.Show($"Güncelleme sırasında hata: {ex.Message}", "Hata", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                lblStatus.Text = "Güncelleme başarısız";
                lblStatus.ForeColor = _themeService.GetColor("error");
            }
            finally
            {
                _isUpdating = false;
                SetUIState(true);
                progressBar.Value = 0;
            }
        }

        private void UpdateProgressHandler(UpdateProgress progress)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<UpdateProgress>(UpdateProgressHandler), progress);
                return;
            }

            progressBar.Value = Math.Min(progress.ProgressPercentage, 100);
            lblStatus.Text = $"{progress.ApplicationName}: {progress.Message}";

            // Detay textbox'ına da yaz
            if (!string.IsNullOrWhiteSpace(progress.Message))
            {
                txtDetails.AppendText($"[{DateTime.Now:HH:mm:ss}] {progress.Message}\r\n");
                txtDetails.SelectionStart = txtDetails.Text.Length;
                txtDetails.ScrollToCaret();
            }

            // Durum rengini ayarla (tema duyarlı)
            switch (progress.Status)
            {
                case UpdateStatus.Checking:
                    lblStatus.ForeColor = _themeService.GetColor("accent");
                    break;
                case UpdateStatus.Downloading:
                    lblStatus.ForeColor = _themeService.GetColor("warning");
                    break;
                case UpdateStatus.Installing:
                    lblStatus.ForeColor = Color.FromArgb(138, 43, 226);
                    break;
                case UpdateStatus.Completed:
                    lblStatus.ForeColor = _themeService.GetColor("success");
                    break;
                case UpdateStatus.Failed:
                    lblStatus.ForeColor = _themeService.GetColor("error");
                    break;
            }

            // ListView'deki ilgili öğeyi güncelle
            UpdateListViewItem(progress);
        }

        private void UpdateListViewItem(UpdateProgress progress)
        {
            foreach (ListViewItem item in listViewApps.Items)
            {
                var app = item.Tag as ApplicationInfo;
                if (app != null && app.Name == progress.ApplicationName)
                {
                    item.SubItems[3].Text = GetStatusText(progress.Status, progress.ProgressPercentage);
                    
                    // Tamamlandığında rengi değiştir
                    if (progress.Status == UpdateStatus.Completed)
                    {
                        item.BackColor = _themeService.GetColor("uptodate");
                        item.ForeColor = _themeService.GetColor("foreground");
                        item.SubItems[1].Text = app.CurrentVersion;
                        item.SubItems[4].Text = "-";
                    }
                    break;
                }
            }
        }

        private string GetStatusText(UpdateStatus status, int percentage)
        {
            switch (status)
            {
                case UpdateStatus.Checking:
                    return "Kontrol ediliyor...";
                case UpdateStatus.Downloading:
                    return $"İndiriliyor... {percentage}%";
                case UpdateStatus.Installing:
                    return $"Kuruluyor... {percentage}%";
                case UpdateStatus.Completed:
                    return "Tamamlandı ✓";
                case UpdateStatus.Failed:
                    return "Başarısız ✗";
                case UpdateStatus.UpToDate:
                    return "Güncel ✓";
                default:
                    return "Güncelleme Mevcut ⚠";
            }
        }

        private List<ApplicationInfo> GetSelectedApplications()
        {
            var selected = new List<ApplicationInfo>();
            
            foreach (ListViewItem item in listViewApps.CheckedItems)
            {
                if (item.Tag is ApplicationInfo app)
                {
                    selected.Add(app);
                }
            }

            return selected;
        }

        private void listViewApps_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listViewApps.SelectedItems.Count > 0)
            {
                var item = listViewApps.SelectedItems[0];
                var app = item.Tag as ApplicationInfo;

                if (app != null)
                {
                    txtDetails.Text = $"═══════════════════════════════════════════════════════════════\r\n" +
                                    $"  UYGULAMA BİLGİLERİ\r\n" +
                                    $"═══════════════════════════════════════════════════════════════\r\n\r\n" +
                                    $"📦 Uygulama Adı      : {app.Name}\r\n" +
                                    $"🔖 Paket ID          : {app.Id}\r\n\r\n" +
                                    $"📌 Mevcut Versiyon   : {app.CurrentVersion}\r\n" +
                                    $"🆕 Yeni Versiyon     : {app.LatestVersion}\r\n\r\n" +
                                    $"Kaynak               : {app.SourceName}\r\n" +
                                    $"📁 Kurulum Yolu      : {app.InstallPath}\r\n\r\n" +
                                    $"🔍 Son Kontrol       : {app.LastChecked:dd.MM.yyyy HH:mm:ss}\r\n\r\n" +
                                    $"───────────────────────────────────────────────────────────────\r\n" +
                                    $"  AÇIKLAMA\r\n" +
                                    $"───────────────────────────────────────────────────────────────\r\n\r\n" +
                                    $"{app.Description}\r\n\r\n" +
                                    $"═══════════════════════════════════════════════════════════════";
                }
            }
        }

        private void SetUIState(bool enabled)
        {
            btnRefresh.Enabled = enabled;
            btnUpdateSelected.Enabled = enabled;
            btnUpdateAll.Enabled = enabled;
            listViewApps.Enabled = enabled;
            txtSearch.Enabled = enabled;
            // Theme toggle her zaman aktif kalmalı
            // btnToggleTheme.Enabled = enabled; // KALDIRILDI!
        }

        private void UpdateCountLabel()
        {
            int updateCount = _applications.Count(a => a.IsUpdateAvailable);
            lblUpdateCount.Text = $"{updateCount} güncelleme mevcut";
            lblUpdateCount.ForeColor = updateCount > 0 ? _themeService.GetColor("error") : _themeService.GetColor("success");
        }

        private void btnViewHistory_Click(object sender, EventArgs e)
        {
            // Geçmişi basit MessageBox'ta göster (geçici)
            var history = _databaseService.GetAllHistory();
            var stats = _databaseService.GetStatistics();

            var sb = new System.Text.StringBuilder();
            sb.AppendLine("═══════════════════════════════════════");
            sb.AppendLine("  GÜNCELLEME GEÇMİŞİ VE İSTATİSTİKLER");
            sb.AppendLine("═══════════════════════════════════════\n");
            
            sb.AppendLine($"📊 GENEL İSTATİSTİKLER:");
            sb.AppendLine($"   Toplam Güncelleme: {stats.TotalUpdates}");
            sb.AppendLine($"   ✅ Başarılı: {stats.SuccessfulUpdates}");
            sb.AppendLine($"   ❌ Başarısız: {stats.FailedUpdates}");
            
            if (stats.TotalBytes > 0)
            {
                double totalMB = stats.TotalBytes / (1024.0 * 1024.0);
                sb.AppendLine($"   💾 Toplam İndirilen: {totalMB:F2} MB");
            }
            
            sb.AppendLine();

            if (history.Count > 0)
            {
                sb.AppendLine($"📋 SON 10 GÜNCELLEME:");
                sb.AppendLine();

                var recentHistory = _databaseService.GetRecentHistory(10);
                foreach (var entry in recentHistory)
                {
                    string status = entry.Success ? "✅" : "❌";
                    sb.AppendLine($"{status} {entry.AppName}");
                    sb.AppendLine($"   {entry.FromVersion} → {entry.ToVersion}");
                    sb.AppendLine($"   {entry.UpdateDate:dd.MM.yyyy HH:mm} - {entry.GetFormattedSize()}");
                    if (!entry.Success)
                    {
                        sb.AppendLine($"   Hata: {entry.ErrorMessage}");
                    }
                    sb.AppendLine();
                }

                // En çok güncellenen uygulamalar
                var topApps = _databaseService.GetMostUpdatedApps(5);
                if (topApps.Count > 0)
                {
                    sb.AppendLine("🏆 EN ÇOK GÜNCELLENEN UYGULAMALAR:");
                    for (int i = 0; i < topApps.Count; i++)
                    {
                        sb.AppendLine($"   {i + 1}. {topApps[i].AppName} ({topApps[i].Count} kez)");
                    }
                }
            }
            else
            {
                sb.AppendLine("Henüz güncelleme kaydı yok.");
                sb.AppendLine("\nBir uygulama güncelleyip tekrar deneyin!");
            }

            sb.AppendLine("\n═══════════════════════════════════════");
            sb.AppendLine($"Database: {System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"EverythingUpToDate\update_history.csv")}");

            MessageBox.Show(sb.ToString(), "Güncelleme Geçmişi", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
