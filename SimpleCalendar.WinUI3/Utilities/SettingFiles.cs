using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Csv;

namespace SimpleCalendar.WinUI3.Utilities
{
    public class SettingFiles
    {
        public const string HEADER = "HEADER";
        public static readonly SettingFiles Styles = new(false, "styles.csv");
        public static readonly SettingFiles Holidays = new(false, "syukujitsu.csv", [(HEADER, "syukujitsu.csv.header")]);
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

        private readonly bool _isLocal;
        private readonly string _filename;
        private readonly Dictionary<string, string> _extFilenameDict = [];
        private bool _isInitialized = false;

        public string SettingDir => Path.Combine(_isLocal ? LocalSettingBaseDir : UserSettingBaseDir, AppName);
        private static string ContentDir => Path.Combine(AppBaseDir, "Contents");

        public string SettingFilename => _filename;
        public string SettingPath => Path.Combine(SettingDir, _filename);
        public string ExtPath(string key) => Path.Combine(SettingDir, _extFilenameDict[key]);

        public SettingFiles(bool isLocal, string filename, List<(string, string)> extFilenameList = null)
        {
            _isLocal = isLocal;
            _filename = filename;
            if (extFilenameList != null)
            {
                foreach ((string k, string v) in extFilenameList)
                {
                    _extFilenameDict.Add(k, v);
                }
            }
        }

        private void copyDefaultFile(string filename)
        {
            string userPath = Path.Combine(SettingDir, filename);
            if (!File.Exists(userPath))
            {
                string origPath = Path.Combine(ContentDir, filename);
                if (File.Exists(origPath))
                {
                    File.Copy(origPath, userPath);
                }
            }
        }

        public void Initialize()
        {
            if (_isInitialized)
            {
                return;
            }
            if (!Directory.Exists(SettingDir))
            {
                Directory.CreateDirectory(SettingDir);
            }
            copyDefaultFile(_filename);
            foreach (string filename in _extFilenameDict.Values)
            {
                copyDefaultFile(filename);
            }
            _isInitialized = true;
        }

        public bool ReadCsvFile(Action<ICsvLine> handler, Action<Exception> error = null)
        {
            Initialize();
            string userPath = SettingPath;
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

        public bool WriteCsvFile(string[] headers, IEnumerable<string[]> enumerable, Action<Exception> error = null)
        {
            Initialize();
            string userPath = SettingPath;
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
