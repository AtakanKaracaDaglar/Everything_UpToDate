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
            // �kon ayarla (�imdilik varsay�lan icon, sonra �zel .ico ekleyece�iz)
            _notifyIcon.Icon = SystemIcons.Application;
            _notifyIcon.Text = "Everything UpToDate";
            _notifyIcon.Visible = false;

            // Context menu olu�tur
            var contextMenu = new ContextMenuStrip();
            
            var openItem = new ToolStripMenuItem("A�");
            openItem.Font = new Font(openItem.Font, FontStyle.Bold);
            openItem.Click += (s, e) => OnShowMainWindow?.Invoke(this, EventArgs.Empty);
            
            var scanItem = new ToolStripMenuItem("Uygulamalar� Tara");
            scanItem.Click += (s, e) => OnScanRequested?.Invoke(this, EventArgs.Empty);
            
            var separator = new ToolStripSeparator();
            
            var exitItem = new ToolStripMenuItem("��k��");
            exitItem.Click += (s, e) => OnExitRequested?.Invoke(this, EventArgs.Empty);

            contextMenu.Items.Add(openItem);
            contextMenu.Items.Add(scanItem);
            contextMenu.Items.Add(separator);
            contextMenu.Items.Add(exitItem);

            _notifyIcon.ContextMenuStrip = contextMenu;

            // �ift t�klama eventi
            _notifyIcon.DoubleClick += (s, e) => OnShowMainWindow?.Invoke(this, EventArgs.Empty);
        }

        // Events
        public event EventHandler OnShowMainWindow;
        public event EventHandler OnScanRequested;
        public event EventHandler OnExitRequested;

        /// <summary>
        /// Tray icon'u g�ster
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
        /// Balon bildirimi g�ster
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
        /// G�ncelleme bildirimi g�ster
        /// </summary>
        public void ShowUpdateNotification(int updateCount)
        {
            if (updateCount == 0)
                return;

            string message = updateCount == 1
                ? "1 uygulama i�in g�ncelleme mevcut!"
                : $"{updateCount} uygulama i�in g�ncelleme mevcut!";

            ShowBalloonTip("G�ncelleme Bulundu", message, ToolTipIcon.Info);
        }

        /// <summary>
        /// Ba�ar� bildirimi
        /// </summary>
        public void ShowSuccessNotification(string message)
        {
            ShowBalloonTip("Ba�ar�l�", message, ToolTipIcon.Info);
        }

        /// <summary>
        /// Hata bildirimi
        /// </summary>
        public void ShowErrorNotification(string message)
        {
            ShowBalloonTip("Hata", message, ToolTipIcon.Error);
        }

        /// <summary>
        /// Uyar� bildirimi
        /// </summary>
        public void ShowWarningNotification(string message)
        {
            ShowBalloonTip("Uyar�", message, ToolTipIcon.Warning);
        }

        /// <summary>
        /// Tray tooltip g�ncelle
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
