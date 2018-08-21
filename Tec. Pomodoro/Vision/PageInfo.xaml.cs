using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Tec.Pomodoro.Vision
{
    public sealed partial class PageInfo : Page
    {
        public PageInfo()
        {
            this.InitializeComponent();
            ShowPage.Begin();
        }

        private void BtBack_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(PageSilencePhone));
        }
    }
}
