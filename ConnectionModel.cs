using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SixtyNames.ConnectionModel
{
    internal class ConnectionModel
    {
        [JsonProperty("connectionString")]
        public string ConnectionString { get; set; }

    }
}
