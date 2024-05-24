using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using SimpleCalendar.WinUI3.Utilities;

namespace SimpleCalendar.WinUI3.Services
{
    public enum HolidayUpdaterStatus
    {
        IN_PROGRESS,
        NO_UPDATE_REQUIRED,
        DOWNLOADED,
        ERROR,
    }

    public class HolidayUpdaterService
    {
        public const string HolidaysCsvUri = "https://www8.cao.go.jp/chosei/shukujitsu/syukujitsu.csv";

        private const string LastModified = "Last-Modified";
        private const string ContentLength = "Content-Length";

        private readonly string _settingPath;
        private readonly string _headerPath;

        private readonly SemaphoreSlim _semaphore = new(1);

        public delegate Task StatusChanged(HolidayUpdaterStatus status, params object[] args);

        public HolidayUpdaterService()
        {
            SettingFiles.Holidays.Initialize();
            _settingPath = SettingFiles.Holidays.SettingPath;
            _headerPath = SettingFiles.Holidays.ExtPath(SettingFiles.HEADER);
        }

        public async Task<HolidayUpdaterStatus> UpdateAsync(StatusChanged statusChanged)
        {
            bool locked = false;
            try
            {
                locked = await _semaphore.WaitAsync(0);
                if (!locked) { return HolidayUpdaterStatus.IN_PROGRESS; }
                await statusChanged(HolidayUpdaterStatus.IN_PROGRESS).ConfigureAwait(false);
                // ローカルに保存された最終更新日を取得
                if (GetSavedLastModified() is string lastModified)
                {
                    // HEAD リクエストで更新状況を確認。
                    // If-Modified-Since, If-None-Match は期待通り動かなかったので、設定せずにリクエスト送出。
                    // また、Etag は、中身が変わっていないのに値が変わっているケースがあったため、チェック対象とはしない。
                    using (var client = new HttpClient())
                    {
                        HttpRequestMessage request = new(HttpMethod.Head, HolidaysCsvUri)
                        {
                            Version = Version.Parse("2.0")
                        };
                        HttpResponseMessage response = await client.SendAsync(request).ConfigureAwait(false);
                        if (response.IsSuccessStatusCode)
                        {
                            // ※「response.Headers」ではなく「response.Content.Headers」でないと、「Last-Modified」が拾えない(!?)
                            HttpContentHeaders h = response.Content.Headers;
                            var lm = DateTimeOffset.Parse(lastModified);
                            if (lm == h.LastModified)
                            {
                                await statusChanged(HolidayUpdaterStatus.NO_UPDATE_REQUIRED, lm).ConfigureAwait(false);
                                return HolidayUpdaterStatus.NO_UPDATE_REQUIRED;
                            }
                        }
                    }
                }

                // ファイルをダウンロード
                using (var client = new HttpClient())
                {
                    HttpResponseMessage response = await client.GetAsync(HolidaysCsvUri).ConfigureAwait(false);
                    if (!response.IsSuccessStatusCode)
                    {
                        await statusChanged(HolidayUpdaterStatus.ERROR, response.StatusCode).ConfigureAwait(false);
                        return HolidayUpdaterStatus.ERROR;
                    }
                    response.EnsureSuccessStatusCode();
                    string newPath = $"{_settingPath}.new";
                    using (Stream stream = await response.Content.ReadAsStreamAsync())
                    {
                        using (FileStream fileStream = File.OpenWrite(newPath))
                        {
                            await stream.CopyToAsync(fileStream).ConfigureAwait(false);
                        }
                    }
                    File.Move(newPath, _settingPath, true);
                    // 新しいヘッダ情報の抜粋を保存
                    SaveHeaders(response.Content.Headers);
                }
                await statusChanged(HolidayUpdaterStatus.DOWNLOADED).ConfigureAwait(false);
                return HolidayUpdaterStatus.DOWNLOADED;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                await statusChanged(HolidayUpdaterStatus.ERROR, e).ConfigureAwait(false);
                return HolidayUpdaterStatus.ERROR;
            }
            finally
            {
                if (locked)
                {
                    _semaphore.Release();
                }
            }
        }

        private string GetSavedLastModified()
        {
            if (!File.Exists(_headerPath)) { return null; }
            using (StreamReader sr = new(_headerPath))
            {
                string line;
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
                if (headers.TryGetValues(LastModified, out IEnumerable<string> lastModified))
                {
                    sw.WriteLine($"{LastModified}: {lastModified.First()}");
                }
                if (headers.TryGetValues(ContentLength, out IEnumerable<string> contentLength))
                {
                    sw.WriteLine($"{ContentLength}: {contentLength.First()}");
                }
            }
            File.Move(newPath, _headerPath, true);
        }
    }
}
