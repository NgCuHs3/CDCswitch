using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDCswitchserver.net
{
    public class Talkcontent
    {
        public TalkCode TalkCode { get; set; }
      
    }
    //thông báo conver num này có giá là text
    [JsonConverter(typeof(StringEnumConverter))]
    public enum TalkCode
    {
        JUSTTALK,
        STARTEDGAME,
    }
}
