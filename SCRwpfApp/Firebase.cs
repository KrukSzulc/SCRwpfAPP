using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SCRwpfApp
{
    class Firebase : FirebaseDAO
    {
      
        public bool CHECK()
        {
            throw new NotImplementedException();
        }

        public TaskSCR GET()
        {
            throw new NotImplementedException();
        }

        public void POST(TaskSCR task)
        {
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(new
            {
                conent = task.Name,
                id = task.id,
                uuid = task.uuid,
            });

            var request = WebRequest.CreateHttp("https://real-time-systems-project.firebaseio.com/Completed/.json");
            request.Method = "POST";

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
    }
}
