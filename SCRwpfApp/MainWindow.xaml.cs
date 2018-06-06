
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
using System.Timers;
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

    //działa dodawanie trzeba zrobić usuwanie i zwiększanie ID.
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        TaskCollection collection = new TaskCollection();
        static int DELAY = 30; // jak długo czeka na wykonanie zadania w Sekundach
        static int ILOSC = 0; // ilosc liczb
        static int ILOSCZADAN = 50;
        static int DELAYREFRESH = 1500;
        static int id = 0;
        List<Thread> threads = new List<Thread>();
        static Firebase myDAO = new Firebase();
        //IFirebaseConfig config;
        public MainWindow()
        {
            InitializeComponent();
            createTASKS();
            
           // Task.Run(() => loadCompletedTasks());
           // Task.Run(() => loadQueueTasks());
            //Task.Run(() => pushTASKS());
          //  startTimer();
            threads.Add(new Thread(pushTASKS));
            threads.Add(new Thread(createFromCompletedTasks));
            //threads.Add(new Thread(loadCompletedTasks));
            threads.Add(new Thread(loadQueueTasks));
            foreach (Thread thread in threads)
            {
                thread.Start();
            }
        }

        public void startTimer()
        {
            System.Timers.Timer aTimer = new System.Timers.Timer();
            aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            aTimer.Interval = 1000;
            aTimer.Enabled = true;

        }

        private void OnTimedEvent(object sender, ElapsedEventArgs e)
        {
            List<TaskSCR> tempList = collection.getCompletedList(false);
            listCompleted.Items.Clear();
            foreach (TaskSCR task in tempList)
            {
                this.listCompleted.Items.Add(task.uuid.ToString() + "     suma" + task.content);
            }
            collection.releaseCompletedMutex();

            listQueue.Items.Clear();
            List<TaskSCR> myTempList = collection.getQueueList(false);
            foreach (TaskSCR task in myTempList)
            {
                TimeSpan span = DateTime.Now.Subtract(task.time);
                string time = span.Minutes + ":" + span.Seconds;
                this.listQueue.Items.Add(task.uuid.ToString() + "   time: " + time);
            }
            collection.releaseQueueMutex();
        }

        public void createFromCompletedTasks()
        {
            while (true)
            {

                collection.setList(collection.getCompletedList(false), collection.getCompletedList(true));
                collection.releaseCompletedMutex();collection.releaseCompletedTempMutex();
                loadCompletedTasks();
                List<TaskSCR> tempList = collection.getCompletedList(true);
                if (tempList.Select(x => x.id == id).Count() > 2)
                {

                    TaskSCR newTask = createTask(tempList[0].content, tempList[1].content, tempList[0].id);

                    Debug.WriteLine(newTask.a + " sdads" + newTask.b);

                    collection.addQueueList(newTask, false); //todo tu zatrzymuje

                  //  collection.releaseQueueMutex();
                    List<TaskSCR> listCompleted = collection.getCompletedList(false);
                    listCompleted.Remove(tempList[0]);
                    listCompleted.Remove(tempList[1]);
                    tempList.Clear();
                }
                collection.releaseCompletedTempMutex();collection.releaseCompletedMutex();
            }
        }
     
        public void pushTASKS()
        {
            while (true){

                collection.setList(collection.getQueueList(false), collection.getQueueList(true));
                collection.releaseQueueMutex(); collection.releaseQueueTempMutex();

                List<TaskSCR> tempList = collection.getQueueList(true);
                List<TaskSCR> list = collection.getQueueList(false);
                //if (tempList.All(x => x.id != id))
                //{
                //    id++;
                //}
                if (tempList.Count != 0)
                {
                   
                    foreach (TaskSCR myTask in tempList)
                    {
                        if (myTask.id == id && myDAO.CHECKCOUNT())
                        {
                            list.Remove(myTask);
                            myDAO.POST(myTask);

                            
                        }
                    }
                  
                    Thread.Sleep(500);
                }
                collection.releaseQueueMutex();
                collection.releaseQueueTempMutex();
            //    collection.setList(collection.getQueueList(true), collection.getQueueList(false));
            //    collection.releaseQueueMutex(); collection.releaseQueueTempMutex();
            }
        }
        public void createTASKS()
        {
            List<int> liczby = new List<int>();
            for(int i =0; i< ILOSC; i++)
            {
                liczby.Add(1);
            }
            int j = ILOSC / 2 + ILOSC % 2;
            for (int i = 0; i < j; i++)
            {
               List<TaskSCR> tasksQueue =  collection.getQueueList(false);
                tasksQueue.Add(createTask(liczby[i], liczby[ILOSC-i-1], 0));
                collection.releaseQueueMutex();
            }
        }
        public void loadCompletedTasks()
        {
           
                string result = myDAO.READ();
                 collection.convert(result, true);

        }
        public void loadQueueTasks(){
            while (true)
            {
                string resultQueue = myDAO.READQUEUE();
                collection.convert(resultQueue, false);
                //Application.Current.Dispatcher.Invoke(new Action(() =>
                //        {
                           
                            List<TaskSCR> myTempList = collection.listReadQueue;
                            foreach (TaskSCR task in myTempList)
                            {
                                TimeSpan span = DateTime.Now.Subtract(task.time);
                            if (span.TotalSeconds >= DELAY && task.blocked == true)
                                {
                                    myDAO.UPTADE(task);
                                }
                            }
                            Thread.Sleep(DELAYREFRESH);
                  //      }));

            }
        }

        private void btn_post_Click(object sender, RoutedEventArgs e)
        {
          
            for (int i = 0; i < Int32.Parse(numberOfTasks.Text); i++)
            {
               // tempTasks.Add(createTask(1, 1, 0));
            }
      
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
                        task.id = id;
                        task.uuid = "ss";
                        myDAO.POST(task);
                    }
                }
            }));
        }
        public TaskSCR createTask(int a, int b, int ID)
        {
            TaskSCR task = new TaskSCR();
            task.content = 0;
            task.a = a;
            task.b = b;
            task.id = ID;
            task.uuid = "ss";
            return task;
        }
    }
}
