using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SCRwpfApp
{
    class Firebase
    {
        string SPECIALUUID = "";
        public void CHECKTIME(int delay)
        {
           
                TaskCollection collection = new TaskCollection();
            collection.convert(READQUEUE(), false);
                foreach(TaskSCR myTask in collection.list)
                {
                    TimeSpan span = DateTime.Now.Subtract(myTask.time);
                    if (span.TotalSeconds>=delay&&myTask.blocked==true)
                    {
                        UPTADE(myTask);
                
                    }
                }
         }
        public bool CHECKCOUNT()
        {
            var request = (HttpWebRequest)WebRequest.CreateHttp("https://real-time-systems-project.firebaseio.com/Queue/.json");
            request.ContentType = "application/json: charset=utf-8";
            var response = request.GetResponse() as HttpWebResponse;
            using (Stream responseStream = response.GetResponseStream())
            {
                StreamReader read = new StreamReader(responseStream, Encoding.UTF8);
                string s = read.ReadToEnd();
                TaskCollection collection = new TaskCollection();
                collection.convert(s, false);
               

                if (collection.listQueue.Count >= 10)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            
        }

        public TaskSCR GET()
        {
            throw new NotImplementedException();
        }
        public void POSTUUID(TaskSCR task)
        {
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(new
            {
                uuid = task.uuid,
            });

            var request = WebRequest.CreateHttp("https://real-time-systems-project.firebaseio.com/Queue/.json");
            request.Method = "POST";

            request.ContentType = "application/json";
            var buffer = Encoding.UTF8.GetBytes(json);
            request.ContentLength = buffer.Length;
            request.GetRequestStream().Write(buffer, 0, buffer.Length);
            var response = request.GetResponse();
            json = (new StreamReader(response.GetResponseStream())).ReadToEnd();
        }
        public void POST(TaskSCR task)
        {
            DateTime date = DateTime.Now;
            string time = date.ToString("HH:mm:ss");
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(new
            {
                content = task.content,
                id = "",
                a = task.a,
                b = task.b,
                uuid = task.uuid,
                time = time,
                specialUuid = SPECIALUUID,
                blocked = false,
            });
          
            var request = WebRequest.CreateHttp("https://real-time-systems-project.firebaseio.com/Queue/.json");
            request.Method = "POST";

            request.ContentType = "application/json";
            var buffer = Encoding.UTF8.GetBytes(json);
            request.ContentLength = buffer.Length;
            request.GetRequestStream().Write(buffer, 0, buffer.Length);
            var response = request.GetResponse();
            json = (new StreamReader(response.GetResponseStream())).ReadToEnd();
            UPTADEUUID(JsonName.convert(json));
        }


        public void UPTADE(TaskSCR task) {
            DateTime date = DateTime.Now;
            string time = date.ToString("HH:mm:ss");
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(new
            {
                time = time,
                blocked = false,
                id = "",
                specialUuid = "",
            });

            var request = WebRequest.CreateHttp("https://real-time-systems-project.firebaseio.com/Queue/" + task.uuid + ".json");
            request.Method = "PATCH";
            request.ContentType = "application/json";
            var buffer = Encoding.UTF8.GetBytes(json);
            request.ContentLength = buffer.Length;
            request.GetRequestStream().Write(buffer, 0, buffer.Length);
            var response = request.GetResponse();
            json = (new StreamReader(response.GetResponseStream())).ReadToEnd();
        }
        public void UPTADEUUID(string uuid)
        {
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(new
            {
                uuid = uuid,
            });

            var request = WebRequest.CreateHttp("https://real-time-systems-project.firebaseio.com/Queue/"+uuid+".json");
            request.Method = "PATCH";
            request.ContentType = "application/json";
            var buffer = Encoding.UTF8.GetBytes(json);
            request.ContentLength = buffer.Length;
            request.GetRequestStream().Write(buffer, 0, buffer.Length);
            var response = request.GetResponse();
            json = (new StreamReader(response.GetResponseStream())).ReadToEnd();
        }
        public string READ()
        {
            var request = (HttpWebRequest)WebRequest.CreateHttp("https://real-time-systems-project.firebaseio.com/Completed/.json");
            request.ContentType = "application/json: charset=utf-8";
            var response = request.GetResponse() as HttpWebResponse;
            using (Stream responseStream = response.GetResponseStream())
            {
                StreamReader read = new StreamReader(responseStream, Encoding.UTF8);
                return read.ReadToEnd();
            }
        }
        public string READQUEUE()
        {
            var request = (HttpWebRequest)WebRequest.CreateHttp("https://real-time-systems-project.firebaseio.com/Queue/.json");
            request.ContentType = "application/json: charset=utf-8";
            var response = request.GetResponse() as HttpWebResponse;
            using (Stream responseStream = response.GetResponseStream())
            {
                StreamReader read = new StreamReader(responseStream, Encoding.UTF8);
                return read.ReadToEnd();
            }
        }
    }

    class JsonName
    {
        [JsonProperty("name")]
        string name;

        static public string convert(string jsonString)
        {
           // dynamic data = JsonConvert.DeserializeObject<dynamic>(jsonString);
          //  var list = new List<JsonName>();
            //foreach (var itemDynamic in data)
            //{
            //    list.Add(JsonConvert.DeserializeObject<JsonName>(((JProperty)itemDynamic).Value.ToString()));
            //}
            JObject ob = JObject.Parse(jsonString);
           
            string s = ob.GetValue("name").ToString();
            return s;

        }
}
}
