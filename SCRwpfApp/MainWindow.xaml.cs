
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
using System.Windows.Threading;

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
        static int ILOSC = 6; // ilosc liczb  // jak zostaje 1 completed i 1 zadanie to robi zadaniedodaje do completed i zawsze zostaje 1 nieskonczona petla
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
           // Task.Run(() => pushTASKS());
           // startTimer();
            threads.Add(new Thread(pushTASKS));
            threads.Add(new Thread(createFromCompletedTasks));
          //  threads.Add(new Thread(loadCompletedList));
            threads.Add(new Thread(loadQueueTasks));
            //threads.Add(new Thread(startTimer));
            startTimer();
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
            //var timer = new DispatcherTimer();
            //timer.Interval = TimeSpan.FromSeconds(1);
            //timer.Tick += new EventHandler(async (object s, EventArgs a) =>
            //{

            //    OnTimedEvent(s,a);

            //});
            //timer.Start();
        }

        private void OnTimedEvent(object s, EventArgs a)
        {
            Debug.WriteLine("lalala");
            List<TaskSCR> tempList = collection.listReadCompleted;
            Dispatcher.Invoke(() =>
            {
                listCompleted.Items.Clear();
                foreach (TaskSCR task in tempList)
                {
                    this.listCompleted.Items.Add(task.uuid.ToString() + "     suma" + task.content);
                }
            });
                Dispatcher.Invoke(() =>
                {
                    listQueue.Items.Clear();
                List<TaskSCR> myTempList = collection.listReadQueue;
                foreach (TaskSCR task in myTempList)
                {
                    TimeSpan span = DateTime.Now.Subtract(task.time);
                    string time = span.Minutes + ":" + span.Seconds;
                    this.listQueue.Items.Add(task.uuid.ToString() + "   time: " + time);
                }
            });
        }
        public void loadCompletedList()
        {
            while (true)
            {
               
                loadCompletedTasks();
                Thread.Sleep(2000);
            }
        }
        public void createFromCompletedTasks()
        {
            while (true)
            {
                try
                {
                 
                    List<TaskSCR> tempList = collection.getCompletedList(true);
                    if (tempList.Count < 2)
                    {
                        List<TaskSCR> list = collection.getCompletedList(false);
                        string result = myDAO.READ2();
                        collection.releaseCompletedMutex();
                        collection.convert(result, true);
                        collection.releaseCompletedMutex();
                        collection.setList(list, tempList);

                    }
                           
           //         if (tempList.Count > 2)
                    else
                    {

                        Debug.WriteLine("wykonujeeee wykonujeeeewykonujeeeewykonujeeeewykonujeeeewykonujeeeewykonujeeee");
                        TaskSCR newTask = createTask(tempList[0].content, tempList[1].content, id);
                        collection.addQueueList(newTask, false);
                        // myDAO.DELETE(tempList[0]);
                        //Thread.Sleep(500);
                        // myDAO.DELETE(tempList[1]);
                        //Thread.Sleep(500);


                        tempList[0].blocked = false;
                        myDAO.UPTADECOMPL(tempList[0]);
                        tempList.RemoveAt(0);
                        tempList[0].blocked = false;
                        myDAO.UPTADECOMPL(tempList[0]);
                        tempList.RemoveAt(0);
                      

                        
                     
                    }


                        //    Debug.WriteLine("my masssage blocked : -1");
                        //    if (collection.getCompletedList(false).Count <= 2)
                        //    {

                        //        string result = myDAO.READ2();
                        //        collection.convert(result, true);
                        //    }
                        //    collection.releaseCompletedMutex();
                        ////    myDAO.releaseReadMutex();


                        //    collection.setList(collection.getCompletedList(false), collection.getCompletedList(true));
                        //    collection.releaseCompletedMutex(); collection.releaseCompletedTempMutex();    
                        //    Debug.WriteLine("my masssage blocked : 0.5");
                        //    List<TaskSCR> tempList = collection.getCompletedList(true);
                        //    Debug.WriteLine("my masssage blocked : 1");
                        //    //    if (tempList.Select(x => x.id == id).Count() > 2)
                        //    if(tempList.Count>2)
                        //    {
                        //        Debug.WriteLine("my masssage blocked : 2");
                        //        TaskSCR newTask = createTask(tempList[0].content, tempList[1].content, tempList[0].id);

                        //        Debug.WriteLine(newTask.a + " sdads" + newTask.b);

                        //        collection.addQueueList(newTask, false); //todo tu zatrzymuje
                        //        Debug.WriteLine("my masssage blocked : 3");
                        //        //  collection.releaseQueueMutex();
                        //        List<TaskSCR> listCompleted = collection.getCompletedList(false);
                        //        Debug.WriteLine("my masssage blocked : 4");
                        //        myDAO.DELETE(tempList[0]);
                        //        myDAO.DELETE(tempList[1]);
                        //        Debug.WriteLine("my masssage blocked : 5");
                        //        listCompleted.Remove(tempList[0]);
                        //        listCompleted.Remove(tempList[1]);
                        //        Debug.WriteLine("my masssage blocked : 6");
                        //        tempList.Clear();
                        //    }
                        //    collection.releaseCompletedTempMutex(); collection.releaseCompletedMutex();
                }
                catch (Exception e)
                {
                    Debug.WriteLine("my massage is : " + e.Message.ToString());
                }

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
       //     myDAO.releaseReadMutex();
            collection.convertToRead(result);
           

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
