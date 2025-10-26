using System;
using System.Drawing;
using System.Windows.Forms;

namespace Everything_UpToDate.Services
{
    /// <summary>
    /// Tema yönetimi servisi
    /// </summary>
    public class ThemeService
    {
        public enum Theme
        {
            Light,
            Dark
        }

        private Theme _currentTheme = Theme.Light;

        public Theme CurrentTheme
        {
            get { return _currentTheme; }
            set
            {
                _currentTheme = value;
                OnThemeChanged?.Invoke(this, value);
            }
        }

        public event EventHandler<Theme> OnThemeChanged;

        // Light Theme Colors
        public static class LightColors
        {
            public static readonly Color Background = Color.White;
            public static readonly Color Foreground = Color.FromArgb(64, 64, 64);
            public static readonly Color Secondary = Color.FromArgb(240, 240, 240);
            public static readonly Color Accent = Color.FromArgb(0, 120, 215);
            public static readonly Color Success = Color.FromArgb(0, 158, 115);
            public static readonly Color Warning = Color.FromArgb(255, 165, 0);
            public static readonly Color Error = Color.FromArgb(232, 17, 35);
            public static readonly Color Border = Color.FromArgb(200, 200, 200);
            public static readonly Color ListViewBack = Color.White;
            public static readonly Color ListViewFore = Color.Black;
            public static readonly Color UpdateAvailable = Color.FromArgb(255, 250, 205); // Açýk sarý
            public static readonly Color UpToDate = Color.FromArgb(240, 255, 240); // Açýk yeþil
        }

        // Dark Theme Colors
        public static class DarkColors
        {
            public static readonly Color Background = Color.FromArgb(30, 30, 30);
            public static readonly Color Foreground = Color.FromArgb(220, 220, 220);
            public static readonly Color Secondary = Color.FromArgb(45, 45, 45);
            public static readonly Color Accent = Color.FromArgb(0, 153, 255);
            public static readonly Color Success = Color.FromArgb(76, 209, 55);
            public static readonly Color Warning = Color.FromArgb(255, 193, 7);
            public static readonly Color Error = Color.FromArgb(231, 76, 60);
            public static readonly Color Border = Color.FromArgb(60, 60, 60);
            public static readonly Color ListViewBack = Color.FromArgb(40, 40, 40);
            public static readonly Color ListViewFore = Color.FromArgb(220, 220, 220);
            public static readonly Color UpdateAvailable = Color.FromArgb(60, 60, 30); // Koyu sarý
            public static readonly Color UpToDate = Color.FromArgb(30, 60, 30); // Koyu yeþil
        }

        /// <summary>
        /// Forma tema uygula
        /// </summary>
        public void ApplyTheme(Form form)
        {
            if (_currentTheme == Theme.Dark)
            {
                ApplyDarkTheme(form);
            }
            else
            {
                ApplyLightTheme(form);
            }
        }

        private void ApplyLightTheme(Form form)
        {
            form.BackColor = LightColors.Background;
            form.ForeColor = LightColors.Foreground;

            ApplyThemeToControls(form.Controls, Theme.Light);
        }

        private void ApplyDarkTheme(Form form)
        {
            form.BackColor = DarkColors.Background;
            form.ForeColor = DarkColors.Foreground;

            ApplyThemeToControls(form.Controls, Theme.Dark);
        }

        private void ApplyThemeToControls(Control.ControlCollection controls, Theme theme)
        {
            foreach (Control control in controls)
            {
                if (control is Button btn)
                {
                    ApplyButtonTheme(btn, theme);
                }
                else if (control is ListView lv)
                {
                    ApplyListViewTheme(lv, theme);
                }
                else if (control is TextBox tb)
                {
                    ApplyTextBoxTheme(tb, theme);
                }
                else if (control is Label lbl)
                {
                    ApplyLabelTheme(lbl, theme);
                }
                else if (control is ProgressBar pb)
                {
                    ApplyProgressBarTheme(pb, theme);
                }
                else if (control is Panel pnl)
                {
                    ApplyPanelTheme(pnl, theme);
                }

                // Recursive: Alt kontrollere de uygula
                if (control.HasChildren)
                {
                    ApplyThemeToControls(control.Controls, theme);
                }
            }
        }

