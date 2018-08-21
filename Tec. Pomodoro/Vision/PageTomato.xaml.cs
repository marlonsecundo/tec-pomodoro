using System;
using System.Threading.Tasks;
using Tec.Pomodoro.Domain;
using Tec.Pomodoro.Util;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Core;
using Windows.ApplicationModel.ExtendedExecution;
using Windows.Foundation.Collections;
using Windows.Media.Playback;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

namespace Tec.Pomodoro.Vision
{
    public sealed partial class PageTomato : Page
    {
        private bool visible = true;
        private Tomato tomato;
        private InternetConnection netConnection;
        private ExtendedExecutionSession newSession;

        private string[] str_images = new string[]
        {
            "tomato",
            "tomatoGreen",
            "clock",
            "tomatoClock",
            "tomatoGreenClock",
            "clockClock"
        };

        public PageTomato()
        {
            this.InitializeComponent();

            ShowPage.Begin();
            Window.Current.VisibilityChanged += Current_VisibilityChanged;

        }

        private void Current_VisibilityChanged(object sender, VisibilityChangedEventArgs e)
        {
            visible = e.Visible;
        }

        public async void BeginExecutedExtension()
        {
            CancelExecutedExtension();

            newSession = new ExtendedExecutionSession();
            newSession.Reason = ExtendedExecutionReason.Unspecified;
            newSession.Description = "Timer Pomodoro Tomato";
            newSession.Revoked += NewSession_Revoked;
            ExtendedExecutionResult result = await newSession.RequestExtensionAsync();

            switch (result)
            {
                case ExtendedExecutionResult.Allowed:
                    tomato.Start();
                    break;
                case ExtendedExecutionResult.Denied:
                    Notifier.GetInstance(Dispatcher).ShowMessageNotification(NotificationType.Error, false);

                    CancelExecutedExtension();

                    tomato.Cancel();
                    var loader = new Windows.ApplicationModel.Resources.ResourceLoader();

                    ucStopNotice.ChangeMessage(loader.GetString("ToastInterrupt0/Text"));

                    ShowStopNotice.Begin();
                    

                    break;
            }
        }


        public void CancelExecutedExtension()
        {
            if (newSession != null)
            {
                newSession.Revoked -= NewSession_Revoked;
                newSession.Dispose();
                newSession = null;
            }
        }

        private void NewSession_Revoked(object sender, ExtendedExecutionRevokedEventArgs args)
        {
            if (tomato.Status != TomatoStatus.Stop)
            {
                Notifier.GetInstance(Dispatcher).ShowMessageNotification(NotificationType.Error, visible);

                tomato.Cancel();

                var loader = new Windows.ApplicationModel.Resources.ResourceLoader();

                ucStopNotice.ChangeMessage(loader.GetString("ToastInterrupt0/Text"));

                ShowStopNotice.Begin();

            }
        }

