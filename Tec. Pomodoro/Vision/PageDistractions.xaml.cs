using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Tec.Pomodoro.Vision
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PageDistractions : Page
    {
        public PageDistractions()
        {
            this.InitializeComponent();
            ShowPage.Begin();
        }

        private bool isConnected = false;

        private void BtOk_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(PageConfigTasklist), isConnected);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            isConnected = (bool)e.Parameter;
        }
    }
}
