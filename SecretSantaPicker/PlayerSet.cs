using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace SecretSantaPicker
{
    public class PlayerSet : List<Player>
    {

        Random rand = new Random();

        public bool Shuffle()
        {
            if (this.Count < 2) return false;
            // brute force random
            for(int attempt = 0; attempt < 100; attempt++)
            {
                ClearAssignments();
                for (int i = 0; i < 100; i++)
                {
                    Player player1 = GetUnassignedPlayerWithMostExclusions();
                    // If blank player1, job done.
                    if (string.IsNullOrEmpty(player1.Name))
                        return true;
                    Player player2 = GetRandomUnboughtforPlayer(player1);
                    // if blank, then no unbought for players left.
                    if (string.IsNullOrEmpty(player2.Name))
                        break;
                    if (!player1.Name.Equals(player2.Name) && !player1.Exclusions.Contains(player2.Name))
                    {
                        player2.BoughtBy = player1.Name;
                        player1.BuyingFor = player2.Name;
                    }
                }
            }
            
            return false;
        }

        public bool ContainsName(string name)
        {
            foreach (Player p in this)
                if (p.Name.Equals(name)) return true;
            return false;
            
        }

        public new bool Remove(Player player)
        {
            bool r = base.Remove(player);
            if(r)
            {
                foreach (Player p in this)
                    p.Exclusions.Remove(player.Name);
            }
            return r;
        }

        public void ClearAssignments()
        {
            foreach (Player p in this)
            {
                p.BoughtBy = "";
                p.BuyingFor = "";
            }
        }

        Player GetUnassignedPlayerWithMostExclusions()
        {
            List<Player> sorted = new List<Player>();
            foreach(Player p in this)
            {
                if (string.IsNullOrWhiteSpace(p.BuyingFor))
                    sorted.Add(p);
            }
            if (sorted.Count < 1) return Player.BlankPlayer;
            sorted.Sort((p1, p2) => { return p1.Exclusions.Count.CompareTo(p2.Exclusions.Count); });
            return sorted[sorted.Count-1];
        }

        Player GetRandomUnboughtforPlayer(Player exceptThisOne)
        {
            List<Player> unbought = new List<Player>();
            foreach (Player p in this)
            {
                if (p == exceptThisOne) continue;
                if (string.IsNullOrEmpty(p.BoughtBy))
                    unbought.Add(p);
            }
            if(unbought.Count == 0) return Player.BlankPlayer;
            int i = rand.Next(unbought.Count);
            return unbought[i];
        }

        // Singleton
        private PlayerSet() { }
        private static PlayerSet set;
        public static PlayerSet Set
        {
            get
            {
                if (set == null) set = new PlayerSet();
                return set;
            }
        }

        public static void LoadState()
        {
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            object s = localSettings.Values[typeof(PlayerSet).ToString()];
            if (s == null && !(s is string)) return;

            string[] ss = (s as string).Split(new string[] { Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries );
            PlayerSet set = new PlayerSet();
            foreach (string p in ss)
                set.Add(Player.LoadFromString(p));
            PlayerSet.set = set;
        }

        public static void SaveState()
        {
            StringBuilder sb = new StringBuilder();
            foreach(Player p in Set)
            {
                sb.AppendLine(p.SaveAsString());
            }
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            localSettings.Values[typeof(PlayerSet).ToString()] = sb.ToString();
        }
    }
}
