using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Ramazan_2025
{
    public partial class FormSettings : Form {
        public event EventHandler CityChanged;

        public FormSettings() {
            InitializeComponent();
            cbEnableReminder.Checked = Properties.Settings.Default.reminder;
            cbChangeCity.SelectedItem = Properties.Settings.Default.SelectedCity ?? "İstanbul";
        }

        private void cbEnableReminder_CheckedChanged(object sender, EventArgs e) {
            Properties.Settings.Default.reminder = cbEnableReminder.Checked;
            Properties.Settings.Default.Save();
        }
        private async void cbChangeCity_SelectedIndexChanged(object sender, EventArgs e) {
            string selectedCity = cbChangeCity.SelectedItem.ToString();
            Properties.Settings.Default.SelectedCity = selectedCity; // Şehri kaydediyoruz
            Properties.Settings.Default.Save(); // Değişiklikleri kaydediyoruz
            CityChanged?.Invoke(this, EventArgs.Empty);
        }
        private void btnClose_Click(object sender, EventArgs e) => this.Close();

        #region Movable Form
        private Point mouseLocation;
        private void FormSettings_MouseMove(object sender, MouseEventArgs e) { if (e.Button == MouseButtons.Left) this.Location = new Point(Control.MousePosition.X + mouseLocation.X, Control.MousePosition.Y + mouseLocation.Y); }
        private void FormSettings_MouseDown(object sender, MouseEventArgs e) => mouseLocation = new Point(-e.X, -e.Y);
        #endregion
    }
}
