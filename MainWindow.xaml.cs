using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;



namespace Timelogger
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    /// 


    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public ActivityManager Acts;

        private System.Windows.Threading.DispatcherTimer Apptimer;


        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            Acts = new ActivityManager();
            Acts.OnNewActivity += new ActivityManager.MyActivityEventHandler(NewActivity);

            Apptimer = new System.Windows.Threading.DispatcherTimer();
            Apptimer.Interval = new TimeSpan(0, 0, 1);
            Apptimer.Tick += new EventHandler(OnTimedEvent);
            Apptimer.Start();

            ListActivities.ItemsSource = Acts.Activities;
        }

        private void NewActivity(object source, EventArgs e, Activity Act)
        {
            this.TextFilename.Text = Act.ProcessName;
            this.TextTitle.Text = Act.Title;
            this.TextPID.Text = Act.Handle.ToString();
            this.TextProject.Text = Act.ProjectName;

        }

        private void resultsListView_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
        {

        }

        private void CopySelectedValuesToClipboard()
        {
            var builder = new StringBuilder();
            foreach (var item in ListActivities.SelectedItems)
                builder.AppendLine(item.ToString());

            System.Windows.Clipboard.SetDataObject(builder.ToString());
        }

        private void OnTimedEvent(object sender, EventArgs e)
        {
            Acts.CheckActive();
        }

        private void ListActivities_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (sender != ListActivities) return;

            if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.C)
                CopySelectedValuesToClipboard();
        }
    }

}
