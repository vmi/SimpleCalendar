using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SimpleCalendar.WPF.Services;
using System.Windows.Media;

namespace SimpleCalendar.WPF.ViewModels
{
    public record struct DayLabelStyle(
        Brush? Foreground,
        Brush? Background,
        Brush? Border);

    public partial class DayLabelStyleViewModel : ObservableObject
    {
        private readonly SettingsService settingsService;
        private readonly BrushConverter brushConverter = new();

        [ObservableProperty]
        private DayLabelStyle _sunday;

        [ObservableProperty]
        private DayLabelStyle _monday;

        [ObservableProperty]
        private DayLabelStyle _tuesday;

        [ObservableProperty]
        private DayLabelStyle _wednesday;

        [ObservableProperty]
        private DayLabelStyle _thursday;

        [ObservableProperty]
        private DayLabelStyle _friday;

        [ObservableProperty]
        private DayLabelStyle _saturday;

        [ObservableProperty]
        private DayLabelStyle _holiday;

        [ObservableProperty]
        private DayLabelStyle _specialday1;

        [ObservableProperty]
        private DayLabelStyle _specialday2;

        [ObservableProperty]
        private DayLabelStyle _specialday3;

        [ObservableProperty]
        private DayLabelStyle _today;

        [ObservableProperty]
        private DayLabelStyle _focus;

        public DayLabelStyleViewModel(SettingsService settingsService)
        {
            this.settingsService = settingsService;
            LoadSetting();
        }

        private Brush? ToBrush(string? name) => String.IsNullOrEmpty(name) ? null : (Brush?) brushConverter.ConvertFromString(name);

        [RelayCommand]
        private void LoadSetting()
        {
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
                DayLabelStyle style = new(ToBrush(fgName), ToBrush(bgName), ToBrush(bdName));
                switch (dTypeName.ToLower())
                {
                    case "SUNDAY": Sunday = style; break;
                    case "MONDAY": Monday = style; break;
                    case "TUESDAY": Tuesday = style; break;
                    case "WEDNESDAY": Wednesday = style; break;
                    case "THURSDAY": Thursday = style; break;
                    case "FRIDAY": Friday = style; break;
                    case "SATURDAY": Saturday = style; break;
                    case "HOLIDAY": Holiday = style; break;
                    case "SPECIALDAY1": Specialday1 = style; break;
                    case "SPECIALDAY2": Specialday2 = style; break;
                    case "SPECIALDAY3": Specialday3 = style; break;
                    case "TODAY": Today = style; break;
                    case "FOCUS": Focus = style; break;
                    default: break;
                }
            });
        }
    }
}