        private void ApplyButtonTheme(Button btn, Theme theme)
        {
            if (theme == Theme.Dark)
            {
                // Buton rengini koru ama border'ý ayarla
                btn.FlatAppearance.BorderColor = DarkColors.Border;
                if (btn.BackColor == SystemColors.Control)
                {
                    btn.BackColor = DarkColors.Secondary;
                    btn.ForeColor = DarkColors.Foreground;
                }
            }
            else
            {
                btn.FlatAppearance.BorderColor = LightColors.Border;
                if (btn.BackColor == DarkColors.Secondary)
                {
                    btn.BackColor = SystemColors.Control;
                    btn.ForeColor = LightColors.Foreground;
                }
            }
        }

        private void ApplyListViewTheme(ListView lv, Theme theme)
        {
            if (theme == Theme.Dark)
            {
                lv.BackColor = DarkColors.ListViewBack;
                lv.ForeColor = DarkColors.ListViewFore;
            }
            else
            {
                lv.BackColor = LightColors.ListViewBack;
                lv.ForeColor = LightColors.ListViewFore;
            }
        }

        private void ApplyTextBoxTheme(TextBox tb, Theme theme)
        {
            if (theme == Theme.Dark)
            {
                tb.BackColor = DarkColors.Secondary;
                tb.ForeColor = DarkColors.Foreground;
                tb.BorderStyle = BorderStyle.FixedSingle;
            }
            else
            {
                tb.BackColor = LightColors.Background;
                tb.ForeColor = LightColors.Foreground;
                tb.BorderStyle = BorderStyle.FixedSingle;
            }
        }

        private void ApplyLabelTheme(Label lbl, Theme theme)
        {
            if (theme == Theme.Dark)
            {
                lbl.ForeColor = DarkColors.Foreground;
            }
            else
            {
                lbl.ForeColor = LightColors.Foreground;
            }
        }

        private void ApplyProgressBarTheme(ProgressBar pb, Theme theme)
        {
            // ProgressBar rengi Windows tarafýndan kontrol edilir
            // Özel drawing gerekebilir ama þimdilik standart
        }

        private void ApplyPanelTheme(Panel pnl, Theme theme)
        {
            if (theme == Theme.Dark)
            {
                pnl.BackColor = DarkColors.Background;
                pnl.ForeColor = DarkColors.Foreground;
            }
            else
            {
                pnl.BackColor = LightColors.Background;
                pnl.ForeColor = LightColors.Foreground;
            }
        }

        /// <summary>
        /// Temayý deðiþtir (toggle)
        /// </summary>
        public void ToggleTheme()
        {
            CurrentTheme = CurrentTheme == Theme.Dark ? Theme.Light : Theme.Dark;
        }

        /// <summary>
        /// Tema için uygun rengi döndürür
        /// </summary>
        public Color GetColor(string colorName)
        {
            if (_currentTheme == Theme.Dark)
            {
                switch (colorName.ToLower())
                {
                    case "background": return DarkColors.Background;
                    case "foreground": return DarkColors.Foreground;
                    case "secondary": return DarkColors.Secondary;
                    case "accent": return DarkColors.Accent;
                    case "success": return DarkColors.Success;
                    case "warning": return DarkColors.Warning;
                    case "error": return DarkColors.Error;
                    case "updateavailable": return DarkColors.UpdateAvailable;
                    case "uptodate": return DarkColors.UpToDate;
                    default: return DarkColors.Foreground;
                }
            }
            else
            {
                switch (colorName.ToLower())
                {
                    case "background": return LightColors.Background;
                    case "foreground": return LightColors.Foreground;
                    case "secondary": return LightColors.Secondary;
                    case "accent": return LightColors.Accent;
                    case "success": return LightColors.Success;
                    case "warning": return LightColors.Warning;
                    case "error": return LightColors.Error;
                    case "updateavailable": return LightColors.UpdateAvailable;
                    case "uptodate": return LightColors.UpToDate;
                    default: return LightColors.Foreground;
                }
            }
        }
    }
}
