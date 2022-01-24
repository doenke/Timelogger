using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Input;
using System.Windows;

namespace Timelogger
{
    public class ActivityManager
    {

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetCursorPos(ref Win32Point pt);

        [StructLayout(LayoutKind.Sequential)]
        internal struct Win32Point
        {
            public Int32 X;
            public Int32 Y;
        };
        public static Point GetMousePosition()
        {
            var w32Mouse = new Win32Point();
            GetCursorPos(ref w32Mouse);

            return new Point(w32Mouse.X, w32Mouse.Y);
        }

        public delegate void MyActivityEventHandler(object source, EventArgs e, Activity MyActivity);

        public event MyActivityEventHandler OnNewActivity;

        public ProjectManager PM = new ProjectManager();

        public ObservableCollection<Activity> Activities = new ObservableCollection<Activity>();
        private Activity CurrentActivity;

        private DateTime oldtime;
        private Double oldpointX=0;
        private Double oldpointY=0;
        

        public List<String> IgnoreTitles;
        public List<String> IgnoreProcesses;

        public readonly int MinSeconds = 1;

    public  ActivityManager()
        {
            IgnoreTitles = new List<String> { "Programmumschaltung", "Taskmanager" };
            IgnoreProcesses = new List<String> { "Timelogger" };

        }


        public void CheckActive()
        {

            Activity newAct;
            newAct = new Activity(this);

            CurrentActivity?.Finish();

            
            
            if (DateTime.Now.Subtract(oldtime).TotalSeconds > 300)
            {
                newAct = new Pause(this);
            }

            if (!newAct.IsSame(CurrentActivity))
            {

                if (CurrentActivity != null && CurrentActivity.Seconds > MinSeconds)

                    Activities.Add(CurrentActivity);
                CurrentActivity = newAct;
                if (OnNewActivity != null)
                {
                    OnNewActivity(this, new EventArgs(), CurrentActivity);
                }
            }
            //Application.Current.MainWindow.CaptureMouse();
            //var newpoint = Mouse.GetPosition(Application.Current.MainWindow);
            //Application.Current.MainWindow.ReleaseMouseCapture();
            var newpoint = GetMousePosition();

            if (oldpointX != newpoint.X || oldpointY != newpoint.Y)
            {
                oldpointX = newpoint.X;
                oldpointY = newpoint.Y;
                oldtime = DateTime.Now;
            }
        }
    }
}