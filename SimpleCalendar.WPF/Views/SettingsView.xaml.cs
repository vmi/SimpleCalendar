using System.Windows;

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

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Hide();
        }
    }
}
