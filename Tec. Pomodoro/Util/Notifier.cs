using Windows.ApplicationModel;
using Windows.Data.Xml.Dom;
using Windows.Storage;
using Windows.UI.Notifications;
using System;
using Windows.ApplicationModel.Resources;
using System.Diagnostics;
using Microsoft.Toolkit.Uwp.Notifications;
using Windows.UI.Xaml.Controls;
using Windows.UI.Core;

namespace Tec.Pomodoro.Util
{
    public enum NotificationType : int
    {
        Error = 0,
        Finish = 1,
        Interrupt = 2,
        Focus = 3,
        Free = 4
    }

    public class Notifier
    {

        private CoreDispatcher Dispatcher;
        private static Notifier _notifier = null;
        private MediaElement player;
        private ResourceLoader loader;

        private Notifier(CoreDispatcher dispatcher)
        {
            Dispatcher = dispatcher;
            player = new MediaElement();
            loader = new ResourceLoader();
            LoadSound();
        }

        private async void LoadSound()
        {
            StorageFolder fdr = await Package.Current.InstalledLocation.GetFolderAsync("Assets");
            fdr = await fdr.GetFolderAsync("Sounds");
            StorageFile file = await fdr.GetFileAsync("alarm.wav");
            var stream = await file.OpenAsync(FileAccessMode.Read);
            player.SetSource(stream, "");
        }

        public static Notifier GetInstance(CoreDispatcher dispatcher)
        {
            if (_notifier == null)
                _notifier = new Notifier(dispatcher);

            return _notifier;
        }


        public void ShowMessageNotification(NotificationType type, bool alarm)
        {
            alarm = !alarm;
            string resource = "";

            resource = "Toast" + type.ToString();

            string title = loader.GetString(resource + 0 + "/Text");
            string text = loader.GetString(resource + 1 + "/Text");
            string image = "Assets/Images/" + loader.GetString(resource + "Image" + "/Text");

            ToastContent content = new ToastContent()
            {
                Launch = "app-defined-string",

                Visual = new ToastVisual()
                {
                    BindingGeneric = new ToastBindingGeneric()
                    {
                        Children =
                        {
                            new AdaptiveText()
                            {
                                Text = title
                            },

                            new AdaptiveText()
                            {
                                Text = text
                            }
                        },

                        AppLogoOverride = new ToastGenericAppLogo()
                        {
                            Source = image
                        },
                    },

                }
            };


            if (type == NotificationType.Finish || type == NotificationType.Focus || type == NotificationType.Free)
                content.Audio = new ToastAudio()
                {
                    Src = new Uri("ms-appx:///Assets/Sounds/alarm.wav")
                };

            if (alarm)
                Show(content.GetXml());
            else
                PlaySound();
        }

        private void Show(XmlDocument xml)
        {
            ToastNotification notification = new ToastNotification(xml);
            var notifier = ToastNotificationManager.CreateToastNotifier();
            notifier.Show(notification);
        }

        private async void PlaySound()
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                player.Play();
            });
        }
        
    }
}
