using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Timelogger
{
    public class Project
    {
        public String Name;


        private String _RootDir = "";
        public String RootDir {
            get { return _RootDir; } 
            set { _RootDir = value;
                Keywords.Add(value);
                LoadKeywords();
            } }

        private String Keywordfile { get { return RootDir + @"\keywords.txt"; } }

        public List<String> Keywords = new List<String>();


        public Project() { }
        public Project(String n) 
        {
            Name = n;
        }


        public Boolean Check(Activity act)
        {
            if (act != null)
            {
                
                return CheckKeywords(act.Filename) || CheckKeywords(act.Title);
            }
            
            return false;
        }

        public Boolean CheckKeywords(String s)
        {
            if (s != null)
            {
                return Keywords.Any(pattern => s.Contains(pattern));
            }
            return false;
        }

        public void LoadKeywords()
        {
            if (File.Exists(Keywordfile))
            {
                string[] lines = File.ReadAllLines(Keywordfile);
                foreach (string line in lines)
                    Keywords.Add(line);
            }
        }
    }
}
