
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
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
        static int ILOSCWYKONANYCH = 0;
        static int DELAYREFRESH = 200; // jak czesto odswieza liste i wysyla zapytania o dane
        static int DELAYTIMER = 1000;
        static int DELAYCOMPLETED = 5000; // jak dluggo ma czekac w przypadku gdy jest jedno completed
        static int id = 0;
     //   static int counterToEnd = 0;
        static int again = 0;
        List<Thread> threads = new List<Thread>();
        static Firebase myDAO = new Firebase();
   
        public MainWindow()
        {
            InitializeComponent();
            createTASKS();
             
       
            threads.Add(new Thread(pushTASKS));
            threads.Add(new Thread(createFromCompletedTasks));
           threads.Add(new Thread(loadCompletedList));
            threads.Add(new Thread(loadQueueTasks));
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
            aTimer.Interval = DELAYTIMER;
            aTimer.Enabled = true;
       
        }

        private void OnTimedEvent(object s, EventArgs a)
        {
          
            List<TaskSCR> tempList = collection.listReadCompleted;
            Dispatcher.Invoke(() =>
            {
                listCompleted.Items.Clear();
                foreach (TaskSCR task in tempList)
                {
                    this.listCompleted.Items.Add(task.uuid.ToString() + "  suma: " + task.content);
                }
            });
            Dispatcher.Invoke(() =>
            {
                send_label.Content = ILOSC;
                    });
            Dispatcher.Invoke(() =>
            {
                receive_label.Content = ILOSCWYKONANYCH;
            });
            Dispatcher.Invoke(() =>
                {
                    listQueue.Items.Clear();
                List<TaskSCR> myTempList = collection.listReadQueue;
                foreach (TaskSCR task in myTempList)
                {
                    TimeSpan span = DateTime.Now.Subtract(task.time);
                    string time = span.Minutes + ":" + span.Seconds;
                    this.listQueue.Items.Add(task.uuid.ToString() + "     liczby:  "+task.a.ToString()+", "+task.b.ToString()+"  time: " + time);
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
                   
                    string result = myDAO.READ2();
                    collection.convert(result, true);
                    collection.releaseCompletedMutex();

                    List<TaskSCR> list = collection.getCompletedList(false);
                    if (list.Count < 2)
                    {
                        if (again==2)  // Sprawdza szybciej czy jest zadanie niż są one wykonywane na telefonie, więc jeśli jest 1 sprawdza po 1s. czy dalej jest jedno
                        {

                            MessageBox.Show("Suma: "+list[0].content);

                            foreach (Thread thread in threads)
                            {
                                thread.Abort();
                            }
                            //TaskSCR newTask = createTask(list[0].content, 0, list[0].id+1);
                            //ILOSC++;
                            //ILOSCWYKONANYCH++;
                            //myDAO.DELETE(list[0]);
                            //list.RemoveAt(0);
                            //again = false;

                            //collection.addQueueList(newTask, false);
                        }
                        else
                        {
                            again++;
                            Thread.Sleep(5000);
                        }
                    }
                    else
                    {
                        again = 0;
                        TaskSCR newTask = createTask(list[0].content, list[1].content, list[0].id+1);
                        ILOSC++;
                        ILOSCWYKONANYCH += 2;
                        myDAO.DELETE(list[0]);
                        myDAO.DELETE(list[1]);

                        list.RemoveAt(1);
                        list.RemoveAt(0);

                        collection.addQueueList(newTask, false);
                    }
                    collection.releaseCompletedMutex();
                    Thread.Sleep(300);


                }
                catch (Exception e)
                {
                    Debug.WriteLine("my massage is : " + e.Message.ToString());
                }

            }

            //        List<TaskSCR> tempList = collection.getCompletedList(true);
            //        List<TaskSCR> list = collection.getCompletedList(false);
            //        string result = myDAO.READ2();
            //        collection.releaseCompletedMutex();
            //        collection.convert(result, true);
            //        collection.setList(list, tempList);
            //        collection.releaseCompletedMutex();

            //        if (tempList.Count < 2)
            //        {
            //            if (again)  /// Sprawdza szybciej czy jest zadanie niż są one wykonywane na telefonie, więc jeśli jest 1 sprawdza po 1s. czy dalej jest jedno
            //            {
            //                TaskSCR newTask = createTask(tempList[0].content, 0, id);


            //                myDAO.DELETE(tempList[0]);
            //                tempList.RemoveAt(0);
            //                again = false;

            //                collection.addQueueList(newTask, false);
            //            }
            //            else
            //            {
            //                again = true;
            //                Thread.Sleep(2000);

            //            }
            //        }
            //        else
            //        {
            //            again = false;
            //            TaskSCR newTask = createTask(tempList[0].content, tempList[1].content, id);


            //            myDAO.DELETE(tempList[0]);
            //            myDAO.DELETE(tempList[1]);
            //            tempList.RemoveAt(1);
            //            tempList.RemoveAt(0);

            //            collection.addQueueList(newTask, false);
            //        }
            //        collection.releaseCompletedTempMutex();
            //        Thread.Sleep(300);


            //    }
            //    catch (Exception e)
            //    {
            //        Debug.WriteLine("my massage is : " + e.Message.ToString());
            //    }

            //}

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
                        if (myDAO.CHECKCOUNT())
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
