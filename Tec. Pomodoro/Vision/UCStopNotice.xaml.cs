using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Tec.Pomodoro.Vision
{
    public sealed partial class UCStopNotice : UserControl
    {
        public UCStopNotice()
        {
            this.InitializeComponent();
        }

        private void BtRepeat_Click(object sender, RoutedEventArgs e)
        {
            Frame frame = (Frame)Window.Current.Content;
            frame.Navigate(typeof(PageSilencePhone));
        }

        private void BtExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Exit();
        }

        public void ChangeMessage(string text)
        {
            TxtMessage.Text = text;
        }

    }
}
