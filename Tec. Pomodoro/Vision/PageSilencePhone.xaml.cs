using System;
using Windows.Foundation.Collections;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Networking.Connectivity;
using Windows.Storage;
using Windows.System.Threading;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;

namespace Tec.Pomodoro.Vision
{ 
    public sealed partial class PageSilencePhone : Page
    {
        ThreadPoolTimer thread;
        
        public PageSilencePhone()
        {
            this.InitializeComponent();
                      
            ShowPage.Begin();
            
        }
   


        private void BtNext_Click(object sender, RoutedEventArgs e)
        {
            thread.Cancel();
            Frame.Navigate(typeof(PageDistractions) , false);
        }

        private async void VerifyDistractions(ThreadPoolTimer timer)
        {

            if (!IsInternet())
            {
                await Dispatcher.RunAsync(CoreDispatcherPriority.High, () =>
                {
                    if ((ShowButton.GetCurrentState() == ClockState.Stopped || ShowButton.GetCurrentState() == ClockState.Filling) && BtNext.Visibility != Visibility.Visible)
                        ShowButton.Begin();
                });
            }
            else
            {
                await Dispatcher.RunAsync(CoreDispatcherPriority.High, () =>
                {
                    if ((HideButton.GetCurrentState() == ClockState.Stopped || HideButton.GetCurrentState() == ClockState.Filling) && BtNext.Visibility != Visibility.Collapsed)
                        HideButton.Begin();
                });
            }

        }

        public static bool IsInternet()
        {
            try
            {
                ConnectionProfile connections = NetworkInformation.GetInternetConnectionProfile();
                bool internet = connections != null && connections.GetNetworkConnectivityLevel() == NetworkConnectivityLevel.InternetAccess;
                return internet;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private void ShowPage_Completed(object sender, object e)
        {
            TimeSpan period = TimeSpan.FromMilliseconds(500);

            thread = ThreadPoolTimer.CreatePeriodicTimer(VerifyDistractions, period);
        }

        private void BtJump_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(PageWarning));
        }

        private void BtHelp_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Help));
        }

        private void BtInfo_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(PageInfo));
        }
    }
}
