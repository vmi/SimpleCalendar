using System;
using System.ComponentModel;
using System.Globalization;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using SimpleCalendar.WinUI3.Models;
using SimpleCalendar.WinUI3.ViewModels;

namespace SimpleCalendar.WinUI3.Views.Controls
{
    public partial class DayLabel : Control, INotifyPropertyChanged
    {
        public static readonly DependencyProperty DayItemProperty = DependencyProperty.Register(
            nameof(DayItem),
            typeof(DayItem),
            typeof(DayLabel),
            new PropertyMetadata(DayItem.EMPTY, OnDayItemPropertyChanged));

        public DayItem DayItem { get => (DayItem)GetValue(DayItemProperty); set => SetValue(DayItemProperty, value); }

        public static readonly DependencyProperty ContentProperty = DependencyProperty.Register(
            nameof(Content),
            typeof(string),
            typeof(DayLabel),
            PropertyMetadata.Create(""));

        public string Content { get => (string)GetValue(ContentProperty); set => SetValue(ContentProperty, value); }

        private static void OnDayItemPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DayLabel obj && e.NewValue is DayItem dayItem)
            {
                obj.Content = dayItem.DayString;
                obj.DayType = dayItem.DayType;
                obj.IsDayTypeEmpty = dayItem.DayType == DayType.EMPTY;
                if (string.IsNullOrEmpty(dayItem.Label))
                {
                    obj.ToolTip = null;
                }
                else
                {
                    obj.ToolTip = dayItem.Label;
                }
                obj.IsToday_PropertyChanged();
            }
        }

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

        public event PropertyChangedEventHandler PropertyChanged;

        public bool IsDayTypeEmpty { get => (bool)GetValue(IsDayTypeEmptyProperty); private set => SetValue(IsDayTypeEmptyProperty, value); }

        public bool IsToday { get => GetIsToday(); }

        private bool GetIsToday()
        {
            if (DataContext is CalendarMonthViewModel calMon)
            {
                MainWindowViewModel curMon = calMon.CurrentMonth;
                var today = curMon.Today;
                var yearMonth = calMon.YearMonth;
                return today.Year == yearMonth.Year && today.Month == yearMonth.Month && today.Day == DayItem.Day;
            }
            else
            {
                return false;
            }
        }

        public string ToolTip { get => (string)((ToolTip)ToolTipService.GetToolTip(this)).Content; set => ((ToolTip)ToolTipService.GetToolTip(this)).Content = value; }

        public DayLabel() : base()
        {
            var toolTip = new ToolTip();
            ToolTipService.SetToolTip(this, toolTip);
            Loaded += DayLabel_Loaded;
        }

        public void IsToday_PropertyChanged()
        {
            PropertyChanged(this, new PropertyChangedEventArgs(nameof(IsToday)));
        }

        private void DayLabel_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is CalendarMonthViewModel calMon)
            {
                MainWindowViewModel curMon = calMon.CurrentMonth;
                calMon.PropertyChanged += IsTodaySource_PropertyChanged;
                curMon.PropertyChanged += IsTodaySource_PropertyChanged;
            }
        }

        private void IsTodaySource_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (sender is DayLabel dayLabel)
            {
                dayLabel.IsToday_PropertyChanged();
            }
        }
    }
}
