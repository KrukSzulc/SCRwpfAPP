
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SCRwpfApp
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        TaskCollection collection = new TaskCollection();
        static int DELAY = 30; // jak długo czeka na wykonanie zadania w Sekundach
        static int ILOSC = 100; // ilosc liczb
        static int id = 0;

        static Firebase myDAO = new Firebase();
        //IFirebaseConfig config;
        public MainWindow()
        {
            InitializeComponent();

            Task.Run(() => loadCompletedTasks());
            Task.Run(() => loadQueueTasks());
        }

        public async Task loadCompletedTasks()
        {
            while (true)
            {
                string result = myDAO.READ();
                 collection.convert(result, true);
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    listCompleted.Items.Clear();
                    foreach (TaskSCR task in collection.list)
                    {
                        this.listCompleted.Items.Add(task.uuid.ToString()+"     suma"+task.content);
                    }
                }));
                Thread.Sleep(1000);
            }
        }
        public async Task loadQueueTasks(){
            while (true)
            {
                string resultQueue = myDAO.READQUEUE();
                collection.convert(resultQueue, false);
                Application.Current.Dispatcher.Invoke(new Action(() =>
                        {
                            listQueue.Items.Clear();
                            foreach (TaskSCR task in collection.listQueue)
                            {
                                       TimeSpan span = DateTime.Now.Subtract(task.time);
                                string time = span.Minutes + ":" + span.Seconds;
                                this.listQueue.Items.Add(task.uuid.ToString()+"   time: "+time);
                            if (span.TotalSeconds >= DELAY && task.blocked == true)
                                {
                                    myDAO.UPTADE(task);
                                }
                            }
                        }));
            }
        }

        private async void btn_post_Click(object sender, RoutedEventArgs e)
        {
            Task.Run(() => add());
        }
        public async Task add()
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                for (int i = 0; i < Int32.Parse(numberOfTasks.Text); i++)
                {
                    if (myDAO.CHECKCOUNT())
                    {
                        TaskSCR task = new TaskSCR();
                        task.content = 0;
                        task.a = task.b = 1;
                        task.id = id.ToString();
                        id++;
                        myDAO.POST(task);
                    }
                }
            }));
        }
    }
}
