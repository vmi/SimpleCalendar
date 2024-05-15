using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using SimpleCalendar.WPF.Models;
using SimpleCalendar.WPF.Utilities;

namespace SimpleCalendar.WPF.Services
{
    public enum HolidayUpdaterStatus
    {
        NO_UPDATE_REQUIRED,
        UPDATED,
        ERROR,
    }

    public class HolidayUpdaterService
    {
        public const string HolidaysCsvUri = "https://www8.cao.go.jp/chosei/shukujitsu/syukujitsu.csv";

        private const string LastModified = "Last-Modified";
        private const string ContentLength = "Content-Length";

        private readonly SettingsLogger _logger;

        private readonly string _settingPath;
        private readonly string _headerPath;

        public HolidayUpdaterService(SettingsLogger logger)
        {
            _logger = logger;
            SettingFiles.Holidays.Initialize();
            _settingPath = SettingFiles.Holidays.SettingPath;
            _headerPath = SettingFiles.Holidays.ExtPath(SettingFiles.HEADER);
        }


        public async Task<HolidayUpdaterStatus> UpdateAsync()
        {
            // ローカルに保存された ETag を取得
            if (GetSavedLastModified() is string lastModified)
            {
                _logger.Log("祝日ファイルの更新を確認中");
                // HEAD リクエストで更新状況を確認。
                // If-Modified-Since, If-None-Match は期待通り動かなかったので、設定せずにリクエスト送出。
                // また、Etag は、中身が変わっていないのに値が変わっているケースがあったため、チェック対象とはしない。
                using (var client = new HttpClient())
                {
                    HttpRequestMessage request = new(HttpMethod.Head, HolidaysCsvUri)
                    {
                        Version = Version.Parse("2.0")
                    };
                    HttpResponseMessage response = await client.SendAsync(request);
                    if (response.IsSuccessStatusCode)
                    {
                        // ※「response.Headers」ではなく「response.Content.Headers」でないと、「Last-Modified」が拾えない(!?)
                        HttpContentHeaders h = response.Content.Headers;
                        var lm = DateTimeOffset.Parse(lastModified);
                        if (lm == h.LastModified)
                        {
                            _logger.Log($"祝日ファイルは最新です (最終更新日時: {lm.ToLocalTime():yyyy-MM-dd(ddd) HH:mm:ss zzz})");
                            return HolidayUpdaterStatus.NO_UPDATE_REQUIRED;
                        }
                    }
                }
            }

            // ファイルをダウンロード
            using (var client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(HolidaysCsvUri);
                if (!response.IsSuccessStatusCode)
                {
                    _logger.Log("祝日ファイルの取得に失敗しました");
                    return HolidayUpdaterStatus.ERROR;
                }
                response.EnsureSuccessStatusCode();
                string newPath = $"{_settingPath}.new";
                using (Stream stream = await response.Content.ReadAsStreamAsync())
                {
                    using (FileStream fileStream = File.OpenWrite(newPath))
                    {
                        await stream.CopyToAsync(fileStream);
                    }
                }
                File.Move(newPath, _settingPath, true);
                // 新しい ETag を保存
                SaveHeaders(response.Content.Headers);
            }
            _logger.Log("祝日ファイルを最新化しました");
            return HolidayUpdaterStatus.UPDATED;
        }

        private string? GetSavedLastModified()
        {
            if (!File.Exists(_headerPath)) { return null; }
            using (StreamReader sr = new(_headerPath))
            {
                string? line;
                while ((line = sr.ReadLine()) != null)
                {
                    string[] entry = line.Split(':', 2, StringSplitOptions.TrimEntries);
                    if (entry.Length != 2)
                    {
                        continue;
                    }
                    switch (entry[0])
                    {
                        case LastModified:
                            return entry[1];
                        default:
                            // no operation.
                            break;
                    }
                }
            }
            return null;
        }

        private void SaveHeaders(HttpContentHeaders headers)
        {
            string newPath = $"{_headerPath}.new";
            using (StreamWriter sw = new(newPath))
            {
                sw.NewLine = "\n";
                if (headers.TryGetValues(LastModified, out IEnumerable<string>? lastModified))
                {
                    sw.WriteLine($"{LastModified}: {lastModified.First()}");
                }
                if (headers.TryGetValues(ContentLength, out IEnumerable<string>? contentLength))
                {
                    sw.WriteLine($"{ContentLength}: {contentLength.First()}");
                }
            }
            File.Move(newPath, _headerPath, true);
        }
    }
}
