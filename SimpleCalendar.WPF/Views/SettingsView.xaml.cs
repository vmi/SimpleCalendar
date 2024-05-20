using System.Diagnostics;
using System.Windows;
using SimpleCalendar.WPF.Utilities;

namespace SimpleCalendar.WPF.Views
{
    /// <summary>
    /// SettingsView.xaml の相互作用ロジック
    /// </summary>
    public partial class SettingsView : Window
    {
        public SettingsView()
        {
            InitializeComponent();
        }

        private void OpenSettingsFolder_Click(object sender, RoutedEventArgs e)
        {
            using Process process = new();
            ProcessStartInfo startInfo = process.StartInfo;
            startInfo.UseShellExecute = true;
            startInfo.FileName = SettingFiles.UserSettingBaseDir;
            process.Start();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Hide();
        }
    }
}
