using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SCRwpfApp
{
    class TaskCollection
    {
        public List<TaskSCR> listReadQueue = new List<TaskSCR>();

        public List<TaskSCR> listCompletedTasks = new List<TaskSCR>();
        public List<TaskSCR> tempListCompletedTasks = new List<TaskSCR>();

        public List<TaskSCR> listQueueTasks = new List<TaskSCR>();
        public List<TaskSCR> tempListQueueTasks = new List<TaskSCR>();

     //   public SpinLock Completed = new SpinLock();
  //      public SpinLock Queue = new SpinLock();
    //    public SpinLock QueueTemp = new SpinLock();
      //  public SpinLock CompletedTemp = new SpinLock();

        public Mutex Completed =    new Mutex();
        public Mutex Queue =        new Mutex();
        public Mutex QueueTemp =    new Mutex();
       public Mutex CompletedTemp =new Mutex();
        Boolean lockedQueue = false;
        Boolean lockedQueueTemp = false;
        Boolean lockedCompleted = false;
        Boolean lockedCompletedTemp = false;
        public void addQueueList(TaskSCR myTask, bool isTemp)
        {
            if (isTemp)
            {
                blockQueueTempMutex();
                tempListQueueTasks.Add(myTask);
                releaseQueueTempMutex();
            }
            else
            {
                blockQueueMutex();
                listQueueTasks.Add(myTask);
                releaseQueueMutex();
            }
        }
        public void addCompletedList(TaskSCR myTask, bool isTemp)
        {
            if (isTemp)
            {
                blockCompletedTempMutex();
                tempListCompletedTasks.Add(myTask);
                releaseCompletedTempMutex();
            }
            else
            {
                blockCompletedMutex();
                listCompletedTasks.Add(myTask);
                releaseCompletedMutex();
            }
        }
        public List<TaskSCR> getQueueList(bool isTemp)
        {
            if (isTemp)
            {
                blockQueueTempMutex();
                return tempListQueueTasks;
              //todo dodac realeasy w kodzie
            }
            else
            {
                blockQueueMutex();
                return listQueueTasks;
             
            }
        }
        public List<TaskSCR> getCompletedList(bool isTemp)
        {
            if (isTemp)
            {
                blockCompletedTempMutex();
                return tempListCompletedTasks;
    
            }
            else
            {
                blockCompletedMutex();
                return listCompletedTasks;
           
            }
        }
        
        public void setList(List<TaskSCR> sourceList,List<TaskSCR> changedList)
        {
            changedList.Clear();

            foreach (TaskSCR myTask in sourceList)
            {
                changedList.Add(myTask);
            }
        }
        public void convert(string json, bool completed)
        {
            if (completed)
            {
                dynamic data = JsonConvert.DeserializeObject<dynamic>(json);
                if (data != null)
                {
                    blockCompletedMutex();
                    listCompletedTasks = ((IDictionary<string, JToken>)data).Select(k =>
                        JsonConvert.DeserializeObject<TaskSCR>(k.Value.ToString())).ToList();
                    releaseCompletedMutex();
                }
            }
            else
            {
                dynamic data = JsonConvert.DeserializeObject<dynamic>(json);
                if (data != null)
                {     
                    listReadQueue = ((IDictionary<string, JToken>)data).Select(k => JsonConvert.DeserializeObject<TaskSCR>(k.Value.ToString())).ToList();  
                }

            }
        }


        public void blockCompletedMutex()
        {
            // Completed.WaitOne();
            try
            {
                Completed.WaitOne();
                //   Completed.Enter(ref lockedCompleted);
                Thread.Sleep(1000);
                Debug.WriteLine("SPIN LOCKED");
            }
            catch
            { 
            }
        }
        public void releaseCompletedMutex()
        {
          //  if (lockedCompleted) Completed.Exit();
            Debug.WriteLine("SPIN RELEASE");
             Completed.Dispose();
        }
        public void blockQueueMutex()
        {
            try
            {
                //Queue.Enter(ref lockedQueue);
                //Thread.Sleep(1000);
                //Debug.WriteLine("SPIN LOCKED");
                Queue.WaitOne();
            }
            catch
            {
            }
          // .. Queue.WaitOne();
        }
        public void releaseQueueMutex()
        {

           // if (lockedQueue) Queue.Exit();
            Debug.WriteLine("SPIN RELEASE");
             Queue.Dispose();
        }
        public void blockCompletedTempMutex()
        {
            try
            {
                CompletedTemp.WaitOne();
                //CompletedTemp.Enter(ref lockedCompletedTemp);
                Thread.Sleep(1000);
                Debug.WriteLine("SPIN LOCKED");
            }
            catch
            {
            }
          //  CompletedTemp.WaitOne();
        }
        public void releaseCompletedTempMutex()
        {
         //   if (lockedCompletedTemp) CompletedTemp.Exit();
            Debug.WriteLine("SPIN RELEASE");
            CompletedTemp.Dispose();
        }
        public void blockQueueTempMutex()
        {
            try
            {
                QueueTemp.WaitOne();
                //QueueTemp.Enter(ref lockedQueueTemp);
                Thread.Sleep(1000);
                Debug.WriteLine("SPIN LOCKED");
            }
            catch
            {
            }
           // QueueTemp.WaitOne();
        }
        public void releaseQueueTempMutex()
        {
          //  if (lockedQueueTemp) QueueTemp.Exit();
            Debug.WriteLine("SPIN RELEASE");
            QueueTemp.Dispose();
        }
    }
}