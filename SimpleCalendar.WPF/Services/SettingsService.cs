using System.IO;
using System.Text;
using Csv;

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

        private readonly string _userSettingsDir;
        private readonly string _appDir;
        private readonly Encoding _cp932;

        public SettingsService()
        {
            // ユーザー設定ディレクトリ
            _userSettingsDir = Path.Combine(UserSettingBaseDir, AppName);
            // アプリケーションディレクトリ
            _appDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Contents");
            // コードページ932 (=WIndows-31J, CP932, MS932)
            // ※事前にEncoding.RegisterProvider(...)を実行しておくこと(静的コンストラクタ内参照)
            _cp932 = Encoding.GetEncoding(932);
        }

        public string StylesCsv { get; } = STYLES_CSV;
        public string HolydaysCsv { get; } = HOLYDAYS_CSV;
        public string SpecialDaysCsv { get; } = SPECIALDAYS_CSV;

        public string InitSettings(string name)
        {
            if (!Directory.Exists(_userSettingsDir))
            {
                Directory.CreateDirectory(_userSettingsDir);
            }
            string userPath = Path.Combine(_userSettingsDir, name);
            if (!File.Exists(userPath))
            {
                string origPath = Path.Combine(_appDir, name);
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
            try
            {
                // 他のプロセス(≒Excel)が対象ファイルを開いていてもエラーにならないよう、FileShare を指定。
                // 参考: https://stackoverflow.com/a/898017
                using FileStream fs = new(userPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                // 読み込みのエンコーディングにデフォルトではCP932を用いるが、
                // ファイルの先頭にBOMが付いているとBOMの判定結果を優先する
                using StreamReader sr = new(fs, _cp932, true);
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
