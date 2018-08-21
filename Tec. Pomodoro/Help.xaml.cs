using Tec.Pomodoro.Vision;
using Windows.ApplicationModel.Background;
using Windows.ApplicationModel.Core;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Tec.Pomodoro
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Help : Page
    {
        public Help()
        {
            this.InitializeComponent();
            ShowPage.Begin();
            NextStep();
        }

        int index = -1;
        

        string[] icons = new string[]
        {
            "\uE292",
            "\uE916",
            "\uE945",
            "\uE7FC",
            "\uEA98",
            "\uE117",
            "\uE121"
        };

        private void BtJump_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(PageSilencePhone));
            SaveSettings();
        }

        private void BtNext_Click(object sender, RoutedEventArgs e)
        {
            PivotMain.SelectedIndex = 1;
            ShowItem1.Begin();
        }

        private void BtNext1_Click(object sender, RoutedEventArgs e)
        {
            if (IconConfirm.Visibility == Visibility.Visible)
                SnapWord.Begin();
            else
            {
                PivotMain.SelectedIndex = 2;
                ShowItem2.Begin();
            }
        }

        private void NextStep()
        {
            index++;

            var loader = new Windows.ApplicationModel.Resources.ResourceLoader();


            TxtTitle.Text = loader.GetString("Tec"+(index + 1)+"/Text");
            Icon.Glyph = icons[index];

            if (index >= 6)
            {
                IconConfirm.Visibility = Visibility.Collapsed;
                IconNext.Visibility = Visibility.Visible;
            }
        }

        private void BtNext2_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(PageSilencePhone));
            SaveSettings();
        }

        public void SaveSettings()
        {
            ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
            localSettings.Values[App.ISFIRSTTIME] = false;

        }

        private void SnapWord_Completed(object sender, object e)
        {
            NextStep();
            SnapWord1.Begin();
        }
    }
}

