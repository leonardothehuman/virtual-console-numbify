using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Virtual_Console_Numbify_fw {
    public class Config {
        public static readonly string UpdateUrl = "https://leonardothehuman.com/static-api/vc-numbify.json";
        public static readonly int CurrentVersion = 1;
        public static readonly int UpdateCheckInterval = 60 * 60 * 24;
    }
}
