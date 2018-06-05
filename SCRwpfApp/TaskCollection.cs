using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCRwpfApp
{
    class TaskCollection
    {
        public List<TaskSCR> list = new List<TaskSCR>();
        public List<TaskSCR> listQueue = new List<TaskSCR>();
        public TaskCollection(string json, bool completed)
        {
            if (completed)
            {
                dynamic data = JsonConvert.DeserializeObject<dynamic>(json);
                if (data != null)
                {
                    list = ((IDictionary<string, JToken>)data).Select(k =>
                        JsonConvert.DeserializeObject<TaskSCR>(k.Value.ToString())).ToList();
                }
            }
            else
            {
                dynamic data = JsonConvert.DeserializeObject<dynamic>(json);
                if (data != null)
                {
                    listQueue = ((IDictionary<string, JToken>)data).Select(k => JsonConvert.DeserializeObject<TaskSCR>(k.Value.ToString())).ToList();

                }

            }
        }
    }
}