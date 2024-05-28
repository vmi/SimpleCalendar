using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace SimpleCalendar.WinUI3.Views.Controls
{
    public class Label : Control
    {
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            nameof(Text),
            typeof(string),
            typeof(Label),
            PropertyMetadata.Create(""));

        public string Text { get => (string)GetValue(TextProperty); set => SetValue(TextProperty, value); }

        public Label()
        {
            DefaultStyleKey = typeof(Label);
        }
    }
}
