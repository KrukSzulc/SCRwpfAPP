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
    class TaskSCR
    {

        public TaskSCR()
        {
        }

        public TaskSCR(string json)
        {
            JArray jArray = JArray.Parse(json);

            Debug.WriteLine(jArray.ToString()+"   jArray  ");
            JObject jObject = JObject.Parse(json);
            JToken jUser = jObject["Completed"];
            content = (int)jUser["content"];
            id = (string)jUser["id"];
        }
        [JsonProperty("a")]
        public int a { get; set; }
        [JsonProperty("b")]
        public int b { get; set; }
        [JsonProperty("content")]
        public int content { get; set; }
        [JsonProperty("id")]
        public string id { get; set; }
        [JsonProperty("uuid")]
        public string uuid { get; set; }
        [JsonProperty("specialUuid")]
        public string specialuuid { get; set; }
        [JsonProperty("blocked")]
        public bool blocked;
        [JsonProperty("time")]
        public DateTime time;
    }
}
