using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Popups;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace SecretSantaPicker
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PersonPage : Page
    {

        Player currentPlayer = Player.BlankPlayer;

        public PersonPage()
        {
            this.InitializeComponent();
            Windows.UI.Core.SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = Windows.UI.Core.AppViewBackButtonVisibility.Visible;
            Windows.UI.Core.SystemNavigationManager.GetForCurrentView().BackRequested += PersonPage_BackRequested;
        }

        private void PersonPage_BackRequested(object sender, Windows.UI.Core.BackRequestedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainPage));
        }

        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainPage));
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            currentPlayer = e.Parameter as Player;
            UpdateContent();
        }

        private void UpdateContent()
        {
            PlayerSet.SaveState();
            listView.Header = currentPlayer.Name+"'s Exclusions";
            listView.Items.Clear();
            foreach (string s in currentPlayer.Exclusions)
            {
                listView.Items.Add(s);
            }
        }

        private async void showButton_Click(object sender, RoutedEventArgs e)
        {
            if(string.IsNullOrEmpty(currentPlayer.BuyingFor))
            {
                MessageDialog er = new MessageDialog("Go back and click Shuffle.", "No Assignment.");
                await er.ShowAsync();
                return;
            }
            MessageDialog dialog = new MessageDialog(currentPlayer.Name + ", you are buying for "+currentPlayer.BuyingFor, currentPlayer.BuyingFor);
            
            await dialog.ShowAsync();
        }

        private async void addExcButton_Click(object sender, RoutedEventArgs e)
        {
            SelectNameDialog dialog = new SelectNameDialog();
            List<string> names = new List<string>();
            foreach(Player p in PlayerSet.Set)
            {
                if (p == currentPlayer) continue;
                if (currentPlayer.Exclusions.Contains(p.Name)) continue;
                names.Add(p.Name);
            }
            dialog.SetNameList(names);

            ContentDialogResult result = await dialog.ShowAsync();

            if(result == ContentDialogResult.Primary)
            {
                if(!string.IsNullOrEmpty(dialog.SelectedName))
                {
                    currentPlayer.Exclusions.Add(dialog.SelectedName);
                    PlayerSet.Set.ClearAssignments();
                    UpdateContent();
                }
            }
        }


        string contextMenuPlayer = "";
        private void listView_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            ListView view = sender as ListView;
            object o = ((FrameworkElement)e.OriginalSource).DataContext;
            if (o == null) return;
            string s = o as string;
            if (s.Equals(currentPlayer.Name)) return;
            contextMenuPlayer = s;
            menuFlyoutContext.ShowAt(view, e.GetPosition(view));
        }


        private void listView_Holding(object sender, HoldingRoutedEventArgs e)
        {
            ListView view = sender as ListView;
            object o = ((FrameworkElement)e.OriginalSource).DataContext;
            if (o == null) return;
            string s = o as string;
            if (s.Equals(currentPlayer.Name)) return;
            contextMenuPlayer = s;
            menuFlyoutContext.ShowAt(view, e.GetPosition(view));
        }


        private void MenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(contextMenuPlayer)) return;
            currentPlayer.Exclusions.Remove(contextMenuPlayer);
            UpdateContent();
        }

        private void Page_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if(e.Key == Windows.System.VirtualKey.Back)
                this.Frame.Navigate(typeof(MainPage));
        }
    }
}
