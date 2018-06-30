using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrainCommon.Model
{
    public class BackData
    {
        //[JsonProperty("resultMessage")]
        public String result_message { get; set; }

        //[JsonProperty("resultCode")]
        public String result_code { get; set; }

        //[JsonProperty("uamtk")]
        public String uamtk { get; set; }
    }
}