        private async void InternetAbled()
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.High, () =>
            {
                if (GridStopNotice.Opacity == 0)
                {
                    CancelExecutedExtension();

                    tomato.Cancel();
                    var loader = new Windows.ApplicationModel.Resources.ResourceLoader();

                    ucStopNotice.ChangeMessage(loader.GetString("ToastInterrupt0/Text"));

                    ShowStopNotice.Begin();

                    Notifier.GetInstance(Dispatcher).ShowMessageNotification(NotificationType.Interrupt, visible);

                }

            });

        }

        private async void Tomato_TimerTickEvent(object status)
        {
            int min = tomato.TimerCount.Minutes;
            int sec = tomato.TimerCount.Seconds;

            string txtSec = sec.ToString();
            string txtMin = min.ToString();

            if (sec < 10)
                txtSec = "0" + txtSec;

            if (min < 10)
                txtMin = "0" + txtMin;

            await Dispatcher.RunAsync(CoreDispatcherPriority.High, () =>
            {
                TxtTime.Text = txtMin + ":" + txtSec;

            });
        }

        private async void Tomato_TimerCompleteEvent(object status)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.High, () =>
            {
                ListTomatos.SelectedIndex++;

                ChangeImageItem(ListTomatos.SelectedIndex);

                SnapImage1.Begin();

                NotificationType type = NotificationType.Free;

                switch(tomato.Status)
                {
                    case TomatoStatus.Focus:
                        type = NotificationType.Focus;
                        break;
                    default:
                        type = NotificationType.Free;
                        break;
                }

                Notifier.GetInstance(Dispatcher).ShowMessageNotification(type, visible);

            });
        }

        private async void Tomato_TimerStopEvent(object status)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.High, () =>
            {
                ShowStopNotice.Begin();
                Notifier.GetInstance(Dispatcher).ShowMessageNotification(NotificationType.Finish, visible);

                CancelExecutedExtension();
            });
        }

        private async void BtCancel_Click(object sender, RoutedEventArgs e)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.High, () =>
            {
                tomato.Cancel();
                ShowStopNotice.Begin();
                CancelExecutedExtension();
            });
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            object[] data = (object[])e.Parameter;


            tomato = (Tomato)data[0];
            string[] tasks = (string[])data[1];

            foreach (string task in tasks)
                ListTask.Items.Add(new CustomItemList(task));

            tomato.TimerCompleteEvent += Tomato_TimerCompleteEvent;
            tomato.TimerTickEvent += Tomato_TimerTickEvent;
            tomato.TimerStopEvent += Tomato_TimerStopEvent;
            LoadList();

            TxtTime.Text = "";
            BeginExecutedExtension();

            if (!tomato.AbleInternet)
            {
                netConnection = new InternetConnection(InternetAbled);
                netConnection.StartVerification();
            }



        }

        private async void ChangeImageItem(int index)
        {
            int previousIndex = index - 1;

            if (previousIndex >= 0)
            {
                string name = str_images[(int)tomato.PreviousStatus];
                Image img = (Image)ListTomatos.Items[previousIndex];
                img.Source = await (GetImage(name + ".png"));
            }

            string imgName = str_images[(int)tomato.Status] + "Clock";
            Image img1 = (Image)ListTomatos.Items[index];
            img1.Source = await GetImage(imgName + ".png");
        }

        private async Task<BitmapImage> GetImage(string name)
        {
            var fdr = await Package.Current.InstalledLocation.GetFolderAsync("Assets");
            StorageFolder folder = await fdr.GetFolderAsync("Images");
            StorageFile file = await folder.GetFileAsync(name);
            var stream = await file.OpenAsync(FileAccessMode.Read);

            BitmapImage bi = new BitmapImage();
            bi.SetSource(stream);

            return bi;
        }

        private async void LoadList()
        {
            int index = 1;
            int tomatosCount = tomato.Replays * 2;
            BitmapImage[] bis = new BitmapImage[3];


            var fdr = await Package.Current.InstalledLocation.GetFolderAsync("Assets");
            StorageFolder folder = await fdr.GetFolderAsync("Images");

            for (int i = 0; i < bis.Length; i++)
            {
                StorageFile file = await folder.GetFileAsync(str_images[i] + ".png");
                using (var stream = await file.OpenAsync(FileAccessMode.Read))
                {
                    bis[i] = new BitmapImage();
                    bis[i].SetSource(stream);
                }
            }

            for (int i = 1; i < tomatosCount + 1; i++)
            {
                if (index == 0 && !(i >= 8 && i % 8 == 0))
                    index = 1;
                else if (i >= 8 && i % 8 == 0)
                    index = 2;
                else if (index == 1)
                    index = 0;
                else if (index == 2)
                    index = 0;


                Image img = new Image();
                img.Source = bis[index];

                ListTomatos.Items.Add(img);
            }

            ListTomatos.SelectedIndex = 0;
            ChangeImageItem(ListTomatos.SelectedIndex);
        }

        private void DoubleAnimationUsingKeyFrames_Completed(object sender, object e)
        {
            switch (tomato.Status)
            {
                case TomatoStatus.Focus:
                    SnapState2.Begin();
                    IconMenu.Foreground = iconCancel.Foreground = new SolidColorBrush(Color.FromArgb(255, 213, 51, 73));
                    break;
                case TomatoStatus.Free:
                case TomatoStatus.Waiting:
                    SnapState1.Begin();
                    IconMenu.Foreground = iconCancel.Foreground = new SolidColorBrush(Color.FromArgb(255, 8, 170, 110));
                    break;
            }

            SnapImage2.Begin();
        }

        private void BtMenu_Click(object sender, RoutedEventArgs e)
        {
            if (SplitMenu.IsPaneOpen)
                SplitMenu.IsPaneOpen = false;
            else
                SplitMenu.IsPaneOpen = true;
        }





    }
}
