using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SimpleCalendar.WPF.Services;
using System.Windows.Media;

namespace SimpleCalendar.WPF.ViewModels
{
    public readonly record struct DayLabelStyle
    {
        private static readonly BrushConverter brushConverter = new();

        public Brush? Foreground { get; }
        public Brush? Background { get; }
        public Brush? Border { get; }

        public DayLabelStyle(string? fg, string? bg, string? bd)
        {
            Foreground = String.IsNullOrEmpty(fg) ? null : (Brush?) brushConverter.ConvertFromString(fg);
            Background = String.IsNullOrEmpty(bg) ? null : (Brush?) brushConverter.ConvertFromString(bg);
            Border = String.IsNullOrEmpty(bd) ? null : (Brush?) brushConverter.ConvertFromString(bd);
        }
    }

    public partial class DayLabelStyleViewModel : ObservableObject
    {
        private readonly SettingsService settingsService;

        [ObservableProperty]
        private Dictionary<string, DayLabelStyle> _dayLabelStyles = [];

        public DayLabelStyleViewModel(SettingsService settingsService)
        {
            this.settingsService = settingsService;
            LoadSetting();
        }

        [RelayCommand]
        private void LoadSetting()
        {
            DayLabelStyles.Clear();
            settingsService.ReadCsvFile(settingsService.StylesCsv, csvLine =>
            {
                string dTypeName = csvLine[0];
                if (String.IsNullOrEmpty(dTypeName))
                {
                    return;
                }
                string fgName = csvLine[1];
                string bgName = csvLine[2];
                string bdName = csvLine[3];
                DayLabelStyle style = new(fgName, bgName, bdName);
                DayLabelStyles[dTypeName.ToUpper()] = style;
            });
            OnPropertyChanged(nameof(DayLabelStyles));
        }
    }
}
