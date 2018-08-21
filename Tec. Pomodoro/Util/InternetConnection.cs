using System;
using Windows.Networking.Connectivity;
using Windows.System.Threading;

namespace Tec.Pomodoro.Util
{
    public delegate void InternetConnectionHandler();

    public class InternetConnection
    {
        private ThreadPoolTimer thread;
        public event InternetConnectionHandler InternetConnected;
        
        public InternetConnection(InternetConnectionHandler methodHandler)
        {
            InternetConnected += methodHandler;
        }

        public bool IsConnected()
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

        public void StartVerification()
        {
            thread = ThreadPoolTimer.CreatePeriodicTimer(OnTimer, TimeSpan.FromSeconds(2));
        }

        public void StopVerification()
        {
            thread.Cancel();
        }

        private void OnTimer(ThreadPoolTimer timer)
        {
            if (IsConnected())
            {
                InternetConnected();
                StopVerification();
            }
        }
    }
}
