using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Popups;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace SecretSantaPicker
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            ApplicationView.PreferredLaunchViewSize = new Size(360, 640);
            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;
        }
        

        private async void addButton_Click(object sender, RoutedEventArgs e)
        {
            NewPlayerDialog dialog = new NewPlayerDialog();
            await dialog.ShowAsync();
            string name = dialog.NewPlayerName;
            if(string.IsNullOrEmpty(name) || PlayerSet.Set.ContainsName(name))
            {
                MessageDialog er = new MessageDialog("Try again.", "Name is in use or invalid.");
                await er.ShowAsync();
                return;
            }
            Player p = new Player(name);
            PlayerSet.Set.Add(p);
            PlayerSet.Set.ClearAssignments();
            UpdateContent();
        }

        void UpdateContent()
        {
            PlayerSet.SaveState();
            listView.Items.Clear();
            foreach (Player p in PlayerSet.Set)
                listView.Items.Add(p);
        }
        

        private void listView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Player player = listView.SelectedItem as Player;
            NavigateToPlayer(player);
        }

        void NavigateToPlayer(Player player)
        {
            this.Frame.Navigate(typeof(PersonPage), player);
        }

        private async void shuffleButton_Click(object sender, RoutedEventArgs e)
        {
            listView.SelectedIndex = -1;
            bool shuffleOk = PlayerSet.Set.Shuffle();
            if(!shuffleOk)
            {
                MessageDialog dialog = new MessageDialog("Do you have enough players with not too many exclusions?", "Shuffle failed.");
                await dialog.ShowAsync();
            }
            else {
                MessageDialog dialog = new MessageDialog("Click a player name to see who they're buying for.", "Shuffle succeeded.");
                await dialog.ShowAsync();
            }
            UpdateContent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            
            listView.SelectedIndex = -1;
        }
        

        Player contextMenuPlayer = Player.BlankPlayer;
        private void listView_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            ListView view = sender as ListView;
            object o = ((FrameworkElement)e.OriginalSource).DataContext;
            if (o == null) return;
            contextMenuPlayer = o as Player;
            menuFlyoutContext.ShowAt(view, e.GetPosition(view));
        }
        

        private void MenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            if (contextMenuPlayer.IsBlank) return;
            PlayerSet.Set.Remove(contextMenuPlayer);
            PlayerSet.Set.ClearAssignments();
            UpdateContent();
        }
        

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            PlayerSet.LoadState();
            UpdateContent();
        }

    }
}
