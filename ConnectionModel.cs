using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SixtyNames.ConnectionModel
{
    /// <summary>
    /// Класс для работы со строкой подключения
    /// </summary>
    internal class ConnectionModel
    {
        [JsonProperty("connectionString")]
        public string ConnectionString { get; set; }

    }
}
