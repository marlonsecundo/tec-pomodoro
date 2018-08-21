using System.Linq;
using Tec.Pomodoro.Domain;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Tec.Pomodoro.Vision
{
    public sealed partial class PageConfigTomato : Page
    {
        public PageConfigTomato()
        {
            this.InitializeComponent();
            ShowPage.Begin();
        }

        private bool isConnected = false;
        private string[] tasks = null;

        private void BtComfirm_Click(object sender, RoutedEventArgs e)
        {
            int focus = int.Parse(TextFocus.Text);
            int free = int.Parse(TextDis.Text);
            int replay = int.Parse((string)(ComboReplay.SelectedItem as ComboBoxItem).Content);

            Tomato tomato = new Tomato(focus, free, replay, isConnected);

            object[] data = new object[2];
            data[0] = tomato;
            data[1] = tasks;

            Frame.Navigate(typeof(PageTomato), data);
        }

        private void TextNumber_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox tb = (TextBox)sender;

            string[] numbers = new string[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };


            string txt = "";

            if (tb.Text.Length > 0)
                txt = tb.Text[tb.Text.Length - 1].ToString();
            else
            {
                ShowORHideButton();
                return;
            }


            if (!numbers.Any(txt.Contains))
            {
                tb.Text = tb.Text.Replace(txt, "");
            }

            ShowORHideButton();
        }

        private void ShowORHideButton()
        {
            if (TextDis.Text.Length > 0 && TextFocus.Text.Length > 0)
            {
                int dis = int.Parse(TextDis.Text);
                int con = int.Parse(TextFocus.Text);

                if (dis >= 1 && con >= 1)
                    ShowButton.Begin();
                else if (BtComfirm.Visibility == Visibility.Visible)
                    HideButton.Begin();
            }


        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            object[] data = (object[])e.Parameter;
            isConnected = (bool) data[0];
            tasks = (string[])data[1]; 
            
        }

        private void BtBack_Click(object sender, RoutedEventArgs e)
        {
            object[] data = new object[2];
            data[0] = isConnected;
            data[1] = tasks;
            Frame.Navigate(typeof(PageConfigTasklist), data);
        }
    }
}