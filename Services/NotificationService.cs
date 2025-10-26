using System;
using System.Drawing;
using System.Windows.Forms;

namespace Everything_UpToDate.Services
{
    /// <summary>
    /// Windows bildirim servisi
    /// </summary>
    public class NotificationService
    {
        private readonly NotifyIcon _notifyIcon;
        private readonly SettingsService _settingsService;

        public NotifyIcon NotifyIcon => _notifyIcon;

        public NotificationService(SettingsService settingsService)
        {
            _settingsService = settingsService;
            _notifyIcon = new NotifyIcon();
            InitializeNotifyIcon();
        }

        private void InitializeNotifyIcon()
        {
            // Ýkon ayarla (þimdilik varsayýlan icon, sonra özel .ico ekleyeceðiz)
            _notifyIcon.Icon = SystemIcons.Application;
            _notifyIcon.Text = "Everything UpToDate";
            _notifyIcon.Visible = false;

            // Context menu oluþtur
            var contextMenu = new ContextMenuStrip();
            
            var openItem = new ToolStripMenuItem("Aç");
            openItem.Font = new Font(openItem.Font, FontStyle.Bold);
            openItem.Click += (s, e) => OnShowMainWindow?.Invoke(this, EventArgs.Empty);
            
            var scanItem = new ToolStripMenuItem("Uygulamalarý Tara");
            scanItem.Click += (s, e) => OnScanRequested?.Invoke(this, EventArgs.Empty);
            
            var separator = new ToolStripSeparator();
            
            var exitItem = new ToolStripMenuItem("Çýkýþ");
            exitItem.Click += (s, e) => OnExitRequested?.Invoke(this, EventArgs.Empty);

            contextMenu.Items.Add(openItem);
            contextMenu.Items.Add(scanItem);
            contextMenu.Items.Add(separator);
            contextMenu.Items.Add(exitItem);

            _notifyIcon.ContextMenuStrip = contextMenu;

            // Çift týklama eventi
            _notifyIcon.DoubleClick += (s, e) => OnShowMainWindow?.Invoke(this, EventArgs.Empty);
        }

        // Events
        public event EventHandler OnShowMainWindow;
        public event EventHandler OnScanRequested;
        public event EventHandler OnExitRequested;

        /// <summary>
        /// Tray icon'u göster
        /// </summary>
        public void Show()
        {
            _notifyIcon.Visible = true;
        }

        /// <summary>
        /// Tray icon'u gizle
        /// </summary>
        public void Hide()
        {
            _notifyIcon.Visible = false;
        }

        /// <summary>
        /// Balon bildirimi göster
        /// </summary>
        public void ShowBalloonTip(string title, string message, ToolTipIcon icon = ToolTipIcon.Info, int timeout = 3000)
        {
            if (!_settingsService.Settings.ShowNotifications)
                return;

            _notifyIcon.BalloonTipTitle = title;
            _notifyIcon.BalloonTipText = message;
            _notifyIcon.BalloonTipIcon = icon;
            _notifyIcon.ShowBalloonTip(timeout);
        }

        /// <summary>
        /// Güncelleme bildirimi göster
        /// </summary>
        public void ShowUpdateNotification(int updateCount)
        {
            if (updateCount == 0)
                return;

            string message = updateCount == 1
                ? "1 uygulama için güncelleme mevcut!"
                : $"{updateCount} uygulama için güncelleme mevcut!";

            ShowBalloonTip("Güncelleme Bulundu", message, ToolTipIcon.Info);
        }

        /// <summary>
        /// Baþarý bildirimi
        /// </summary>
        public void ShowSuccessNotification(string message)
        {
            ShowBalloonTip("Baþarýlý", message, ToolTipIcon.Info);
        }

        /// <summary>
        /// Hata bildirimi
        /// </summary>
        public void ShowErrorNotification(string message)
        {
            ShowBalloonTip("Hata", message, ToolTipIcon.Error);
        }

        /// <summary>
        /// Uyarý bildirimi
        /// </summary>
        public void ShowWarningNotification(string message)
        {
            ShowBalloonTip("Uyarý", message, ToolTipIcon.Warning);
        }

        /// <summary>
        /// Tray tooltip güncelle
        /// </summary>
        public void UpdateTooltip(string text)
        {
            _notifyIcon.Text = text.Length > 63 ? text.Substring(0, 63) : text;
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            _notifyIcon?.Dispose();
        }
    }
}
