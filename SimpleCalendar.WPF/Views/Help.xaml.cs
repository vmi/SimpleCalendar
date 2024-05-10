using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Markup;

namespace SimpleCalendar.WPF.Views
{
    /// <summary>
    /// Help.xaml の相互作用ロジック
    /// </summary>
    public partial class Help : Window
    {
        public Help()
        {
            InitializeComponent();
            string appDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Contents");
            try
            {
                using FileStream fs = new(Path.Combine(appDir, "Help.xaml"), FileMode.Open, FileAccess.ReadWrite);
                var doc = XamlReader.Load(fs) as FlowDocument;
                HelpViewer.Document = doc;
            }
            catch (Exception e)
            {
                // TODO ヘルプファイルの読み込みに失敗したときの処理を追加
                Debug.WriteLine($"ヘルプファイルの読み込みに失敗しました: {e}");
            }
        }
    }
}
