using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using SimpleCalendar.WinUI3.Utilities;

namespace SimpleCalendar.WinUI3.Services
{
    public record LocalConfigEntry
    {
        public const string DEFAULT_FONTFAMILY = "Yu Gothic UI";
        public const int DEFAULT_FONTSIZE = 12;

        public double Left { get; set; } = 0.0;
        public double Top { get; set; } = 0.0;
        public double Width { get; set; } = 0.0;
        public double Height { get; set; } = 0.0;
        public string FontFamily { get; set; } = DEFAULT_FONTFAMILY;
        public int FontSize { get; set; } = DEFAULT_FONTSIZE;
    }

    public enum LoadStatus
    {
        NotLoaded,
        Loading,
        Loaded,
    }

    public enum SaveStatus
    {
        NotSaved,
        Saved,
    }

    public class LocalConfigService
    {
        public const int SAVE_DELEY = 1000; // ms

        public static readonly string[] Header =
        [
            "画面情報", "左", "上", "幅", "高", "フォント名", "フォントサイズ"
        ];

        private readonly SortedDictionary<string, LocalConfigEntry> _configs = [];

        private SaveStatus _saveStatus = SaveStatus.NotSaved;

        private int _savedCount = 0;

        private readonly DisplayAreas _displayAreas = ServiceRegistry.GetService<DisplayAreas>();

        public LoadStatus LoadStatus { get; private set; } = LoadStatus.NotLoaded;

        private static string parseColumn(string value, string defaultValue)
        {
            return string.IsNullOrEmpty(value) ? defaultValue : value;
        }

        private static int parseColumn(string value, int defaultValue)
        {
            if (string.IsNullOrEmpty(value))
            {
                return defaultValue;
            }
            try
            {
                return int.Parse(value);
            }
            catch
            {
                return defaultValue;
            }
        }

        public void Load(Action<LocalConfigEntry> handler = null)
        {
            lock (this)
            {
                if (LoadStatus != LoadStatus.NotLoaded) { return; }
                LoadStatus = LoadStatus.Loading;
                SettingFiles.Config.ReadCsvFile(csvLine =>
                {
                    string key = csvLine[0];
                    if (string.IsNullOrEmpty(key))
                    {
                        return;
                    }
                    LocalConfigEntry entry = new();
                    _configs.Add(key, entry);
                    switch (csvLine.ColumnCount)
                    {
                        case 2:
                            entry.Left = parseColumn(csvLine[1], 0);
                            break;
                        case 3:
                            entry.Top = parseColumn(csvLine[2], 0);
                            goto case 2;
                        case 4:
                            entry.Width = parseColumn(csvLine[3], -1);
                            goto case 3;
                        case 5:
                            entry.Height = parseColumn(csvLine[4], -1);
                            goto case 4;
                        case 6:
                            entry.FontFamily = parseColumn(csvLine[5], LocalConfigEntry.DEFAULT_FONTFAMILY);
                            goto case 5;
                        default:
                            entry.FontSize = parseColumn(csvLine[6], LocalConfigEntry.DEFAULT_FONTSIZE);
                            goto case 6;
                    }
                });
                if (handler != null && _configs.TryGetValue(_displayAreas.ScreenId, out LocalConfigEntry entry))
                {
                    handler.Invoke(entry);
                }
                LoadStatus = LoadStatus.Loaded;
            }
        }

        private IEnumerable<string[]> generateEntries()
        {
            foreach ((string key, LocalConfigEntry entry) in _configs)
            {
                string[] row =
                [
                     key!,
                    ((int) entry!.Left).ToString(),
                    ((int) entry!.Top).ToString(),
                    ((int) entry!.Width).ToString(),
                    ((int) entry!.Height).ToString(),
                    entry!.FontFamily,
                    entry!.FontSize.ToString()
                ];
                yield return row;
            }
        }

        public void Save(double left, double top, double width, double height)
        {
            lock (this)
            {
                if (LoadStatus != LoadStatus.Loaded) { return; }
                string key = _displayAreas.ScreenId;
                if (_configs.TryGetValue(key, out LocalConfigEntry entry))
                {
                    if (entry.Left == left && entry.Top == top)
                    {
                        return;
                    }
                }
                else
                {
                    entry = new();
                    _configs[key] = entry;
                }
                entry.Left = left;
                entry.Top = top;
                entry.Width = width;
                entry.Height = height;
                _saveStatus = SaveStatus.NotSaved;
                Debug.WriteLine($"Saving: ({entry.Left}, {entry.Top})");
                Task.Delay(SAVE_DELEY).ContinueWith(task =>
                {
                    lock (this)
                    {
                        if (_saveStatus != SaveStatus.NotSaved) { return; }
                        SettingFiles.Config.WriteCsvFile(Header, generateEntries());
                        _saveStatus = SaveStatus.Saved;
                        _savedCount++;
                        Debug.WriteLine($"[INFO] Saved local config: {_savedCount}");
                    }
                });
            }
        }
    }
}
