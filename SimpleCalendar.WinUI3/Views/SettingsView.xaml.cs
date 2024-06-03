using System.Collections;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using SimpleCalendar.WinUI3.Utilities;
using WinUIEx;

namespace SimpleCalendar.WinUI3.Views
{
    /// <summary>
    /// SettingsView.xaml の相互作用ロジック
    /// </summary>
    public partial class SettingsView : WindowEx
    {
        public SettingsView()
        {
            InitializeComponent();
            ExtendsContentIntoTitleBar = true;
            if (WindowContent is FrameworkElement elem)
            {
                elem.Loaded += (_, _) =>
                {
                    var p = VisualTreeHelper.GetParent(elem);
                };
            }

            if (LogListView?.ItemsSource is INotifyCollectionChanged notify)
            {
                notify.CollectionChanged += LogListView_CollectionChanged;
            }
        }

        private void LogListView_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems is IList newItems)
            {
                object item = newItems[newItems.Count - 1]!;
                LogListView?.ScrollIntoView(item);
            }
        }

        private void OpenSettingsFolder_Click(object sender, RoutedEventArgs e)
        {
            using Process process = new();
            ProcessStartInfo startInfo = process.StartInfo;
            startInfo.UseShellExecute = true;
            startInfo.FileName = Path.Combine(SettingFiles.UserSettingBaseDir, SettingFiles.AppName);
            process.Start();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            // Hide();
        }
    }
}
