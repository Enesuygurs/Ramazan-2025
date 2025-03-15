using Microsoft.Win32;

namespace Ramazan_2025
{
    public partial class FormSettings : Form {
        public event EventHandler? CityChanged;
        private bool _isInitializing = false;

        public FormSettings() {
            InitializeComponent();
        }
        private void FormSettings_Load(object sender, EventArgs e) {
            cbEnableReminder.Checked = Properties.Settings.Default.reminder;
            cbChangeCity.SelectedItem = Properties.Settings.Default.SelectedCity ?? "İstanbul";
            _isInitializing = true; // 🔥 Event'leri tekrar aktif et
        }

        #region Settings Controls
        private void cbEnableReminder_CheckedChanged(object sender, EventArgs e) {
            Properties.Settings.Default.reminder = cbEnableReminder.Checked;
            Properties.Settings.Default.Save();
        }
        private void cbRunStartup_CheckedChanged(object sender, EventArgs e) {
            string appName = "Ramazan 2025";
            string exePath = Application.ExecutablePath;
            string registryPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";

            try {
                using (RegistryKey? regKey = Registry.CurrentUser.OpenSubKey(registryPath, true)) {
                    if (regKey == null) {
                        MessageBox.Show("Başlangıç ayarlarını değiştirirken bir hata oluştu!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    if (cbRunOnStartup.Checked) {
                        regKey.SetValue(appName, exePath); // Başlangıçta aç
                    } else {
                        if (regKey.GetValue(appName) != null) regKey.DeleteValue(appName, false); // Başlangıçtan kaldır
                    }
                }
            } catch (Exception ex) {
                MessageBox.Show($"Başlangıç ayarı değiştirilemedi: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ChangeCityAsync() {
            try {
                string selectedCity = cbChangeCity.SelectedItem?.ToString() ?? "İstanbul";
                Properties.Settings.Default.SelectedCity = selectedCity; // Şehri kaydediyoruz
                Properties.Settings.Default.Save(); // Değişiklikleri kaydediyoruz
                CityChanged?.Invoke(this, EventArgs.Empty);
            } catch (Exception ex) {
                MessageBox.Show($"Şehir değiştirilirken bir hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void cbChangeCity_SelectedIndexChanged(object sender, EventArgs e) {
            if (!_isInitializing) return;
            ChangeCityAsync();
        }
        #endregion

        private void btnClose_Click(object sender, EventArgs e) => this.Close();

        #region Movable Form
        private Point mouseLocation;
        private void FormSettings_MouseMove(object sender, MouseEventArgs e) { if (e.Button == MouseButtons.Left) this.Location = new Point(Control.MousePosition.X + mouseLocation.X, Control.MousePosition.Y + mouseLocation.Y); }
        private void FormSettings_MouseDown(object sender, MouseEventArgs e) => mouseLocation = new Point(-e.X, -e.Y);
        #endregion
    }
}
