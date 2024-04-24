using Csv;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleCalendar.WPF.Services
{
    public class SettingsService
    {
        public static string UserSettingBaseDir { get; set; }
        public static string AppName {  get; set; }

        static SettingsService()
        {
            // Unicode系以外のエンコーディングを使用する場合は必須
            // 参考: https://www.curict.com/item/72/72d5fb2.html
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            // ユーザー設定基点ディレクトリ
            // ApplicationData = %HOMEDRIVE%%HOMEPATH%\AppData\Roaming - デバイス間で共有する(アカウントに紐付く)情報を格納する
            // LocalApplicationData = %HOMEDRIVE%%HOMEPATH%\AppData\Local - デバイス固有の情報を格納する
            UserSettingBaseDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var asmName = typeof(App).Assembly.GetName();
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

        public string InitSettings(string name)
        {
            if (!Directory.Exists(userSettingsDir))
            {
                Directory.CreateDirectory(userSettingsDir);
            }
            var userPath = Path.Combine(userSettingsDir, name);
            if (!File.Exists(userPath))
            {
                var origPath = Path.Combine(appDir, name);
                if (!File.Exists(origPath))
                {
                    throw new FileNotFoundException("No initial configuration file", origPath);
                }
                File.Copy(origPath, userPath);
            }
            return userPath;
        }

        public void ReadCsvFile(string name, Action<ICsvLine> handler)
        {
            var userPath = InitSettings(name);
            // BOMが付いていると、BOMの判定を優先する
            using var sr = new StreamReader(userPath, cp932, true);
            var opts = new CsvOptions()
            {
                HeaderMode = HeaderMode.HeaderPresent,
                TrimData = true,
                AllowNewLineInEnclosedFieldValues = true,
            };
            foreach (var row in CsvReader.Read(sr, opts))
            {
                handler(row);
            }
        }
    }
}
