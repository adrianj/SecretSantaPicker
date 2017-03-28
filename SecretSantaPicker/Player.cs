using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecretSantaPicker
{
    public class Player
    {
        private string name = "";
        public string Name { get { return name; } set { name = value.Replace(':', '_'); } }

        public HashSet<string> Exclusions { get; private set; } = new HashSet<string>();

        public string BuyingFor { get; set; } = "";

        public string BoughtBy { get; set; } = "";

        public bool IsBlank { get { return string.IsNullOrEmpty(Name); } }
        

        public Player(string name)
        {
            Name = name;
        }

        public override string ToString()
        {
            return Name;
        }

        public static Player BlankPlayer = new Player("");
    
        public string SaveAsString()
        {
            string s = Name;
            s += ":" + BuyingFor;
            s += ":" + BoughtBy;
            foreach(string ex in Exclusions)
            {
                s += ":" + ex;
            }
            return s;
        }

        public static Player LoadFromString(string s)
        {
            string[] ss = s.Split(new char[] { ':' });
            if (ss.Length < 1) return BlankPlayer;
            Player ret = new Player(ss[0]);
            if (ss.Length < 2) return ret;
            ret.BuyingFor = ss[1];
            if (ss.Length < 3) return ret;
            ret.BoughtBy = ss[2];
            for (int i = 3; i < ss.Length; i++)
                ret.Exclusions.Add(ss[i]);
            return ret;
        }
    }
}
