using System;
using System.ComponentModel;
using System.Runtime.Versioning;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using SimpleCalendar.WinUI3.Models;
using SimpleCalendar.WinUI3.ViewModels;
using SimpleCalendar.WPF.Utilities;

namespace SimpleCalendar.WinUI3.Views.Controls
{
    [SupportedOSPlatform("windows")]
    public partial class DayLabel : Label
    {
        public static readonly DependencyProperty DayItemProperty = DependencyProperty.Register(
            nameof(DayItem),
            typeof(DayItem),
            typeof(DayLabel),
            new PropertyMetadata(DayItem.EMPTY, OnDayItemPropertyChanged));

        public DayItem DayItem { get => (DayItem)GetValue(DayItemProperty); set => SetValue(DayItemProperty, value); }

        public static readonly DependencyProperty DayTypeProperty = DependencyProperty.Register(
            nameof(DayType),
            typeof(DayType),
            typeof(DayLabel),
            PropertyMetadata.Create(DayType.EMPTY));

        public DayType DayType { get => (DayType)GetValue(DayTypeProperty); private set => SetValue(DayTypeProperty, value); }

        public static readonly DependencyProperty IsDayTypeEmptyProperty = DependencyProperty.Register(
            nameof(IsDayTypeEmpty),
            typeof(bool),
            typeof(DayLabel),
            PropertyMetadata.Create(true));

        public bool IsDayTypeEmpty { get => (bool)GetValue(IsDayTypeEmptyProperty); private set => SetValue(IsDayTypeEmptyProperty, value); }

        private static void OnDayItemPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DayLabel obj && e.NewValue is DayItem dayItem)
            {
                obj.DayType = dayItem.DayType;
                string propName;
                switch (obj.DayType)
                {
                    case DayType.SUNDAY: propName = nameof(DayLabelStyleSettingViewModel.SundayBrush); break;
                    case DayType.MONDAY: propName = nameof(DayLabelStyleSettingViewModel.MondayBrush); break;
                    case DayType.TUESDAY: propName = nameof(DayLabelStyleSettingViewModel.TuesdayBrush); break;
                    case DayType.WEDNESDAY: propName = nameof(DayLabelStyleSettingViewModel.WednesdayBrush); break;
                    case DayType.THURSDAY: propName = nameof(DayLabelStyleSettingViewModel.ThursdayBrush); break;
                    case DayType.FRIDAY: propName = nameof(DayLabelStyleSettingViewModel.FridayBrush); break;
                    case DayType.SATURDAY: propName = nameof(DayLabelStyleSettingViewModel.SaturdayBrush); break;
                    case DayType.HOLIDAY: propName = nameof(DayLabelStyleSettingViewModel.HolidayBrush); break;
                    case DayType.SPECIALDAY1: propName = nameof(DayLabelStyleSettingViewModel.Specialday1Brush); break;
                    case DayType.SPECIALDAY2: propName = nameof(DayLabelStyleSettingViewModel.Specialday2Brush); break;
                    case DayType.SPECIALDAY3: propName = nameof(DayLabelStyleSettingViewModel.Specialday3Brush); break;
                    default: obj.Text = ""; propName = null; break;
                }
                if (!string.IsNullOrEmpty(propName))
                {
                    Binding binding = new() { Path = new PropertyPath($"DayLabelStyleSetting.{propName}") };
                    obj.SetBinding(ForegroundProperty, binding);
                }
                obj.Text = dayItem.DayString;
                obj.IsDayTypeEmpty = dayItem.DayType == DayType.EMPTY;
                if (string.IsNullOrEmpty(dayItem.Label))
                {
                    obj.ToolTip = null;
                }
                else
                {
                    obj.ToolTip = dayItem.Label;
                }
                DayLabel.IsTodaySource_PropertyChanged(obj, default);
            }
        }

        public string ToolTip { get => (string)((ToolTip)ToolTipService.GetToolTip(this)).Content; set => ((ToolTip)ToolTipService.GetToolTip(this)).Content = value; }

        public DayLabel() : base()
        {
            var toolTip = new ToolTip();
            ToolTipService.SetToolTip(this, toolTip);
            Loaded += DayLabel_Loaded;
            PointerEntered += DayLabel_PointerEntered;
            PointerExited += DayLabel_PointerExited;
        }

        private void DayLabel_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is CalendarMonthViewModel calMon)
            {
                MainWindowViewModel curMon = calMon.CurrentMonth;
                calMon.RegisterPropertyChangedEventHandler(IsTodaySource_PropertyChanged);
                curMon.RegisterPropertyChangedEventHandler(IsTodaySource_PropertyChanged);
            }
        }

        private static void IsTodaySource_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender is DayLabel dayLabel && dayLabel.DataContext is CalendarMonthViewModel calMon)
            {
                MainWindowViewModel curMon = calMon.CurrentMonth;
                DateOnly today = curMon.Today;
                YearMonth ym = calMon.YearMonth;
                if (today.Year == ym.Year && today.Month == ym.Month && today.Day == dayLabel.DayItem.Day)
                {
                    dayLabel.Background = calMon.DayLabelStyleSetting.TodayBrush;
                }
                else
                {
                    dayLabel.Background = DayLabelStyleSettingViewModel.DefaultBackgroundBrush;
                }
            }
        }
        private static void DayLabel_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            if (sender is DayLabel dayLabel && dayLabel.DataContext is CalendarMonthViewModel calMon)
            {
                if (dayLabel.DayType != DayType.EMPTY)
                {
                    dayLabel.Background = calMon.DayLabelStyleSetting.MouseOverBrush;
                }
            }
        }

        private static void DayLabel_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            IsTodaySource_PropertyChanged(sender, default);
        }

    }
}
