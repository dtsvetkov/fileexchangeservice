using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileExchangerClient.Settings
{
    public class ClientSettings
    {
        public string DownloadsPath { get; set; }
        public int ChunkSize { get; set; }
    }
}
