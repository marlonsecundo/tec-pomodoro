using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Tec.Pomodoro.Vision
{
    public sealed partial class PageWarning : Page
    {
        public PageWarning()
        {
            this.InitializeComponent();
            ShowPage.Begin();
        }

        private void BtBack_Click(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }

        private void BtContinue_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(PageDistractions), true);
        }
    }
}
