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
        private const string CONFIG_CSV = "config.csv";

        public static string UserSettingBaseDir { get; set; }
        public static string LocalSettingBaseDir { get; set; }
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
            LocalSettingBaseDir = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            System.Reflection.AssemblyName asmName = typeof(App).Assembly.GetName();
            AppName = asmName.Name!; // nullになることはないと思うんだが、ほんとに大丈夫?
        }

        private readonly string _userSettingsDir;
        private readonly string _localSettingsDir;
        private readonly string _appDir;
        private readonly Encoding _cp932;

        public SettingsService()
        {
            // ユーザー設定ディレクトリ
            _userSettingsDir = Path.Combine(UserSettingBaseDir, AppName);
            _localSettingsDir = Path.Combine(LocalSettingBaseDir, AppName);
            // アプリケーションディレクトリ
            _appDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Contents");
            // コードページ932 (=WIndows-31J, CP932, MS932)
            // ※事前にEncoding.RegisterProvider(...)を実行しておくこと(静的コンストラクタ内参照)
            _cp932 = Encoding.GetEncoding(932);
        }

        public string StylesCsv { get; } = STYLES_CSV;
        public string HolydaysCsv { get; } = HOLYDAYS_CSV;
        public string SpecialDaysCsv { get; } = SPECIALDAYS_CSV;
        public string ConfigCSV { get; } = CONFIG_CSV;

        private string SettingsDir(bool isLocal)
        {
            return isLocal ? _localSettingsDir : _userSettingsDir;
        }

        public string InitSettings(string name, bool isLocal)
        {
            string dir = SettingsDir(isLocal);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            string userPath = Path.Combine(dir, name);
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

        public bool ReadCsvFile(string name, Action<ICsvLine> handler, Action<Exception>? error = null, bool isLocal = false)
        {
            string userPath = InitSettings(name, isLocal);
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

        public bool WriteCsvFile(string name, string[] headers, IEnumerable<string[]> enumerable, Action<Exception>? error = null, bool isLocal = false)
        {
            string dir = SettingsDir(isLocal);
            string userPath = Path.Combine(dir, name);
            string userPathNew = $"{userPath}.new";
            try
            {
                using (FileStream fs = new(userPathNew, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
                {
                    using StreamWriter sw = new(fs, Encoding.UTF8);
                    CsvWriter.Write(sw, headers, enumerable);
                }
                File.Move(userPathNew, userPath, true);
                return true;
            }
            catch (Exception e)
            {
                error?.Invoke(e);
                if (File.Exists(userPathNew))
                {
                    File.Delete(userPathNew);
                }
                return false;
            }
        }
    }
}
