using System.IO;
using System.Text;
using Csv;

namespace SimpleCalendar.WPF.Utilities
{
    public class SettingFiles(bool isLocal, params string[] filenames)
    {
        public static readonly SettingFiles Styles = new(false, "styles.csv");
        public static readonly SettingFiles Holidays = new(false, "syukujitsu.csv", "syukujitsu.csv.header");
        public static readonly SettingFiles Specialdays = new(false, "specialdays.csv");
        public static readonly SettingFiles Config = new(true, "config.csv");

        public static string UserSettingBaseDir { get; set; }
        public static string LocalSettingBaseDir { get; set; }
        public static string AppName { get; set; }
        public static string AppBaseDir { get; set; }

        private static readonly Encoding s_cp932;

        static SettingFiles()
        {
            // ユーザー設定基点ディレクトリ
            // ApplicationData = %HOMEDRIVE%%HOMEPATH%\AppData\Roaming - デバイス間で共有する(アカウントに紐付く)情報を格納する
            // LocalApplicationData = %HOMEDRIVE%%HOMEPATH%\AppData\Local - デバイス固有の情報を格納する
            UserSettingBaseDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            LocalSettingBaseDir = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            System.Reflection.AssemblyName asmName = typeof(App).Assembly.GetName();
            AppName = asmName.Name!; // nullになることはないと思うんだが、ほんとに大丈夫?
            AppBaseDir = AppDomain.CurrentDomain.BaseDirectory;

            // Unicode系以外のエンコーディングを使用する場合は必須
            // 参考: https://www.curict.com/item/72/72d5fb2.html
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            // コードページ932 (=WIndows-31J, CP932, MS932)
            // ※事前にEncoding.RegisterProvider(...)を実行しておくこと(静的コンストラクタ内参照)
            s_cp932 = Encoding.GetEncoding(932);
        }

        private bool _isInitialized = false;

        public string SettingDir => Path.Combine(isLocal ? LocalSettingBaseDir : UserSettingBaseDir, AppName);
        private static string ContentDir => Path.Combine(AppBaseDir, "Contents");

        public string SettingPath(int n = 0) => Path.Combine(SettingDir, filenames[n]);

        public int Count => filenames.Length;

        public void Initialize()
        {
            if (_isInitialized)
            {
                return;
            }
            string sDir = SettingDir;
            string cDir = ContentDir;
            if (!Directory.Exists(sDir))
            {
                Directory.CreateDirectory(sDir);
            }
            foreach (string filename in filenames)
            {
                string userPath = Path.Combine(sDir, filename);
                if (!File.Exists(userPath))
                {
                    string origPath = Path.Combine(cDir, filename);
                    if (File.Exists(origPath))
                    {
                        File.Copy(origPath, userPath);
                    }
                }
            }
            _isInitialized = true;
        }

        public bool ReadCsvFile(Action<ICsvLine> handler, Action<Exception>? error = null)
        {
            Initialize();
            string userPath = SettingPath(0);
            try
            {
                // 他のプロセス(≒Excel)が対象ファイルを開いていてもエラーにならないよう、FileShare を指定。
                // 参考: https://stackoverflow.com/a/898017
                using FileStream fs = new(userPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                // 読み込みのエンコーディングにデフォルトではCP932を用いるが、
                // ファイルの先頭にBOMが付いているとBOMの判定結果を優先する
                using StreamReader sr = new(fs, s_cp932, true);
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

        public bool WriteCsvFile(string[] headers, IEnumerable<string[]> enumerable, Action<Exception>? error = null)
        {
            Initialize();
            string userPath = SettingPath(0);
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
