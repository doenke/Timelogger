using Microsoft.Office.Interop.Excel;
using Microsoft.Office.Interop.Word;
using Microsoft.Office.Interop.PowerPoint;
using Microsoft.Office.Interop.Outlook;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;


namespace Timelogger
{
    public class Activity : INotifyPropertyChanged
    {

        private String _Title;
        private String _Filename;
        private String _ProcessName;
        private IntPtr _Handle;
        private uint _ThreadID;
        public event PropertyChangedEventHandler PropertyChanged;

        public Project ActiveProject { set; get; }

        public String ProjectName { get{
            if (ActiveProject == null) return "";
                return ActiveProject.Name;
            } }

        public ActivityManager Manager;


        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public IntPtr Handle
        {
            get { return _Handle; }
            protected set
            {
                _Handle = value;
                NotifyPropertyChanged();
            }
        }

        public uint ThreadID
        {
            get { return _ThreadID; }
            protected set
            {
                _ThreadID = value;
                NotifyPropertyChanged();
            }
        }
        public String Title
        {
            get { return _Title; }
            protected set
            {
                _Title = value;
                NotifyPropertyChanged();
            }
        }

        public String Filename
        {
            get { return _Filename; }
            protected set
            {
                _Filename = value;
                NotifyPropertyChanged();

            }
        }

        public String ProcessName
        {
            get { return _ProcessName; }
            protected set
            {
                _ProcessName = value;
                NotifyPropertyChanged();

            }
        }

        protected DateTime Start;
        protected DateTime End;


        public String StartString { get { return this.Start.ToString(); } }
        public String EndString { get { return this.End.ToString(); } }



        private Int16 MinSeconds = 2;
        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll")]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);


        [DllImport("user32.dll", SetLastError = true)]
        static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        public Activity() {
            this.Start = DateTime.Now;
            this.End = DateTime.Now;
        }

        public Activity(ActivityManager m) :this()
        {
            
            this.Manager = m;

            const int nChars = 256; ;
            StringBuilder Buff = new StringBuilder(nChars);
            Handle = GetForegroundWindow();
            uint processid = 0;
            ThreadID = GetWindowThreadProcessId(Handle, out processid);

            Process localById = Process.GetProcessById((int)processid);
            ProcessName = localById.ProcessName;

            if (GetWindowText(Handle, Buff, nChars) > 0)
            {
                Title = Buff.ToString();
            }


            switch (ProcessName)
            {
                case "EXCEL":
                {
                    var Excel = (Microsoft.Office.Interop.Excel.Application)Marshal.GetActiveObject("Excel.Application");
                    Filename = Excel.ActiveWorkbook.FullName;
                    break;
                }
                case "WINWORD":
                    {
                        var Word = (Microsoft.Office.Interop.Word.Application)Marshal.GetActiveObject("Word.Application");
                        Filename = Word.ActiveDocument.FullName;
                        break;
                    }
                case "POWERPNT":
                    {
                        var ppt = (Microsoft.Office.Interop.PowerPoint.Application)Marshal.GetActiveObject("Powerpoint.Application");
                        Filename = ppt.ActivePresentation.FullName;
                        break;
                    }
                case "OUTLOOK":
                    {
                        var Outlook = (Microsoft.Office.Interop.Outlook.Application)Marshal.GetActiveObject("Outlook.Application");
                        Filename = "";
                        break;
                    }

                case "chrome":
                    {
                        if (Title.Contains("Exact Synergy")) Title = "Esynergy";
                        else Title = "Google Chrome";
                        break;
                    }

                case "Teams":
                    {
                        if (Title.Contains("|"))
                        {
                            Filename = Title.Split('|')[0].Trim();
                            Title = Title.Split('|')[1].Trim();
                        }
                        break;
                    }
            }

            ActiveProject = Manager.PM.FindProject(this);

        }


        public void Finish()
        {
            this.End = DateTime.Now;
            NotifyPropertyChanged("EndString");
            NotifyPropertyChanged("Seconds");
            NotifyPropertyChanged("Minutes");
            NotifyPropertyChanged("IsRelevant");
        }

        public Boolean IsSame(Activity old)
        {
            if (this.Title != null && this.Manager.IgnoreTitles.Any(pattern => Regex.IsMatch(this.Title, pattern))) return true;
            if (this.ProcessName != null && this.Manager.IgnoreProcesses.Any(pattern => Regex.IsMatch(this.ProcessName, pattern))) return true;
            if (old is not object) return false;
            return old.Handle == this.Handle && old.Title == this.Title && old.ProcessName == this.ProcessName;
        }

        public Boolean IsRelevant
        { 
            get
            {
                if (this.Seconds > this.MinSeconds)
                    return true;
                else                
                    return false;
            }
        }

        public Int64 Seconds
        {
            get
            {
               return (long)this.End.Subtract(this.Start).TotalSeconds;
            }
        }

        public Int64 Minutes
        {
            get
            {
                return (long)this.End.Subtract(this.Start).TotalMinutes;
            }
        }


        public override string ToString()
        {
            List<String> ExportList = new List<String>();
            ExportList.Add(StartString);
            ExportList.Add(EndString);
            ExportList.Add(Title);
            ExportList.Add(Filename);
            return String.Join("\t", ExportList);
        }
    }



    public class Pause : Activity
    {
        public Pause(ActivityManager m)
        {
            Manager = m;
            Title = "Pause";
            Filename = "Pause";
            ProcessName = "Pause";


        }
    }
}
