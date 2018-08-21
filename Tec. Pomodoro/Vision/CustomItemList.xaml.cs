using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace Tec.Pomodoro.Vision
{
    public sealed partial class CustomItemList : UserControl
    {
        public CustomItemList(string text)
        {
            this.InitializeComponent();
            TxtTask.Text = Task = text;
        }

        public string Task { get; }

        public bool IsComplete { get; set; }

        private void Grid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            IsComplete = !IsComplete;
            if (IsComplete)
                TaskComplete.Begin();
            else
                TaskIncomplete.Begin();
        }
    }
}
