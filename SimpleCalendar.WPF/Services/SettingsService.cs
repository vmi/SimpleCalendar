using Csv;
using System.IO;
using System.Security.Permissions;
using System.Text;

namespace SimpleCalendar.WPF.Services
{
    public class SettingsService
    {
        private const string STYLES_CSV = "styles.csv";
        private const string HOLYDAYS_CSV = "syukujitsu.csv";
        private const string SPECIALDAYS_CSV = "specialdays.csv";

        public static string UserSettingBaseDir { get; set; }
        public static string AppName { get; set; }

        static SettingsService()
        {
            // Unicode系以外のエンコーディングを使用する場合は必須
            // 参考: https://www.curict.com/item/72/72d5fb2.html
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            // ユーザー設定基点ディレクトリ
            // ApplicationData = %HOMEDRIVE%%HOMEPATH%\AppData\Roaming - デバイス間で共有する(アカウントに紐付く)情報を格納する
            // LocalApplicationData = %HOMEDRIVE%%HOMEPATH%\AppData\Local - デバイス固有の情報を格納する
            UserSettingBaseDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            System.Reflection.AssemblyName asmName = typeof(App).Assembly.GetName();
            AppName = asmName.Name!; // nullになることはないと思うんだが、ほんとに大丈夫?
        }

        private readonly string userSettingsDir;
        private readonly string appDir;
        private readonly Encoding cp932;

        public SettingsService()
        {
            // ユーザー設定ディレクトリ
            userSettingsDir = Path.Combine(UserSettingBaseDir, AppName);
            // アプリケーションディレクトリ
            appDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Contents");
            // コードページ932 (=WIndows-31J, CP932, MS932)
            // ※事前にEncoding.RegisterProvider(...)を実行しておくこと(静的コンストラクタ内参照)
            cp932 = Encoding.GetEncoding(932);
        }

        public string StylesCsv { get; } = STYLES_CSV;
        public string HolydaysCsv { get; } = HOLYDAYS_CSV;
        public string SpecialDaysCsv { get; } = SPECIALDAYS_CSV;

        public string InitSettings(string name)
        {
            if (!Directory.Exists(userSettingsDir))
            {
                Directory.CreateDirectory(userSettingsDir);
            }
            string userPath = Path.Combine(userSettingsDir, name);
            if (!File.Exists(userPath))
            {
                string origPath = Path.Combine(appDir, name);
                if (!File.Exists(origPath))
                {
                    throw new FileNotFoundException("No initial configuration file", origPath);
                }
                File.Copy(origPath, userPath);
            }
            return userPath;
        }

        public bool ReadCsvFile(string name, Action<ICsvLine> handler, Action<Exception>? error = null)
        {
            string userPath = InitSettings(name);
            // 読み込みのエンコーディングにデフォルトではCP932を用いるが、
            // ファイルの先頭にBOMが付いているとBOMの判定結果を優先する
            try
            {
                using StreamReader sr = new(userPath, cp932, true);
                CsvOptions opts = new()
                {
                    HeaderMode = HeaderMode.HeaderPresent,
                    TrimData = true,
                    AllowNewLineInEnclosedFieldValues = true,
                };
                foreach (ICsvLine row in CsvReader.Read(sr, opts))
                {
                    handler(row);
                }
                return true;
            }
            catch (IOException e)
            {
                error?.Invoke(e);
                return false;
            }
        }
    }
}
