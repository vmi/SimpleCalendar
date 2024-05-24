using Microsoft.UI.Xaml;

namespace SimpleCalendar.WinUI3.Views
{
    /// <summary>
    /// Help.xaml の相互作用ロジック
    /// </summary>
    public partial class Help : Window
    {
        public Help()
        {
            InitializeComponent();
#if false
            // ヘルプはxamlファイルに直接埋め込むよう変更。
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
#endif
        }
    }
}
